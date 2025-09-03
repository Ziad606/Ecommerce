using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Entities.DTO.Wishlist
{
	// Response DTOs
	public class AddWishlistItemResponse
	{
		public Guid Id { get; set; }
		public Guid ProductId { get; set; }
		public string ProductName { get; set; }
		public DateTime AddedAt { get; set; }
	}
}
