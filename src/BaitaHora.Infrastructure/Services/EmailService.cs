using BaitaHora.Configurations.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using BaitaHora.Application.IServices.Auth;

namespace BaitaHora.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailSettings> options, ILogger<EmailService> logger)
        {
            _settings = options.Value;
            _logger = logger;
        }

        public async Task SendAsync(string toEmail, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(_settings.From));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = subject;
            message.Body = new TextPart("html") { Text = body };

            using var smtp = new SmtpClient();

            try
            {
                _logger.LogInformation("Conectando ao servidor SMTP {Host}:{Port}...", _settings.SmtpServer, _settings.Port);
                await smtp.ConnectAsync(_settings.SmtpServer, _settings.Port, SecureSocketOptions.StartTls);

                _logger.LogInformation("Autenticando como {Username}...", _settings.Username);
                await smtp.AuthenticateAsync(_settings.Username, _settings.Password);

                _logger.LogInformation("Enviando e-mail para {ToEmail}...", toEmail);
                await smtp.SendAsync(message);

                await smtp.DisconnectAsync(true);
                _logger.LogInformation("E-mail enviado com sucesso.");
            }
            catch (AuthenticationException ex)
            {
                _logger.LogError(ex, "Falha na autenticação SMTP. Verifique o e-mail e senha de aplicativo.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao enviar e-mail.");
                throw;
            }
        }
    }
}