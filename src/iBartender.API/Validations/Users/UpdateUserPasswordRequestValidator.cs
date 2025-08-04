using FluentValidation;
using iBartender.API.Contracts.Users;

namespace iBartender.API.Validations.Users
{
    public class UpdateUserPasswordRequestValidator : AbstractValidator<UpdateUserPasswordRequest>
    {
        private readonly int _passwordMinLength = 8;
        private readonly int _passwordMaxLength = 32;

        public UpdateUserPasswordRequestValidator()
        {
            RuleFor(x => x.OldPassword)
                .NotEmpty()
                .Length(_passwordMinLength, _passwordMaxLength)
                .Matches(@"^[a-zA-Z0-9!@#$%^&*()_]+$")
                .WithMessage("Old password is incorrect.");

            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .Length(_passwordMinLength, _passwordMaxLength)
                .WithMessage($"New password must be between {_passwordMinLength} and {_passwordMaxLength} in length.")
                .Matches(@"^[a-zA-Z0-9!@#$%^&*()_]+$")
                .WithMessage("New password must contain symbols a-z, A-Z, 0-9, !@#$%^&*()_")
                .NotEqual(x => x.OldPassword)
                .WithMessage("New password must differ from the old one.");

            RuleFor(x => x.NewPasswordConfirm)
                .NotEmpty()
                .Equal(x => x.NewPassword)
                .WithMessage("Confirmation password differs from new password.");
        }
    }
}
