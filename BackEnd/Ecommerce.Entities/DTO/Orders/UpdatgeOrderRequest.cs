using Ecommerce.Utilities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Entities.DTO.Orders
{
    public class UpdateOrderRequest
    {
        public OrderStatus Status { get; set; }  // "Pending", "Shipped", "Delivered", "Cancelled"
    }
}
