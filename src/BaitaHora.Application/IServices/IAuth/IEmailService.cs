namespace BaitaHora.Application.IServices.Auth
{
    public interface IEmailService
    {
        Task SendAsync(string toEmail, string subject, string body);
    }
}