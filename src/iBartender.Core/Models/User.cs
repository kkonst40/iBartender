

namespace iBartender.Core.Models
{
    public class User : ICloneable
    {
        private User(Guid id, string login, string email, string passwordHash, string bio, string photo, Guid tokenId)
        {
            Id = id;
            Login = login;
            Email = email;
            PasswordHash = passwordHash;
            Bio = bio;
            Photo = photo;
            TokenId = tokenId;
        }

        public Guid Id { get; }
        public string Login { get; } = string.Empty;
        public string Email { get; } = string.Empty;
        public string PasswordHash { get; } = string.Empty;
        public string Photo { get; } = string.Empty;
        public string Bio { get; } = string.Empty;
        public Guid TokenId { get; }

        public static User Create(Guid id, string login, string email, string passwordHash, string bio, string photo, Guid tokenId)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(id), "User id is empty");
            }

            if (string.IsNullOrEmpty(login))
            {
                throw new ArgumentNullException(nameof(login), "Login is empty");
            }

            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException(nameof(email), "Email is empty or null");
            }

            return new User(id, login, email, passwordHash, bio, photo, tokenId);
        }

        public object Clone()
        {
            return new User(Id, Login, Email, PasswordHash, Bio, Photo, TokenId);
        }
    }
}
