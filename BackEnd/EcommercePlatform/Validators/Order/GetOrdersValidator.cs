using Ecommerce.Entities.DTO.Orders;
using Ecommerce.Utilities.Enums;
using FluentValidation;

namespace Ecommerce.API.Validators.Order
{
    public class GetOrdersValidator : AbstractValidator<GetOrdersRequest>
    {
        public GetOrdersValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThan(0).WithMessage("Page number must be greater than 0.");
            RuleFor(x => x.PageSize)
                .GreaterThan(0).WithMessage("Page size must be greater than 0.")
                .LessThanOrEqualTo(100).WithMessage("Page size must be less than or equal to 100.");
        }
    }
}
