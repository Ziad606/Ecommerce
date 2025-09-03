using Ecommerce.Entities.DTO.Payments.Requests;
using Ecommerce.Entities.DTO.Payments.Responses;
using Ecommerce.Entities.Shared.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.DataAccess.Services.Payments;
public interface IPaymentService
{
    Task<Response<PaymentResponse>> BuyProductAsync(Guid productId, BuyProductRequest request, CancellationToken cancellationToken = default);
    Task<Response<PaymentResponse>> BuyCartAsync(Guid cartId,BuyCartRequest request, CancellationToken cancellationToken = default);
    Task<Response<PaymentStatusResponse>> ConfirmPaymentAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Response<PaymentStatusResponse>> GetPaymentStatusAsync(Guid id, CancellationToken cancellationToken = default);
    public Task<Response<ValidatePromoResponse>> ValidatePromoCodeAsync(ValidatePromoRequest request, CancellationToken cancellationToken = default);
}
