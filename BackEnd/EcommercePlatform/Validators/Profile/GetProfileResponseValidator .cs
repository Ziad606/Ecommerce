using Ecommerce.Entities.DTO.Profile;
using FluentValidation;

namespace Ecommerce.API.Validators.Profile;

public class GetProfileResponseValidator : AbstractValidator<GetProfileResponse>
{
    public GetProfileResponseValidator()
    {

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.");

        RuleFor(x => x.OrdersCount)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.ReviewsCount)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.WishlistCount)
            .GreaterThanOrEqualTo(0);
    }
}