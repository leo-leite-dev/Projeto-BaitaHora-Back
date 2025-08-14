
using BaitaHora.Domain.Entities;

namespace BaitaHora.Domain.Factories
{
    public static class UserFactory
    {
        public static User Create(
              string email,
            string rawPassword,
            UserProfile profile,
            string? username,
            Func<string, string> hashFunction)
        {
            var user = new User(
                email: email,
                profile: profile,
                username: username
            );

            user.SetPassword(rawPassword, hashFunction);
            return user;
        }
    }
}