using System.Text.Json;

namespace BaitaHora.Api.Authorization
{
    public interface ICompanyContextResolver
    {
        Task<Guid?> ResolveCompanyIdAsync(HttpContext httpContext);
    }

    public sealed class CompanyContextResolver : ICompanyContextResolver
    {
        private const string CompanyIdHeader = "X-Company-Id";

        public async Task<Guid?> ResolveCompanyIdAsync(HttpContext ctx)
        {
            if (ctx.Request.RouteValues.TryGetValue("companyId", out var rv) &&
                Guid.TryParse(rv?.ToString(), out var fromRoute))
                return fromRoute;

            if (ctx.Request.Query.TryGetValue("companyId", out var q) &&
                Guid.TryParse(q.ToString(), out var fromQuery))
                return fromQuery;

            if (ctx.Request.Headers.TryGetValue(CompanyIdHeader, out var h) &&
                Guid.TryParse(h.ToString(), out var fromHeader))
                return fromHeader;

            if (ctx.Request.ContentLength is > 0 &&
                ctx.Request.ContentType?.Contains("application/json", StringComparison.OrdinalIgnoreCase) == true)
            {
                ctx.Request.EnableBuffering();

                using var ms = new MemoryStream();
                await ctx.Request.Body.CopyToAsync(ms);
                ms.Position = 0;
                ctx.Request.Body.Position = 0; 

                try
                {
                    using var doc = JsonDocument.Parse(ms);
                    if (doc.RootElement.TryGetProperty("companyId", out var prop) &&
                        prop.ValueKind == JsonValueKind.String &&
                        Guid.TryParse(prop.GetString(), out var fromBody))
                        return fromBody;
                }
                catch { /* ignora parse */ }
            }

            return null;
        }
    }
}