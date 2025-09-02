using Ecommerce.Entities.DTO.Orders;
using FluentValidation;

namespace Ecommerce.API.Validators.Order
{
    public class BuyerCreateOrderValidator : AbstractValidator<BuyerCreateOrderRequest>
    {
        public BuyerCreateOrderValidator()
        {
            RuleFor(x => x.ShippingCity).NotEmpty().MaximumLength(100);
            RuleFor(x => x.ShippingState).NotEmpty().MaximumLength(100);
            RuleFor(x => x.ShippingCountry).NotEmpty().MaximumLength(100);
            RuleFor(x => x.ShippingZipCode).GreaterThan(0).LessThan(999999);
        }
    }
}
