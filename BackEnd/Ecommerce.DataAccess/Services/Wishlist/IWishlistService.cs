using Ecommerce.Entities.DTO.Wishlist;

using Ecommerce.Entities.Shared.Bases;

namespace Ecommerce.Services.Interfaces
{
	public interface IWishlistService
	{
		Task<Response<AddWishlistItemResponse>> AddItemToWishlistAsync(
			AddWishlistItemReq dto,
			string buyerId,
			CancellationToken cancellationToken = default);

		Task<Response<GetWishlistResponse>> GetWishlistAsync(
		 string buyerId,
		 CancellationToken cancellationToken = default);
		Task<Response<bool>> RemoveItemFromWishlistAsync(
	  string buyerId,
	  Guid wishlistItemId,
	  CancellationToken cancellationToken = default);
	}
}