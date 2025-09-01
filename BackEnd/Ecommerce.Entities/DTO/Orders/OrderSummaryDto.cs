using Ecommerce.Utilities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Entities.DTO.Orders
{
    public class OrderSummaryDto
    {
        public Guid OrderId { get; set; }
        public string BuyerName { get; set; }
        public OrderStatus Status { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
