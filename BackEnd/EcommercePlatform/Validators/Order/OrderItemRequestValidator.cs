using Ecommerce.Entities.DTO.Orders;
using FluentValidation;

namespace Ecommerce.API.Validators.Order
{
    public class OrderItemRequestValidator : AbstractValidator<OrderItemRequest>
    {
        public OrderItemRequestValidator() {
            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("ProductId is required.");
            RuleFor(x => x.Quantity)
                .NotEmpty().WithMessage("Quantity is required.")
                .GreaterThan(0).WithMessage("Quantity must be greater than 0.");

        }
    }
}
