using Ecommerce.Entities.DTO.Payments.Requests;
using FluentValidation;

namespace Ecommerce.API.Validators.Payments;

public class BuyCartRequestValidator : AbstractValidator<BuyCartRequest>
{
    public BuyCartRequestValidator()
    {
        RuleFor(x => x.CustomerEmail)
            .NotEmpty().WithMessage("Customer email is required.")
            .EmailAddress().WithMessage("Invalid email format.");
    }
}
