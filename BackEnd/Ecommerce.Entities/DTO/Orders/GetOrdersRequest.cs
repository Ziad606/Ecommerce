using Ecommerce.Utilities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Entities.DTO.Orders
{
    public class GetOrdersRequest
    {
        public OrderStatus? Status { get; set; }      // e.g., "Pending", "Shipped"
        public string? SearchTerm { get; set; }       // Order ID or Buyer Name
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;

    }
}
