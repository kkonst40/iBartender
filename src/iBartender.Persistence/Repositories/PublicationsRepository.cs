using iBartender.Core.Models;
using iBartender.Core.Exceptions;
using iBartender.Persistence.Entities;
using iBartender.Persistence.Utils;
using Microsoft.EntityFrameworkCore;


namespace iBartender.Persistence.Repositories
{
    public class PublicationsRepository : IPublicationsRepository
    {
        private readonly BartenderDbContext _bartenderDbContext;

        public PublicationsRepository(BartenderDbContext bartenderDbContext)
        {
            _bartenderDbContext = bartenderDbContext;
        }

        public async Task<Publication?> Get(Guid id)
        {
            var publicationEntity = await _bartenderDbContext.Publications
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (publicationEntity == null)
                return null;

            var publication = new Publication
            {
                Id = publicationEntity.Id,
                UserId = publicationEntity.UserId,
                Text = publicationEntity.Text,
                Files = publicationEntity.Files,
                CreatedAt = publicationEntity.CreatedAt,
                IsEdited = publicationEntity.IsEdited,
            };

            return publication;
        }

        public async Task<List<Publication>> GetMany(IEnumerable<Guid> ids)
        {
            var batchSize = 1000;
            if (ids == null || !ids.Any())
                return new List<Publication>();

            var distinctIds = ids.Distinct().ToList();
            var resultEntities = new List<PublicationEntity>();

            for (int i = 0; i < distinctIds.Count; i += batchSize)
            {
                var currentBatch = distinctIds.Skip(i).Take(batchSize).ToList();

                var batchPublications = await _bartenderDbContext.Publications
                    .AsNoTracking()
                    .Where(u => currentBatch.Contains(u.Id))
                    .ToListAsync();

                resultEntities.AddRange(batchPublications);
            }

            var publications = resultEntities.Select(p => new Publication
                {
                    Id = p.Id,
                    UserId = p.UserId,
                    Text = p.Text,
                    Files = p.Files,
                    CreatedAt = p.CreatedAt,
                    IsEdited = p.IsEdited,
                })
                .ToList();

            return publications;
        }

        public async Task<List<Publication>> GetByUserId(Guid userId)
        {
            var publications = await _bartenderDbContext.Publications
                .AsNoTracking()
                .Where(p => p.UserId == userId)
                .Select(p => new Publication
                {
                    Id = p.Id,
                    UserId = p.UserId,
                    Text = p.Text,
                    Files = p.Files,
                    CreatedAt = p.CreatedAt,
                    IsEdited = p.IsEdited,
                })
                .ToListAsync();

            return publications;
        }

        //publications of users that curerent user subscribed to
        public async Task<List<Publication>> GetSubscribed(Guid userId)
        {
            var subscriptions = _bartenderDbContext.UserSubscribers
                .AsNoTracking()
                .Where(us => us.SubscriberId == userId)
                .Select(us => us.UserId);

            var publications = await _bartenderDbContext.Publications
                .AsNoTracking()
                .Where(p => subscriptions.Contains(p.UserId))
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new Publication
                {
                    Id = p.Id,
                    UserId = p.UserId,
                    Text = p.Text,
                    Files = p.Files,
                    CreatedAt = p.CreatedAt,
                    IsEdited = p.IsEdited,
                })
                .ToListAsync();

            return publications;
        }

