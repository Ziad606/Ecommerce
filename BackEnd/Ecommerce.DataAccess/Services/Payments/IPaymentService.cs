using Ecommerce.Entities.DTO.Payments.Requests;
using Ecommerce.Entities.DTO.Payments.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.DataAccess.Services.Payments;
public interface IPaymentService
{
    Task<PaymentResponse> BuyProductAsync(Guid productId, BuyProductRequest request, CancellationToken cancellationToken = default);
    Task<PaymentResponse> BuyCartAsync(Guid cartId,BuyCartRequest request, CancellationToken cancellationToken = default);
    Task<PaymentStatusResponse> ConfirmPaymentAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PaymentStatusResponse> GetPaymentStatusAsync(Guid id, CancellationToken cancellationToken = default);
}
