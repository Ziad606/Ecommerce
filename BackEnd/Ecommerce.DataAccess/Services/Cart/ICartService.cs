using Ecommerce.Entities.DTO.Cart;
using Ecommerce.Entities.DTO.CartDTOs;
using Ecommerce.Entities.Shared.Bases;

namespace Ecommerce.Services.Interfaces
{
	public interface ICartService
	{
	
		Task<Response<AddCartResponse>> AddItemToCartAsync(AddCartReq dto, string buyerId, CancellationToken cancellationToken = default);


		Task<Response<GetCartResponse>> GetCartAsync(string buyerId, CancellationToken cancellationToken = default);

		
		Task<Response<UpdateCartResponse>> UpdateCartItemQuantityAsync(string buyerId, Guid cartItemId, int quantity, CancellationToken cancellationToken = default);

		
		Task<Response<bool>> RemoveItemFromCartAsync(string buyerId, Guid cartItemId, CancellationToken cancellationToken = default);

		
		Task<Response<bool>> ClearCartAsync(string buyerId, CancellationToken cancellationToken = default);
	}
}