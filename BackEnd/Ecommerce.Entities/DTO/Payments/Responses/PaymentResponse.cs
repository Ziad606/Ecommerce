using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Entities.DTO.Payments.Responses;
public record PaymentResponse(
     bool Success ,
     Guid? PaymentId ,
     Guid? OrderId,
     string ClientSecret ,
     decimal Amount,
    decimal? OriginalAmount = null,
    decimal? DiscountAmount = null,
    string? PromoCodeUsed = null
);
