using BaitaHora.Application.IServices.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace BaitaHora.Infrastructure.Services
{
    public class CookieService : ICookieService
    {
        private readonly bool _isProduction;

        public CookieService(IHostEnvironment env)
        {
            _isProduction = env.IsProduction();
        }

        public void SetJwtCookie(HttpResponse response, string token, TimeSpan expiration)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTimeOffset.UtcNow.Add(expiration)
            };

            if (_isProduction)
            {
                cookieOptions.Secure = true;
                cookieOptions.SameSite = SameSiteMode.None;
            }
            else
            {
                cookieOptions.Secure = false;
                cookieOptions.SameSite = SameSiteMode.Lax;
            }

            response.Cookies.Append("jwtToken", token, cookieOptions);
        }

        public void ClearJwtCookie(HttpResponse response)
        {
            response.Cookies.Delete("jwtToken");
        }
    }
}