using Ecommerce.Entities.DTO.Shared;
using Ecommerce.Utilities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Entities.DTO.Orders
{
    public class GetOrdersRequest : RequestFilters<OrderStatus>
    {
        public OrderStatus? Status { get; set; }      // e.g., "Pending", "Shipped"


    }
}
