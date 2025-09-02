using Ecommerce.Entities.Models;
using Ecommerce.Utilities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Entities.DTO.Orders
{
    public class OrderDetailsResponse
    {
        public string OrderId { get; set; }
        public string BuyerName { get; set; }
        public OrderStatus Status { get; set; }
        public decimal TotalPrice { get; set; }
        public string CourierService { get; set; }
        public DateTime OrderDate { get; set; }
        public List<ProductItemDto> Products { get; set; }
        public string ShippingAddress { get; set; }
    }

    public class ProductItemDto
    {
        public string ProductName { get; set; }
        public ProductImage ProdcutImage { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
