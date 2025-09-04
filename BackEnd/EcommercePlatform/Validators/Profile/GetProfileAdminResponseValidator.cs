using Ecommerce.Entities.DTO.Profile;
using FluentValidation;

namespace Ecommerce.API.Validators.Profile;

public class GetProfileAdminResponseValidator : AbstractValidator<GetProfileAdminResponse>
{
    public GetProfileAdminResponseValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(100).WithMessage("First name cannot exceed 100 characters.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(100).WithMessage("Last name cannot exceed 100 characters.");

        RuleFor(x => x.Roles)
            .NotNull().WithMessage("Roles list must not be null.");

        RuleFor(x => x.OrdersCount)
            .GreaterThanOrEqualTo(0).WithMessage("Orders count cannot be negative.");

        RuleFor(x => x.ReviewsCount)
            .GreaterThanOrEqualTo(0).WithMessage("Reviews count cannot be negative.");

        RuleFor(x => x.WishlistCount)
            .GreaterThanOrEqualTo(0).WithMessage("Wishlist count cannot be negative.");

        RuleFor(x => x.CartItemsCount)
            .GreaterThanOrEqualTo(0).WithMessage("Cart items count cannot be negative.");
    }
}