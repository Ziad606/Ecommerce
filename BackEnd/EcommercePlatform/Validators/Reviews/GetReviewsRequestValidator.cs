using Ecommerce.Entities.DTO.Reviews;
using FluentValidation;

namespace Ecommerce.API.Validators.Reviews
{
    public class GetReviewsRequestValidator : AbstractValidator<GetReviewsRequest>
    {
        public GetReviewsRequestValidator() 
        { 
            RuleFor(x => x.OrderId)
                .NotEmpty()
                .WithMessage("OrderId is required.");
            RuleFor(x => x.ProductId)
                .NotEmpty()
                .WithMessage("ProductId is required.");
        }
    }
}
