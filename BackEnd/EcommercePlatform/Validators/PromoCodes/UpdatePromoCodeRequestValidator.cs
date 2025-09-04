using Ecommerce.Entities.DTO.PromoCodes;
using FluentValidation;

namespace Ecommerce.API.Validators.PromoCodes;

public class UpdatePromoCodeRequestValidator : AbstractValidator<UpdatePromoCodeRequest>
{
    public UpdatePromoCodeRequestValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Promo code is required.");

        When(x => x.StartAt.HasValue && x.EndAt.HasValue, () =>
        {
            RuleFor(x => x.EndAt.Value)
                .GreaterThan(x => x.StartAt.Value).WithMessage("End date must be after Start date.");
        });

        When(x => x.DiscountPercentage.HasValue, () =>
        {
            RuleFor(x => x.DiscountPercentage.Value)
                .InclusiveBetween(0, 100).WithMessage("Discount percentage must be between 0 and 100.");
        });
    }
}