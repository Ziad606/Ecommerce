using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Entities.DTO.Wishlist
{
	public class GetWishlistResponse
	{
		public Guid Id { get; set; }
		public List<WishlistItemDetailsDto> Items { get; set; } = new List<WishlistItemDetailsDto>();
		public int TotalItems { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime? UpdatedAt { get; set; }
	}
	public class WishlistItemDetailsDto
	{
		public Guid Id { get; set; }
		public Guid ProductId { get; set; }
		public string ProductName { get; set; }
		public decimal ProductPrice { get; set; }
		public string? ProductImage { get; set; }
		public string StockStatus { get; set; }
		public bool IsActive { get; set; }
		public DateTime AddedAt { get; set; }
	}

}
