using iBartender.Core.Models;

namespace iBartender.Persistence.Repositories
{
    public interface IPublicationsRepository
    {
        Task Create(Publication newPublication);
        Task CreateComment(Comment newComment);
        Task CreateLike(Guid userId, Guid publicationId);
        Task Delete(Guid id);
        Task DeleteComment(Guid id);
        Task DeleteLike(Guid userId, Guid publicationId);
        Task<Publication> Get(Guid id);
        Task<List<Publication>> GetByUserId(Guid userId);
        Task<Comment> GetComment(Guid id);
        Task<List<Comment>> GetComments(Guid publicationId);
        Task<bool> GetLike(Guid publicationId, Guid userId);
        Task<int> GetLikesCount(Guid publicationId);
        Task<List<Publication>> GetMany(IEnumerable<Guid> ids);
        Task<List<Publication>> GetSubscribed(Guid userId);
        Task Update(Publication updatePublication);
    }
}