

namespace iBartender.Core.Models
{
    public class User : ICloneable
    {
        public Guid Id { get; init; }
        public string Login { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string PasswordHash { get; init; } = string.Empty;
        public string Photo { get; init; } = string.Empty;
        public string Bio { get; init; } = string.Empty;
        public Guid TokenId { get; init; }

        public object Clone()
        {
            return new User
            {
                Id = this.Id,
                Login = this.Login,
                Email = this.Email,
                PasswordHash = this.PasswordHash,
                Photo = this.Photo,
                Bio = this.Bio,
                TokenId = this.TokenId
            };
        }
    }
}
