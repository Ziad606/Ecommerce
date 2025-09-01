
using System;

namespace Ecommerce.Entities.DTO.CartDTOs
{
	public class AddCartResponse
	{
		public Guid Id { get; set; }
		public Guid ProductId { get; set; }
		public string ProductName { get; set; }
		public decimal ProductPrice { get; set; }
		public int Quantity { get; set; }
		public decimal Subtotal => ProductPrice * Quantity;
	}
}