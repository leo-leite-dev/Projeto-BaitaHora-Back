namespace BaitaHora.Application.DTOs.Requests.Auth
{
       public sealed class ChangeUsernameRequest
    {
        public Guid UserId { get; set; }
        public string NewUsername { get; set; } = string.Empty;
    }
}