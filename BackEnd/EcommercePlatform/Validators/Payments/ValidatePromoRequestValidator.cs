using Ecommerce.Entities.DTO.Payments.Requests;
using FluentValidation;

namespace Ecommerce.API.Validators.Payments;

public class ValidatePromoRequestValidator : AbstractValidator<ValidatePromoRequest>
{
    public ValidatePromoRequestValidator()
    {
        RuleFor(x => x.PromoCode)
         .NotEmpty().WithMessage("Promo code is required.")
         .Length(3, 50).WithMessage("Promo code must be between 3 and 50 characters.");
    }
}
