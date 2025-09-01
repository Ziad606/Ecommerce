using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Entities.DTO.Orders
{
    public class CreateOrderRequest
    {
        public string BuyerId { get; set; }
        public List<OrderItemDto> OrderItems { get; set; } 
        public string ShippingCity { get; set; }
        public string ShippingState { get; set; }
        public string ShippingCountry { get; set; }
        public int ShippingZipCode { get; set; }
    }
}
