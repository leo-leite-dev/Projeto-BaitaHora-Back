namespace BaitaHora.Application.IServices
{
    public interface IPasswordManager
    {
        string Hash(string plainPassword);
        bool Verify(string rawPassword, string passwordHash);
    }
}