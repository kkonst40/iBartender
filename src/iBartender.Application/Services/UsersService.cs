using iBartender.Application.Utils;
using iBartender.Core.Models;
using iBartender.Core.Exceptions;
using iBartender.Persistence.Repositories;
using Microsoft.AspNetCore.Http;


namespace iBartender.Application.Services
{
    public class UsersService : IUsersService
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IImageProcessor _imageProcessor;
        private readonly IEmailValidator _credentialsValidator;

        public UsersService(IUsersRepository usersRepository, IPasswordHasher passwordHasher, IImageProcessor imageValidator, IEmailValidator credentialsValidator)
        {
            _usersRepository = usersRepository;
            _passwordHasher = passwordHasher;
            _imageProcessor = imageValidator;
            _credentialsValidator = credentialsValidator;
        }

        public async Task<User?> Get(Guid id)
        {
            return await _usersRepository.Get(id);
        }

        public async Task<List<User>> GetSubscribers(Guid id)
        {
            return await _usersRepository.GetSubscribers(id);
        }

        public async Task<List<User>> GetSubscribtions(Guid id)
        {
            return await _usersRepository.GetSubscribtions(id);
        }

        public async Task Subscribe(Guid userId, Guid subscriberId)
        {
            if (userId == subscriberId)
                throw new InvalidOperationException("User cannot subscribe to himself");

            await _usersRepository.CreateSubscribtion(userId, subscriberId);
        }

        public async Task Unsubscribe(Guid userId, Guid subscriberId)
        {
            await _usersRepository.DeleteSubscribtion(userId, subscriberId);
        }

        public async Task<User> Create(string login, string email, string password)
        {
            if (!_credentialsValidator.Validate(email))
                throw new InvalidCredentialsException("Email does not exist.");

            var newId = Guid.CreateVersion7();
            var newTokenId = Guid.NewGuid();
            var passwordHash = _passwordHasher.Generate(password);

            var newUser = new User
            {
                Id = newId,
                Login = login,
                Email = email,
                PasswordHash = passwordHash,
                Photo = "",
                Bio = "",
                TokenId = newTokenId
            };

            await _usersRepository.Create(newUser);
            return newUser;
        }

        public async Task<User?> UpdateLogin(Guid id, string newLogin)
        {
            var user = await _usersRepository.Get(id);
            if (user == null)
                return null;

            var updatedUser = new User
            {
                Id = id,
                Login = newLogin,
                Email = user.Email,
                PasswordHash = user.PasswordHash,
                Photo = user.Photo,
                Bio = user.Bio,
                TokenId = user.TokenId,
            };

            await _usersRepository.Update(updatedUser);
            return updatedUser;
        }

        public async Task<User?> UpdatePhoto(Guid id, IFormFile file, string path)
        {
            var user = await _usersRepository.Get(id);

            if (user == null)
                return null;

            var oldPhotoPath = Path.Combine(path, user.Photo);

            if (!_imageProcessor.Validate(file))
                throw new InvalidFileException("File is not valid");

            var newName = Guid.NewGuid().ToString() + ".png";
            var newPath = Path.Combine(path, "Users", newName);
            var photoBytes = _imageProcessor.ProcessProfilePhoto(file, 800);
            _imageProcessor.SaveAsPng(photoBytes, newPath);

            var updatedUser = new User
            {
                Id = id,
                Login = user.Login,
                Email = user.Email,
                PasswordHash = user.PasswordHash,
                Photo = Path.Combine("Users", newName),
                Bio = user.Bio,
                TokenId = user.TokenId,
            };

            await _usersRepository.Update(updatedUser);

            if (File.Exists(oldPhotoPath))
                File.Delete(oldPhotoPath);

            return updatedUser;
        }

        public async Task<User?> UpdatePassword(Guid id, string newPassword, string confirmNewPassword, string oldPassword)
        {
            var user = await _usersRepository.Get(id);

            if (user == null)
                return null;

            if (!_passwordHasher.Verify(oldPassword, user.PasswordHash))
                throw new InvalidPasswordException("Old password is incorrect.");

            var newPasswordHash = _passwordHasher.Generate(newPassword);

            var updatedUser = new User
            {
                Id = id,
                Login = user.Login,
                Email = user.Email,
                PasswordHash = newPasswordHash,
                Photo = user.Photo,
                Bio = user.Bio,
                TokenId = user.TokenId,
            };

            await _usersRepository.Update(updatedUser);

            return updatedUser;
        }

        public async Task<User?> UpdateBio(Guid id, string newBio)
        {
            var user = await _usersRepository.Get(id);

            if (user == null)
                return null;

            var updatedUser = new User
            {
                Id = id,
                Login = user.Login,
                Email = user.Email,
                PasswordHash = user.PasswordHash,
                Photo = user.Photo,
                Bio = newBio,
                TokenId = user.TokenId,
            };

            await _usersRepository.Update(updatedUser);

            return updatedUser;
        }

        public async Task Delete(Guid id)
        {
            await _usersRepository.Delete(id);
        }
    }
}
