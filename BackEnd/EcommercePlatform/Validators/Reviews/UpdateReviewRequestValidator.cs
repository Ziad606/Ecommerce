using Ecommerce.Entities.DTO.Reviews;
using Ecommerce.Utilities.Enums;
using FluentValidation;

namespace Ecommerce.API.Validators.Reviews
{
    public class UpdateReviewRequestValidator : AbstractValidator<UpdateReviewRequest>
    {
        public UpdateReviewRequestValidator()
        {
            RuleFor(x => x.Rating)
                .NotEmpty()
                .IsInEnum();
            RuleFor(x => x.Comment)
                .NotEmpty()
                .MinimumLength(5)
                .MaximumLength(1000);
            RuleFor(x => x.Confirm)
                .NotEmpty();
        }
    }
}
