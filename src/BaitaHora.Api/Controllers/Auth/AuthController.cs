using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BaitaHora.Api.Helpers;
using BaitaHora.Application.DTOs.Requests.Auth;
using BaitaHora.Application.IServices.Auth;
using BaitaHora.Application.IServices.Auths;

namespace BaitaHora.Api.Controllers.Auth
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ICookieService _cookieService;
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            ICookieService cookieService,
            IAuthService authService,
            ILogger<AuthController> logger)
        {
            _cookieService = cookieService ?? throw new ArgumentNullException(nameof(cookieService));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("register-owner")]
        [AllowAnonymous]
        public async Task<IActionResult> BootstrapOwner([FromBody] RegisteOwnerRequest request)
        {
            if (!ModelState.IsValid)
                return ApiResponseHelper.CreateError("Erro de validação", "Dados inválidos.", 400);

            try
            {
                var result = await _authService.RegisterOwnerWithCompanyAsync(request);

                if (!result.IsSuccess)
                    return ApiResponseHelper.CreateError("Falha no cadastro", result.Error ?? "Erro", 400);


                return Ok(ApiResponseHelper.CreateSuccess(result.Value, "Admin e empresa criados com sucesso"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno no bootstrap de owner/empresa.");
                return ApiResponseHelper.CreateError("Erro no servidor", "Tente novamente mais tarde", 500);
            }
        }

        [HttpPost("register-employees")]
        [Authorize(Roles = "Owner,Manager")]
        public async Task<IActionResult> RegisterEmployee([FromBody] RegisterEmployeeRequest request)
        {
            if (!ModelState.IsValid)
                return ApiResponseHelper.CreateError("Erro de validação", "Dados inválidos.", 400);

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out var actorUserId))
                return ApiResponseHelper.CreateError("Não autenticado", "Token inválido", 401);

            try
            {
                var result = await _authService.RegisterEmployeeAsync(request, actorUserId);

                if (!result.IsSuccess)
                {
                    var msg = result.Error ?? "Falha no registro.";
                    var status = msg.Contains("já está em uso", StringComparison.OrdinalIgnoreCase) ? 409 : 400;
                    var title = status == 409 ? "Conflito" : "Falha no registro";
                    return ApiResponseHelper.CreateError(title, msg, status);
                }

                return Ok(ApiResponseHelper.CreateSuccess(result.Value, "Usuário registrado com sucesso"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno ao registrar funcionário.");
                return ApiResponseHelper.CreateError("Erro no servidor", "Tente novamente mais tarde", 500);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _authService.AuthenticateAsync(request);
            if (!result.IsSuccess || string.IsNullOrEmpty(result.Value))
                return Problem(result.Error ?? "Falha na autenticação.");

            _cookieService.SetJwtCookie(Response, result.Value, TimeSpan.FromHours(1));

            return Ok(new
            {
                authenticated = true,
                token = result.Value
            });
        }


        [HttpPost("logout")]
        public IActionResult Logout()
        {
            _cookieService.ClearJwtCookie(Response);
            return NoContent();
        }
    }
}