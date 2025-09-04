using Ecommerce.Entities.DTO.Reviews;
using FluentValidation;

namespace Ecommerce.API.Validators.Reviews
{
    public class CreateReviewRequestValidator : AbstractValidator<CreateReviewRequest>
    {
        public CreateReviewRequestValidator()
        {
            RuleFor(x => x.OrderId)
                .NotEmpty()
                .WithMessage("OrderId is required.");
            RuleFor(x => x.ProductId)
                .NotEmpty()
                .WithMessage("ProductId is required.");
            RuleFor(x => x.Rating)
                .NotEmpty()
                .IsInEnum()
                .WithMessage("Rating must be a valid enum value.");
            RuleFor(x => x.Comment)
                .NotEmpty()
                .MinimumLength(5)
                .MaximumLength(1000)
                .WithMessage("Comment must be between 5 and 1000 characters.");
        }
    }
}
