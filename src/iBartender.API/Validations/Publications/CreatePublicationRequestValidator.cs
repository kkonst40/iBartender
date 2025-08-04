using FluentValidation;
using iBartender.API.Contracts.Publications;

namespace iBartender.API.Validations.Publications
{
    public class CreatePublicationRequestValidator : AbstractValidator<CreatePublicationRequest>
    {
        private readonly int _textMaxLength = 5000;

        public CreatePublicationRequestValidator() 
        {
            RuleFor(r => r.Text)
                .NotEmpty()
                .MaximumLength(_textMaxLength)
                .WithMessage($"Publiation text length must be {_textMaxLength} characters or lower.");
        }
    }
}
