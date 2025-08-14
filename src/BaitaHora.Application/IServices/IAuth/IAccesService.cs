namespace BaitaHora.Application.IServices.Auths
{
    public interface IAccessService
    {
        Task<bool> CanCreateUsersAsync(Guid actorUserId, Guid companyId);
    }
}