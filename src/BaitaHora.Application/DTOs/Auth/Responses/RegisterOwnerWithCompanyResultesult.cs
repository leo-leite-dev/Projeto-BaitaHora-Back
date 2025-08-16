namespace BaitaHora.Application.DTOs.Auth.Responses
{
    public sealed record RegisterOwnerWithCompanyResult(
        Guid OwnerUserId,
        Guid CompanyId,
        string Username,
        string CompanyName
    );
}