using iBartender.Core.Models;
using Microsoft.AspNetCore.Http;

namespace iBartender.Application.Services
{
    public interface IUsersService
    {
        Task<User> Create(string login, string email, string password);
        Task Delete(Guid id);
        Task<User?> Get(Guid id);
        Task<List<User>> GetSubscribers(Guid id);
        Task<List<User>> GetSubscribtions(Guid id);
        Task Subscribe(Guid userId, Guid subscriberId);
        Task Unsubscribe(Guid userId, Guid subscriberId);
        Task<User?> UpdateBio(Guid id, string newBio);
        Task<User?> UpdateLogin(Guid id, string newLogin);
        Task<User?> UpdatePassword(Guid id, string newPassword, string confirmNewPassword, string oldPassword);
        Task<User?> UpdatePhoto(Guid id, IFormFile file, string path);
    }
}