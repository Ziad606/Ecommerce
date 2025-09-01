using Ecommerce.Entities.DTO.CartDTOs;
using FluentValidation;

namespace Ecommerce.API.Validators.CartValidators
{
	public class AddCartValidator : AbstractValidator<AddCartReq>
	{
		public AddCartValidator()
		{
			RuleFor(x => x.ProductId)
				.NotEmpty().WithMessage("ProductId is required.");

			RuleFor(x => x.Quantity)
				.GreaterThan(0).WithMessage("Quantity must be greater than zero.");
		}
	}
}