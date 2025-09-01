using Ecommerce.Entities.DTO.Cart;

using FluentValidation;

namespace Ecommerce.Business.Validators.Cart
{
	public class UpdateCartItemValidator : AbstractValidator<UpdateCartRequest>
	{
		public UpdateCartItemValidator()
		{
			RuleFor(x => x.Quantity)
			   .NotNull().WithMessage("Quantity is required.")
			   .GreaterThan(0).WithMessage("Quantity must be greater than 0.")
			   .LessThanOrEqualTo(1000).WithMessage("Quantity is too large."); // عدّل الحد لو حابب
		}
	}
}