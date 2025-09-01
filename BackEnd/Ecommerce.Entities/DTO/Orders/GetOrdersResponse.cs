using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Entities.DTO.Orders
{
    public class GetOrdersResponse
    {
        public List<OrderSummaryDto> Orders { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public bool IsEmpty => Orders.Count == 0;
    }
}
