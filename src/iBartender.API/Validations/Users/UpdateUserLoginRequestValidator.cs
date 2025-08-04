using FluentValidation;
using iBartender.API.Contracts.Users;

namespace iBartender.API.Validations.Users
{
    public class UpdateUserLoginRequestValidator : AbstractValidator<UpdateUserLoginRequest>
    {
        private readonly int _loginMinLength = 3;
        private readonly int _loginMaxLength = 32;

        public UpdateUserLoginRequestValidator()
        {
            RuleFor(x => x.NewLogin)
                .NotEmpty()
                .Length(_loginMinLength, _loginMaxLength)
                .WithMessage($"Login must be between {_loginMinLength} and {_loginMaxLength} in length.")
                .Matches(@"^[a-zA-Z0-9_]+$")
                .WithMessage("Login must contain symbols a-z, A-Z, 0-9, _");
        }
    }
}
