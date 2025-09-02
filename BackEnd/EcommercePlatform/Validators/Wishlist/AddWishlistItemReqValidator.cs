using Ecommerce.Entities.DTO.Wishlist;
using FluentValidation;

namespace Ecommerce.API.Validators.Wishlist
{
	public class AddWishlistItemReqValidator : AbstractValidator<AddWishlistItemReq>
	{
		public AddWishlistItemReqValidator()
		{
			RuleFor(x => x.ProductId)
				.NotEmpty()
				.WithMessage("Product ID is required")
				.NotEqual(Guid.Empty)
				.WithMessage("Product ID must be a valid GUID");
		}
	}
}
