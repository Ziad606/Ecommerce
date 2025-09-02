using Ecommerce.Entities.DTO.Wishlist;
using FluentValidation;

namespace Ecommerce.API.Validators.Wishlist
{
	public class MoveToCartRequestValidator : AbstractValidator<MoveToCartRequest>
	{
		public MoveToCartRequestValidator()
		{
			RuleFor(x => x.Quantity)
				.NotEmpty()
				.WithMessage("Quantity is required")
				.GreaterThan(0)
				.WithMessage("Quantity must be greater than 0")
				.LessThanOrEqualTo(1000)
				.WithMessage("Quantity cannot exceed 1000 items");
		}
	}
}
