using FluentValidation;
using iBartender.API.Contracts.Users;

namespace iBartender.API.Validations.Users
{
    public class UpdateUserBioRequestValidator : AbstractValidator<UpdateUserBioRequest>
    {
        private readonly int _bioMaxLength = 250;
        public UpdateUserBioRequestValidator()
        {
            RuleFor(r => r.NewBio)
                .MaximumLength(_bioMaxLength)
                .WithMessage($"Bio length must be {_bioMaxLength} characters or lower.");
        }
    }
}
