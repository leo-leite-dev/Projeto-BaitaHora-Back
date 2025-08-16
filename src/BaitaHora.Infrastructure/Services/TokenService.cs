using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BaitaHora.Application.IServices.Auth;
using BaitaHora.Domain.Entities.Users;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BaitaHora.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;

        public TokenService(IConfiguration config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public string GenerateToken(User user, TimeSpan? expiration = null)
            => GenerateToken(user, expiration, extraClaims: null);

        public string GenerateToken(
            User user,
            TimeSpan? expiration = null,
            IEnumerable<Claim>? extraClaims = null)
        {
            if (user is null) throw new ArgumentNullException(nameof(user));

            var keyStr = _config["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key não configurado.");
            var issuer = _config["Jwt:Issuer"] ?? throw new InvalidOperationException("Jwt:Issuer não configurado.");
            var audience = _config["Jwt:Audience"] ?? throw new InvalidOperationException("Jwt:Audience não configurado.");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, user.Username ?? string.Empty)
            };

            if (extraClaims is not null)
                claims.AddRange(extraClaims);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyStr));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.Add(expiration ?? TimeSpan.FromHours(1)),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public ClaimsPrincipal? ValidateToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) return null;

            try
            {
                var keyStr = _config["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key não configurado.");
                var issuer = _config["Jwt:Issuer"] ?? throw new InvalidOperationException("Jwt:Issuer não configurado.");
                var audience = _config["Jwt:Audience"] ?? throw new InvalidOperationException("Jwt:Audience não configurado.");

                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyStr)),
                    ClockSkew = TimeSpan.Zero,

                    RoleClaimType = ClaimTypes.Role,
                    NameClaimType = ClaimTypes.NameIdentifier
                };

                return tokenHandler.ValidateToken(token, validationParameters, out _);
            }
            catch
            {
                return null;
            }
        }
    }
}