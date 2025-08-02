using iBartender.Application.Utils;
using iBartender.Core.Exceptions;
using iBartender.Persistence.Repositories;


namespace iBartender.Application.Services
{
    public class AuthService: IAuthService
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtProvider _jwtProvider;

        public AuthService(IUsersRepository usersRepository, IPasswordHasher passwordHasher, IJwtProvider jwtProvider)
        {
            _usersRepository = usersRepository;
            _passwordHasher = passwordHasher;
            _jwtProvider = jwtProvider;
        }

        public async Task<string> Login(string email, string password)
        {
            var user = await _usersRepository.GetByEmail(email);

            if (user == null || !_passwordHasher.Verify(password, user.PasswordHash))
                throw new InvalidCredentialsException("Email or password is incorrect");

            return _jwtProvider.Generate(user);
        }

        public async Task Logout(Guid id)
        {
            var newTokenId = Guid.NewGuid();
            await _usersRepository.UpdateTokenId(id, newTokenId);
        }
    }
}
