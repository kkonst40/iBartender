using Microsoft.Extensions.Configuration;

namespace iBartender.Application.Utils
{
    public class PasswordHasher : IPasswordHasher
    {
        public string Generate(string password)
            => BCrypt.Net.BCrypt.EnhancedHashPassword(password);

        public bool Verify(string password, string passwordHash)
            => BCrypt.Net.BCrypt.EnhancedVerify(password, passwordHash);
    }
}
