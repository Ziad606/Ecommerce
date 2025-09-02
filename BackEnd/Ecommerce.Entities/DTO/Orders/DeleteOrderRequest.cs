using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Entities.DTO.Orders
{
    public class DeleteOrderRequest
    {
        public bool Confirm { get; set; }  // Optional: client confirms deletion
    }
}
