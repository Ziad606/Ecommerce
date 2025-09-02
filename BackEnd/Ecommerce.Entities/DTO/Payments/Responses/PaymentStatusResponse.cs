using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Entities.DTO.Payments.Responses;
public record PaymentStatusResponse(
    bool Success,
    string Status,
    Guid? OrderId
);
