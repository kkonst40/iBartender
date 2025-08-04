using FluentValidation;
using iBartender.API.Contracts.Publications;

namespace iBartender.API.Validations.Publications
{
    public class CreateCommentRequestValidator : AbstractValidator<CreateCommentRequest>
    {
        private readonly int _textMaxLength = 1000;

        public CreateCommentRequestValidator()
        {
            RuleFor(r => r.Text)
                .NotEmpty()
                .MaximumLength(_textMaxLength)
                .WithMessage($"Publiation text length must be {_textMaxLength} characters or lower.");
        }
    }
}
