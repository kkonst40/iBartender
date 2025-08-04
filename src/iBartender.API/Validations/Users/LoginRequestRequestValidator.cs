using FluentValidation;
using iBartender.API.Contracts.Users;

namespace iBartender.API.Validations.Users
{
    public class LoginRequestRequestValidator : AbstractValidator<LoginUserRequest>
    {

        private readonly int _passwordMinLength = 8;
        private readonly int _passwordMaxLength = 32;

        public LoginRequestRequestValidator() 
        {
            RuleFor(r => r.Email)
                .NotEmpty()
                .EmailAddress()
                .WithMessage("Email or password is invalid.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .Length(_passwordMinLength, _passwordMaxLength)
                .Matches(@"^[a-zA-Z0-9!@#$%^&*()_]+$")
                .WithMessage("Email or password is invalid.");
        }
    }
}
