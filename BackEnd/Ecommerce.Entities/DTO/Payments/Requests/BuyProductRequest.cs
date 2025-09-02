using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Entities.DTO.Payments.Requests;
public record BuyProductRequest(
    string CustomerEmail,
    int Quantity
); // Guid ProductId at route 
