using Ecommerce.Entities.DTO.Orders;
using FluentValidation;

namespace Ecommerce.API.Validators.Order
{
    public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
    {
        public CreateOrderRequestValidator() { 
        
            RuleFor(x => x.BuyerId)
                .NotEmpty().WithMessage("BuyerId is required.");
            RuleFor(x => x.OrderItems)
                .NotEmpty().WithMessage("At least one order item is required.")
                .NotNull().WithMessage("At least one order item is required.");
            RuleFor(x => x.ShippingCity)
                .NotEmpty().WithMessage("Shipping city is required.")
                .MaximumLength(100).WithMessage("Shipping city must not exceed 100 characters.");
            RuleFor(x => x.ShippingState).
                NotEmpty().WithMessage("Shipping state is required.")
                .MaximumLength(100).WithMessage("Shipping state must not exceed 100 characters.");
            RuleFor(x => x.ShippingCountry)
                .NotEmpty().WithMessage("Shipping country is required.")
                .MaximumLength(100).WithMessage("Shipping country must not exceed 100 characters.");
            RuleFor(x => x.ShippingZipCode)
                .GreaterThan(0).WithMessage("Shipping zip code must be a positive number.").
                LessThan(999999).WithMessage("Shipping zip code must be a valid zip code.");
        }
    }
}
