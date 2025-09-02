using System;
using System.Collections.Generic;

namespace Ecommerce.Entities.DTO.CartDTOs
{
	public class GetCartResponse
	{
		public Guid Id { get; set; }
		public List<CartItemDetailsDto> Items { get; set; } = new List<CartItemDetailsDto>();
		public int TotalItems { get; set; }
		public decimal TotalPrice { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime? UpdatedAt { get; set; }
	}

	public class CartItemDetailsDto
	{
		public Guid Id { get; set; }
		public Guid ProductId { get; set; }
		public string ProductName { get; set; }
		public decimal ProductPrice { get; set; }
		public string ProductImage { get; set; }
		public int Quantity { get; set; }
		public decimal Subtotal => ProductPrice * Quantity;
		public string StockStatus { get; set; }
		public int StockQuantity { get; set; }
	}
}
