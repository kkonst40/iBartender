using iBartender.Core.Models;
using iBartender.Core.Exceptions;
using iBartender.Persistence.Entities;
using iBartender.Persistence.Utils;
using Microsoft.EntityFrameworkCore;


namespace iBartender.Persistence.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        private readonly BartenderDbContext _bartenderDbContext;

        public UsersRepository(BartenderDbContext bartenderDbContext)
        {
            _bartenderDbContext = bartenderDbContext;
        }

        public async Task<User?> Get(Guid id)
        {
            var userEntity = await _bartenderDbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id);

            if (userEntity == null)
                return null;

            var user = new User
            {
                Id = userEntity.Id,
                Login = userEntity.Login,
                Email = userEntity.Email,
                PasswordHash = userEntity.PasswordHash,
                Photo = userEntity.Photo,
                Bio = userEntity.Bio,
                TokenId = userEntity.TokenId
            };

            return user;
        }

        public async Task<User?> GetByLogin(string login)
        {
            var userEntity = await _bartenderDbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Login == login);

            if (userEntity == null)
                return null;

            var user = new User
            {
                Id = userEntity.Id,
                Login = userEntity.Login,
                Email = userEntity.Email,
                PasswordHash = userEntity.PasswordHash,
                Photo = userEntity.Photo,
                Bio = userEntity.Bio,
                TokenId = userEntity.TokenId
            };

            return user;
        }

        public async Task<User?> GetByEmail(string email)
        {
            var userEntity = await _bartenderDbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email);

            if (userEntity == null)
                return null;

            var user = new User
            {
                Id = userEntity.Id,
                Login = userEntity.Login,
                Email = userEntity.Email,
                PasswordHash = userEntity.PasswordHash,
                Photo = userEntity.Photo,
                Bio = userEntity.Bio,
                TokenId = userEntity.TokenId
            };

            return user;
        }

        public async Task<List<User>> GetMany(IEnumerable<Guid> ids)
        {
            var batchSize = 1000;
            if (ids == null || !ids.Any())
                return new List<User>();

            var distinctIds = ids.Distinct().ToList();
            var resultEntities = new List<UserEntity>();

            for (int i = 0; i < distinctIds.Count; i += batchSize)
            {
                var currentBatch = distinctIds.Skip(i).Take(batchSize).ToList();

                var batchUsers = await _bartenderDbContext.Users
                    .AsNoTracking()
                    .Where(u => currentBatch.Contains(u.Id))
                    .AsNoTracking()
                    .ToListAsync();

                resultEntities.AddRange(batchUsers);
            }

            return resultEntities.Select(u => new User
                {
                    Id = u.Id,
                    Login = u.Login,
                    Email = u.Email,
                    PasswordHash = u.PasswordHash,
                    Photo = u.Photo,
                    Bio = u.Bio,
                    TokenId = u.TokenId
                })
                .ToList();
        }

        public async Task Create(User newUser)
        {
            var userEntity = new UserEntity
            {
                Id = newUser.Id,
                Login = newUser.Login,
                Email = newUser.Email,
                PasswordHash = newUser.PasswordHash,
                Bio = newUser.Bio,
                Photo = newUser.Photo,
                TokenId = newUser.TokenId
            };

            try
            {
                await _bartenderDbContext.Users.AddAsync(userEntity);
                await _bartenderDbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (DbExceptionHelper.IsUniqueConstraintViolation(ex))
            {
                if (await _bartenderDbContext.Users.AnyAsync(u => u.Login == newUser.Login))
                    throw new AlreadyExistsException($"User with login {newUser.Login} already exists.");

                if (await _bartenderDbContext.Users.AnyAsync(u => u.Email == newUser.Email))
                    throw new AlreadyExistsException($"User with email {newUser.Email} already exists.");

                if (await _bartenderDbContext.Users.AnyAsync(u => u.Id == newUser.Id))
                    throw new AlreadyExistsException($"User {newUser.Id} already exists.");
            }
        }

        public async Task Update(User updateUser)
        {
            int updatedCount = await _bartenderDbContext.Users
                .Where(u => u.Id == updateUser.Id)
                .ExecuteUpdateAsync(u => u
                .SetProperty(u => u.Login, updateUser.Login)
                .SetProperty(u => u.Email, updateUser.Email)
                .SetProperty(u => u.PasswordHash, updateUser.PasswordHash)
                .SetProperty(u => u.Bio, updateUser.Bio)
                .SetProperty(u => u.Photo, updateUser.Photo)
                .SetProperty(u => u.TokenId, updateUser.TokenId));

            if (updatedCount == 0)
                throw new NotFoundException($"User {updateUser.Id} does not exist.");
        }

        public async Task Delete(Guid id)
        {
            await _bartenderDbContext.Users
                .Where(u => u.Id == id)
                .ExecuteDeleteAsync();
        }


        public async Task CreateSubscribtion(Guid userId, Guid subscriberId)
        {
            var newSubscription = new UserSubscriberEntity
            {
                UserId = userId,
                SubscriberId = subscriberId
            };

            try
            {
                await _bartenderDbContext.UserSubscribers.AddAsync(newSubscription);
                await _bartenderDbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (DbExceptionHelper.IsUniqueConstraintViolation(ex))
            {
                if (await _bartenderDbContext.UserSubscribers.AnyAsync(us => us.UserId == userId && us.SubscriberId == subscriberId))
                    throw new AlreadyExistsException($"User {subscriberId} already subscribed to user {userId}");

                if (!await _bartenderDbContext.Users.AnyAsync(u => u.Id == userId))
                    throw new NotFoundException($"User {userId} does not exist");

                if (!await _bartenderDbContext.Users.AnyAsync(u => u.Id == subscriberId))
                    throw new NotFoundException($"User(subscriber) {subscriberId} does not exist");
            }
        }

        public async Task DeleteSubscribtion(Guid userId, Guid subscriberId)
        {
            await _bartenderDbContext.UserSubscribers
                .Where(us => us.UserId == userId && us.SubscriberId == subscriberId)
                .ExecuteDeleteAsync();
        }


        public async Task<List<User>> GetSubscribers(Guid id)
        {
            var user = await _bartenderDbContext.Users
                .AsNoTracking()
                .Include(u => u.Subscribers)
                .ThenInclude(us => us.Subscriber)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return new List<User>();

            var subscribers = user.Subscribers.Select(us => new User
                {
                    Id = us.Subscriber.Id,
                    Login = us.Subscriber.Login,
                    Email = us.Subscriber.Email,
                    PasswordHash = "",
                    Photo = us.Subscriber.Photo,
                    Bio = us.Subscriber.Bio,
                    TokenId = us.Subscriber.TokenId
                 });

            return subscribers.ToList();
        }

        public async Task<List<User>> GetSubscribtions(Guid id)
        {
            var user = await _bartenderDbContext.Users
                .AsNoTracking()
                .Include(u => u.Subscriptions)
                .ThenInclude(us => us.User)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return new List<User>();

            return user.Subscriptions.Select(us => new User
                {
                    Id = us.Subscriber.Id,
                    Login = us.Subscriber.Login,
                    Email = us.Subscriber.Email,
                    PasswordHash = "",
                    Photo = us.Subscriber.Photo,
                    Bio = us.Subscriber.Bio,
                    TokenId = us.Subscriber.TokenId
                })
                .ToList();
        }

        public async Task UpdateTokenId(Guid userId, Guid newTokenId)
        {
            await _bartenderDbContext.Users
                .Where(u => u.Id == userId)
                .ExecuteUpdateAsync(u => u
                .SetProperty(u => u.TokenId, newTokenId));
        }
    }
}
