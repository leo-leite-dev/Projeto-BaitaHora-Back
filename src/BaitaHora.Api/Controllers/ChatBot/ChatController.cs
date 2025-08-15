using Microsoft.AspNetCore.Mvc;
using BaitaHora.Application.IServices.IChatbot;
using BaitaHora.Application.IRepositories;
using BaitaHora.Application.IServices; // <- adiciona a namespace das queries

namespace BaitaHora.Api.Controllers.Chatbot
{
    [ApiController]
    [Route("api/devchat")]
    public sealed class ChatController : ControllerBase
    {
        public record ChatIn(Guid CompanyId, string FromPhoneE164, string Text);
        public record ChatOut(string Reply, bool Done);

        private readonly IChatSessionStore _sessions;
        private readonly IChatbotQuickService _chat;
        private readonly ICompanyMemberRepository _companyMembers;     // continua para validar proId
        private readonly ICompanyPositionQueries _positionQueries;     // <- NOVO: queries de cargos

        public ChatController(
            IChatSessionStore sessions,
            IChatbotQuickService chat,
            ICompanyMemberRepository companyMembers,
            ICompanyPositionQueries positionQueries) // <- injeta queries
        {
            _sessions = sessions;
            _chat = chat;
            _companyMembers = companyMembers;
            _positionQueries = positionQueries; // <- guarda queries
        }

        private static string Norm(string? s) => (s ?? string.Empty).Trim().ToLowerInvariant();

        // Agora busca papéis usando o serviço de query (e não mais members)
        private async Task<string[]> GetAvailableRolesAsync(Guid companyId, CancellationToken ct)
        {
            var roles = await _positionQueries.ListActiveNamesAsync(companyId, ct);
            return roles
                .Where(r => !string.IsNullOrWhiteSpace(r))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(r => r, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        [HttpPost("message")]
        public async Task<ActionResult<ChatOut>> Message([FromBody] ChatIn input, CancellationToken ct)
        {
            var key = $"{input.CompanyId}:{input.FromPhoneE164}";
            var session = await _sessions.GetAsync(key, ct) ??
                          new ChatSession
                          {
                              Key = key,
                              CompanyId = input.CompanyId,
                              PhoneE164 = input.FromPhoneE164,
                              Stage = "start"
                          };

            switch (session.Stage)
            {
                case "start":
                    {
                        session.Stage = "askName";
                        await _sessions.SetAsync(session, ct);
                        return Ok(new ChatOut("Olá! Qual seu nome completo?", false));
                    }

                case "askName":
                    {
                        session.FullName = (input.Text ?? string.Empty).Trim();

                        var customerId = await _chat.EnsureCustomerUserAsyncMinimal(
                            session.CompanyId,
                            session.FullName!,
                            session.PhoneE164,
                            email: null,
                            ct);

                        session.CustomerId = customerId;
                        session.Stage = "askRoleOrPro";
                        await _sessions.SetAsync(session, ct);

                        var roles = await GetAvailableRolesAsync(session.CompanyId, ct);
                        var opts = roles.Length == 0 ? "Nenhuma função cadastrada" : string.Join(", ", roles);

                        return Ok(new ChatOut(
                            $"Prazer, {session.FullName}! Quer alguém específico? Se sim, envie o **ID** do profissional; " +
                            $"senão, escolha um papel entre: {opts}.",
                            false));
                    }

                case "askRoleOrPro":
                    {
                        var txt = (input.Text ?? string.Empty).Trim();

                        // Se usuário mandou um GUID, valida profissional existente
                        if (Guid.TryParse(txt, out var proId))
                        {
                            var member = await _companyMembers.GetAsync(session.CompanyId, proId, ct);
                            if (member is null || !member.IsActive)
                            {
                                var roles = await GetAvailableRolesAsync(session.CompanyId, ct);
                                var opts = roles.Length == 0 ? "Nenhuma função cadastrada" : string.Join(", ", roles);
                                await _sessions.SetAsync(session, ct);
                                return Ok(new ChatOut(
                                    $"Não encontrei esse profissional. Envie o ID de um profissional válido " +
                                    $"ou escolha um papel entre: {opts}.",
                                    false));
                            }

                            session.ProfessionalUserId = proId;
                            session.RoleName = null;
                            session.Stage = "askWeek";
                            await _sessions.SetAsync(session, ct);
                            return Ok(new ChatOut("Para quando? Envie a data inicial da semana em UTC (ex.: 2025-08-18).", false));
                        }

                        // Caso contrário, tenta casar com um papel existente
                        var rolesAvail = await GetAvailableRolesAsync(session.CompanyId, ct);
                        var normalized = Norm(txt);
                        var match = rolesAvail.FirstOrDefault(r => Norm(r) == normalized);

                        if (match is null)
                        {
                            var opts = rolesAvail.Length == 0 ? "Nenhuma função cadastrada" : string.Join(", ", rolesAvail);
                            await _sessions.SetAsync(session, ct);
                            return Ok(new ChatOut(
                                $"Nenhum profissional com o papel '{txt}'. Por favor, escolha uma opção válida: {opts}.",
                                false));
                        }

                        session.RoleName = match;
                        session.ProfessionalUserId = null;
                        session.Stage = "askWeek";
                        await _sessions.SetAsync(session, ct);
                        return Ok(new ChatOut("Perfeito. Para quando? Envie a data inicial da semana em UTC (ex.: 2025-08-18).", false));
                    }

                case "askWeek":
                    {
                        if (!DateTime.TryParse(input.Text, out var weekStartUtc))
                            return Ok(new ChatOut("Formato inválido. Ex.: 2025-08-18", false));

                        session.WeekStartUtc = DateTime.SpecifyKind(weekStartUtc, DateTimeKind.Utc);

                        var appt = await _chat.BookForCustomerAsync(
                            session.CompanyId,
                            session.CustomerId!.Value,
                            session.WeekStartUtc.Value,
                            session.ProfessionalUserId,
                            session.RoleName,
                            serviceId: null,
                            ct: ct);

                        await _sessions.ClearAsync(key, ct);

                        return Ok(new ChatOut(
                            $"Prontinho! Agendado: {appt.StartsAtUtc:u}–{appt.EndsAtUtc:u} com status {appt.Status}.",
                            true));
                    }

                default:
                    {
                        await _sessions.ClearAsync(key, ct);
                        return Ok(new ChatOut("Vamos recomeçar? Diga 'oi' 😉", true));
                    }
            }
        }
    }
}