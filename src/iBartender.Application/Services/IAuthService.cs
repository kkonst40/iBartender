

namespace iBartender.Application.Services
{
    public interface IAuthService
    {
        Task<string> Login(string email, string password);
        Task Logout(Guid id);
    }
}
