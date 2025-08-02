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
        private readonly ICredentialsValidator _credentialsValidator;

        public UsersService(IUsersRepository usersRepository, IPasswordHasher passwordHasher, IImageProcessor imageValidator, ICredentialsValidator credentialsValidator)
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
            if (!_credentialsValidator.ValidateEmail(email))
                throw new InvalidCredentialsException("Email is not valid");

            if (!_credentialsValidator.ValidateLogin(login))
                throw new InvalidCredentialsException("Login is not valid");

            if (!_credentialsValidator.ValidatePassword(password))
                throw new InvalidCredentialsException("Password is not valid");

            var newId = Guid.CreateVersion7();
            var newTokenId = Guid.NewGuid();
            var passwordHash = _passwordHasher.Generate(password);

            var newUser = User.Create(
                newId,
                login,
                email,
                passwordHash,
                "",
                "",
                newTokenId);

            await _usersRepository.Create(newUser);
            return newUser;
        }

        public async Task<User?> UpdateLogin(Guid id, string newLogin)
        {
            var user = await _usersRepository.Get(id);
            if (user == null)
                return null;

            var updatedUser = User.Create(
                id,
                newLogin,
                user.Email,
                user.PasswordHash,
                user.Bio,
                user.Photo,
                user.TokenId);

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

            var updatedUser = User.Create(
                id,
                user.Login,
                user.Email,
                user.PasswordHash,
                user.Bio,
                Path.Combine("Users", newName),
                user.TokenId);

            await _usersRepository.Update(updatedUser);

            if (File.Exists(oldPhotoPath))
                File.Delete(oldPhotoPath);

            return updatedUser;
        }

        public async Task<User?> UpdatePassword(Guid id, string newPassword, string confirmNewPassword, string oldPassword)
        {
            if (string.IsNullOrEmpty(newPassword)
                || string.IsNullOrEmpty(confirmNewPassword)
                || string.IsNullOrEmpty(oldPassword))
                throw new InvalidPasswordException("One of the passwords is empty");

            if (newPassword != confirmNewPassword)
                throw new InvalidPasswordException("New password and confirm password are different");

            var user = await _usersRepository.Get(id);

            if (user == null)
                return null;

            if (!_passwordHasher.Verify(oldPassword, user.PasswordHash))
                throw new InvalidPasswordException("Incorrect current password");

            var newPasswordHash = _passwordHasher.Generate(newPassword);

            var updatedUser = User.Create(
                id,
                user.Login,
                user.Email,
                newPasswordHash,
                user.Bio,
                user.Photo,
                user.TokenId);

            await _usersRepository.Update(updatedUser);

            return updatedUser;
        }

        public async Task Delete(Guid id)
        {
            await _usersRepository.Delete(id);
        }
    }
}
