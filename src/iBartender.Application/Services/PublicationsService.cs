using iBartender.Core.Models;
using iBartender.Persistence.Repositories;
using iBartender.Application.Utils;
using Microsoft.AspNetCore.Http;
using iBartender.Core.Exceptions;


namespace iBartender.Application.Services
{
    public class PublicationsService : IPublicationsService
    {
        private readonly IPublicationsRepository _publicationsRepository;
        private readonly IImageProcessor _imageProcessor;

        public PublicationsService(IPublicationsRepository publicationsRepository, IImageProcessor imageValidator)
        {
            _publicationsRepository = publicationsRepository;
            _imageProcessor = imageValidator;
        }

        public async Task<Publication?> Get(Guid id)
        {
            return await _publicationsRepository.Get(id);
        }

        public async Task<List<Publication>> GetSubscribed(Guid userId)
        {
            return await _publicationsRepository.GetSubscribed(userId);
        }

        public async Task<bool> GetLike(Guid publicationId, Guid userId)
        {
            return await _publicationsRepository.GetLike(publicationId, userId);
        }

        public async Task<int> GetLikesCount(Guid publicationId)
        {
            return await _publicationsRepository.GetLikesCount(publicationId);
        }

        public async Task CreateLike(Guid publicationId, Guid userId)
        {
            await _publicationsRepository.CreateLike(userId, publicationId);
        }

        public async Task DeleteLike(Guid publicationId, Guid userId)
        {
            await _publicationsRepository.DeleteLike(userId, publicationId);
        }

        public async Task<Publication> Create(Guid userId, string text, IFormFileCollection files, string path)
        {
            List<string> filePaths = [];

            foreach (var file in files)
            {
                if (!_imageProcessor.Validate(file))
                    throw new InvalidFileException("One of the files is not valid");

                var newName = Guid.NewGuid().ToString() + ".png";
                var newPath = Path.Combine(path, "Publications", newName);
                var photoBytes = _imageProcessor.ProcessPublicationPhoto(file);
                _imageProcessor.SaveAsPng(photoBytes, newPath);

                filePaths.Add(Path.Combine("Publications", newName));
            }

            var newId = Guid.NewGuid();

            var newPublication = Publication.Create(
                newId,
                userId,
                text,
                filePaths,
                DateTime.Now.ToUniversalTime(),
                false);

            await _publicationsRepository.Create(newPublication);

            return newPublication;
        }

        public async Task<Publication?> UpdateText(Guid id, string text, Guid userId)
        {
            var publication = await _publicationsRepository.Get(id);

            if (publication == null)
                return null;

            if (publication.UserId != userId)
                throw new UnauthorizedAccessException($"User {userId} is not owner of publication {id}");

            var updatedPublication = Publication.Create(
                id,
                publication.UserId,
                text,
                publication.Files,
                publication.CreatedAt,
                true);

            await _publicationsRepository.Update(updatedPublication);

            return updatedPublication;
        }

        public async Task<Publication?> UpdateFiles(Guid id, IFormFileCollection files, string path, Guid userId)
        {
            var publication = await _publicationsRepository.Get(id);

            if (publication == null)
                return null;

            if (publication.UserId != userId)
                throw new UnauthorizedAccessException($"User {userId} is not owner of publication {id}");


            var oldFilePaths = publication.Files;
            var newFilePaths = new List<string>();

            foreach (var file in files)
            {
                if (!_imageProcessor.Validate(file))
                    throw new InvalidFileException("One of the files is not valid");

                var newName = Guid.NewGuid().ToString() + ".png";
                var newPath = Path.Combine(path, "Publications", newName);
                var photoBytes = _imageProcessor.ProcessPublicationPhoto(file);
                _imageProcessor.SaveAsPng(photoBytes, newPath);

                newFilePaths.Add(Path.Combine("Publications", newName));
            }

            var updatedPublication = Publication.Create(
                id,
                publication.UserId,
                publication.Text,
                newFilePaths,
                publication.CreatedAt,
                true);

            await _publicationsRepository.Update(updatedPublication);

            foreach (var filePath in oldFilePaths)
            {
                if (File.Exists(filePath))
                    File.Delete(filePath);
            }

            return updatedPublication;
        }

        public async Task Delete(Guid id, Guid userId)
        {
            var publications = await _publicationsRepository.GetByUserId(userId);

            if (!publications.Any(p => p.Id == id))
                throw new UnauthorizedAccessException($"User {userId} is not owner of publication {id}");

            await _publicationsRepository.Delete(id);
        }

        public async Task<Comment> CreateComment(Guid publicationId, Guid userId, string text)
        {
            var newCommentId = Guid.NewGuid();
            var newComment = Comment.Create(newCommentId, publicationId, userId, text, DateTimeOffset.UtcNow);

            await _publicationsRepository.CreateComment(newComment);
            return newComment;
        }

        public async Task DeleteComment(Guid userId, Guid commentId)
        {
            var comment = await _publicationsRepository.GetComment(commentId);

            if (comment.UserId != userId)
                throw new UnauthorizedAccessException($"User {userId} is not owner of comment {commentId}");

            await _publicationsRepository.DeleteComment(commentId);
        }

        public async Task<List<Publication>> GetPublicationsByUserId(Guid userId)
        {
            return await _publicationsRepository.GetByUserId(userId);
        }

        public async Task<List<Comment>> GetComments(Guid publicationId)
        {
            return await _publicationsRepository.GetComments(publicationId);
        }
    }
}
