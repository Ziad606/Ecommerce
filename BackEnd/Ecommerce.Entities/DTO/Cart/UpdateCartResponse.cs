using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Entities.DTO.Cart
{
	public class UpdateCartResponse
	{
		public Guid Id { get; set; }          // cart item id
		public int Quantity { get; set; }
		public decimal Subtotal { get; set; }  
	}
}
