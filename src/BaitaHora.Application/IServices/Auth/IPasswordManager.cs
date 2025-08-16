namespace BaitaHora.Application.IServices.Auth
{
    public interface IPasswordManager
    {
        string Hash(string plainPassword);
        bool Verify(string rawPassword, string passwordHash);
    }
}