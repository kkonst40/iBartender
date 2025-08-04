using FluentValidation;
using iBartender.API.Contracts.Users;

namespace iBartender.API.Validations.Users
{
    public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
    {
        private readonly int _loginMinLength = 3;
        private readonly int _loginMaxLength = 32;

        private readonly int _passwordMinLength = 8;
        private readonly int _passwordMaxLength = 32;

        public CreateUserRequestValidator()
        {
            RuleFor(x => x.Login)
                .NotEmpty()
                .Length(_loginMinLength, _loginMaxLength)
                .WithMessage($"Login must be between {_loginMinLength} and {_loginMaxLength} in length.")
                .Matches(@"^[a-zA-Z0-9_]+$")
                .WithMessage("Login must contain symbols a-z, A-Z, 0-9, _");

            RuleFor(x => x.Password)
                .NotEmpty()
                .Length(_passwordMinLength, _passwordMaxLength)
                .WithMessage($"Password must be between {_passwordMinLength} and {_passwordMaxLength} in length.")
                .Matches(@"^[a-zA-Z0-9!@#$%^&*()_]+$")
                .WithMessage("Password must contain symbols a-z, A-Z, 0-9, !@#$%^&*()_");

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .WithMessage("Email is invalid.");

        }
    }
}
