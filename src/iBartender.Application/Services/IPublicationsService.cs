using iBartender.Core.Models;
using Microsoft.AspNetCore.Http;

namespace iBartender.Application.Services
{
    public interface IPublicationsService
    {
        Task<Comment> CreateComment(Guid publicationId, Guid userId, string text);
        Task CreateLike(Guid publicationId, Guid userId);
        Task<Publication> Create(Guid userId, string text, IFormFileCollection files, string path);
        Task Delete(Guid id, Guid userId);
        Task<Publication?> Get(Guid id);
        Task<List<Comment>> GetComments(Guid publicationId);
        Task<bool> GetLike(Guid publicationId, Guid userId);
        Task<int> GetLikesCount(Guid publicationId);
        Task<List<Publication>> GetPublicationsByUserId(Guid userId);
        Task<List<Publication>> GetSubscribed(Guid userId);
        Task DeleteComment(Guid userId, Guid commentId);
        Task DeleteLike(Guid publicationId, Guid userId);
        Task<Publication?> UpdateFiles(Guid id, IFormFileCollection files, string path, Guid userId);
        Task<Publication?> UpdateText(Guid id, string text, Guid userId);
    }
}