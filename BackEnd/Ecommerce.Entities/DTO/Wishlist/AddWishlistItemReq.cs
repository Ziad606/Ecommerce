using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Entities.DTO.Wishlist
{
	public class AddWishlistItemReq
	{
		[Required(ErrorMessage = "Product ID is required")]
		public Guid ProductId { get; set; }
	}

}
