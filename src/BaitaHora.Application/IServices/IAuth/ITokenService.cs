
using System.Security.Claims;
using BaitaHora.Domain.Entities;

namespace BaitaHora.Application.IServices.Auth
{
    public interface ITokenService
    {
        string GenerateToken(User user, TimeSpan? expiration = null);
        ClaimsPrincipal? ValidateToken(string token);
    }
}