        public async Task Create(Publication newPublication)
        {
            var publicationEntity = new PublicationEntity
            {
                Id = newPublication.Id,
                UserId = newPublication.UserId,
                Text = newPublication.Text,
                Files = newPublication.Files,
                CreatedAt = newPublication.CreatedAt,
                IsEdited = newPublication.IsEdited,
            };

            try
            {
                await _bartenderDbContext.Publications.AddAsync(publicationEntity);
                await _bartenderDbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (DbExceptionHelper.IsUniqueConstraintViolation(ex))
            {
                if (await _bartenderDbContext.Publications.AnyAsync(p => p.Id == newPublication.Id))
                    throw new AlreadyExistsException($"Publication {newPublication.Id} already exists.");
            }
        }

        public async Task Update(Publication updatePublication)
        {
            int updatedCount = await _bartenderDbContext.Publications
                .Where(p => p.Id == updatePublication.Id)
                .ExecuteUpdateAsync(u => u
                .SetProperty(p => p.UserId, updatePublication.UserId)
                .SetProperty(p => p.Text, updatePublication.Text)
                .SetProperty(p => p.Files, updatePublication.Files)
                .SetProperty(p => p.CreatedAt, updatePublication.CreatedAt)
                .SetProperty(p => p.IsEdited, updatePublication.IsEdited));

            if (updatedCount == 0)
                throw new Exception($"Publication {updatePublication.Id} does not exist.");
        }

        public async Task Delete(Guid id)
        {
            await _bartenderDbContext.Publications
                .Where(p => p.Id == id)
                .ExecuteDeleteAsync();
        }


        public async Task<List<Comment>> GetComments(Guid publicationId)
        {
            var comments = await _bartenderDbContext.Comments
                .AsNoTracking()
                .Where(c => c.PublicationId == publicationId)
                .OrderBy(c => c.CreatedAt)
                .Select(c => new Comment
                {
                    Id = c.Id,
                    PublicationId = c.PublicationId,
                    UserId = c.UserId,
                    Text = c.Text,
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync();

            return comments;
        }
        
        public async Task<Comment?> GetComment(Guid id)
        {
            var commentEntity = await _bartenderDbContext.Comments
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (commentEntity == null)
                return null;

            var comment = new Comment
            {
                Id = commentEntity.Id,
                PublicationId = commentEntity.PublicationId,
                UserId = commentEntity.UserId,
                Text = commentEntity.Text,
                CreatedAt = commentEntity.CreatedAt
            };

            return comment;
        }

        public async Task CreateComment(Comment newComment)
        {
            var commentEntity = new CommentEntity
            {
                Id = newComment.Id,
                PublicationId = newComment.PublicationId,
                UserId = newComment.UserId,
                Text = newComment.Text,
                CreatedAt = newComment.CreatedAt,
            };

            try
            {
                await _bartenderDbContext.Comments.AddAsync(commentEntity);
                await _bartenderDbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (DbExceptionHelper.IsUniqueConstraintViolation(ex))
            {
                if (!await _bartenderDbContext.Publications.AnyAsync(p => p.Id == newComment.PublicationId))
                    throw new Exception($"Publication {newComment.PublicationId} does not exist.");

                if (!await _bartenderDbContext.Users.AnyAsync(u => u.Id == newComment.UserId))
                    throw new Exception($"User {newComment.UserId} does not exist.");

                if (await _bartenderDbContext.Comments.AnyAsync(c => c.Id == newComment.Id))
                    throw new Exception($"Comment {newComment.Id} already exists.");
            }
        }


        public async Task DeleteComment(Guid id)
        {
            await _bartenderDbContext.Comments
                .Where(p => p.Id == id)
                .ExecuteDeleteAsync();
        }


        public async Task<bool> GetLike(Guid publicationId, Guid userId)
        {
            var likeEntity = await _bartenderDbContext.UserPublications
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return likeEntity != null;
        }

        public async Task<int> GetLikesCount(Guid publicationId)
        {
            var likesCount = await _bartenderDbContext.UserPublications
                .AsNoTracking()
                .CountAsync(up => up.PublicationId == publicationId);

            return likesCount;
        }

        public async Task CreateLike(Guid userId, Guid publicationId)
        {
            var userPublicationEntity = new UserPublicationEntity
            {
                UserId = userId,
                PublicationId = publicationId
            };

            try
            {
                await _bartenderDbContext.UserPublications.AddAsync(userPublicationEntity);
                await _bartenderDbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (DbExceptionHelper.IsUniqueConstraintViolation(ex))
            {
                if (await _bartenderDbContext.UserPublications
                    .AnyAsync(up => up.UserId == userId && up.PublicationId == publicationId))
                    throw new Exception($"Publication {publicationId} is already liked by user {userId}.");

                if (!await _bartenderDbContext.Publications.AnyAsync(p => p.Id == publicationId))
                    throw new Exception($"Publication {publicationId} does not exist.");

                if (!await _bartenderDbContext.Users.AnyAsync(u => u.Id == userId))
                    throw new Exception($"User {userId} does not exist.");
            }
        }

        public async Task DeleteLike(Guid userId, Guid publicationId)
        {
            await _bartenderDbContext.UserPublications
                .Where(up => up.UserId == userId && up.PublicationId == publicationId)
                .ExecuteDeleteAsync();
        }
    }
}
