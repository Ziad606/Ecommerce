
using Ecommerce.Entities.DTO.Cart;
using Ecommerce.Entities.DTO.CartDTOs;

namespace Ecommerce.Services.Interfaces
{
	public interface ICartService
	{
		Task<AddCartResponse?> AddItemToCartAsync(AddCartReq dto, string buyerId);
		Task<GetCartResponse?> GetCartAsync(string buyerId);
		Task<UpdateCartResponse> UpdateCartItemQuantityAsync( string buyerId, Guid cartItemId, int quantity, CancellationToken ct = default);
	}
}