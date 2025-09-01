

namespace Ecommerce.Entities.DTO.CartDTOs
{
	public class AddCartReq
	{
		public Guid ProductId { get; set; }
		public int Quantity { get; set; }
	}
}