using Ecommerce.Entities.DTO.PromoCodes;
using FluentValidation;

namespace Ecommerce.API.Validators.PromoCodes;

public class CreatePromoCodeRequestValidator : AbstractValidator<CreatePromoCodeRequest>
{
    public CreatePromoCodeRequestValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Promo code is required.")
            .MaximumLength(50).WithMessage("Promo code must not exceed 50 characters.");

        RuleFor(x => x.StartAt)
            .NotEmpty().WithMessage("Start date is required.");

        RuleFor(x => x.EndAt)
            .NotEmpty().WithMessage("End date is required.")
            .GreaterThan(x => x.StartAt).WithMessage("End date must be after Start date.");

        RuleFor(x => x.DiscountPercentage)
            .InclusiveBetween(0, 100).WithMessage("Discount percentage must be between 0 and 100.");
    }
}