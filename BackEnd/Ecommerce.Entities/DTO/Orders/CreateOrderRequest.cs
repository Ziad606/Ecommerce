using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Entities.DTO.Orders
{
    public class CreateOrderRequest
    {

        public string shippingAddress { get; set; }
        List<Guid> ProductIds { get; set; } = new List<Guid>();
    }
}
