using Ecommerce.Entities.DTO.CartDTOs;

namespace Ecommerce.Services.Interfaces
{
	public interface ICartService
	{
		Task<AddCartResponse?> AddItemToCartAsync(AddCartReq dto, string buyerId);
	}
}