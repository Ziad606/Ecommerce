using Ecommerce.Entities.DTO.Payments.Requests;
using FluentValidation;

namespace Ecommerce.API.Validators.Payments;

public class BuyProductRequestValidator : AbstractValidator<BuyProductRequest>
{
    public BuyProductRequestValidator()
    {
        RuleFor(x => x.Quantity)
           .GreaterThan(0).WithMessage("Quantity must be greater than zero.");

        RuleFor(x => x.CustomerEmail)
            .NotEmpty().WithMessage("Customer email is required.")
            .EmailAddress().WithMessage("Invalid email format.");
    }
}
