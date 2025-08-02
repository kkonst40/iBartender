using iBartender.Core.Models;

namespace iBartender.Persistence.Repositories
{
    public interface IUsersRepository
    {
        Task Create(User newUser);
        Task CreateSubscribtion(Guid userId, Guid subscriberId);
        Task Delete(Guid id);
        Task DeleteSubscribtion(Guid userId, Guid subscriberId);
        Task<User?> Get(Guid id);
        Task<User?> GetByEmail(string email);
        Task<User?> GetByLogin(string login);
        Task<List<User>> GetMany(IEnumerable<Guid> ids);
        Task<List<User>> GetSubscribers(Guid id);
        Task<List<User>> GetSubscribtions(Guid id);
        Task Update(User updateUser);
        Task UpdateTokenId(Guid userId, Guid newTokenId);
    }
}