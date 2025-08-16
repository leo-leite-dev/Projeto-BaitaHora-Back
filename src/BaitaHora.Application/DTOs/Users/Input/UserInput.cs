namespace BaitaHora.Application.DTOs.Commands.Users
{
    public sealed record UserInput(
        string Email,
        string Username,
        string Password,
        UserProfileInput Profile
    );
}