using Ecommerce.Entities.DTO.Reviews;
using FluentValidation;

namespace Ecommerce.API.Validators.Reviews
{
    public class DeleteReviewRequestValidator : AbstractValidator<DeleteReviewRequest>

    {
        public DeleteReviewRequestValidator()
        {
            RuleFor(x => x.Confirm)
                .NotEmpty()
                .WithMessage("Confirmation is required to delete a review.")
                .Equal(true)
                .WithMessage("You must confirm the deletion of the review.");
        }
    }
}
