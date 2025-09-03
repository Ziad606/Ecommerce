using Ecommerce.DataAccess.ApplicationContext;
using Ecommerce.Entities.DTO.Payments.Requests;
using Ecommerce.Entities.DTO.Payments.Responses;
using Ecommerce.Entities.Models;
using Ecommerce.Entities.Shared.Bases;
using Ecommerce.Utilities.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Stripe;

namespace Ecommerce.DataAccess.Services.Payments;

public class PaymentService(PaymentIntentService paymentIntentService,
        AuthContext context,
        ResponseHandler responseHandler,
        ILogger<PaymentService> logger
    ) : IPaymentService
{
    private readonly PaymentIntentService _paymentIntentService = paymentIntentService;
    private readonly AuthContext _context = context;
    private readonly ResponseHandler _responseHandler = responseHandler;
    private readonly ILogger<PaymentService> _logger = logger;

    public async Task<Response<PaymentResponse>> BuyProductAsync(Guid productId, BuyProductRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("BuyProductAsync called for ProductId={ProductId}, CustomerEmail={CustomerEmail}",
                productId, request.CustomerEmail);

            var product = await _context.Products
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id.Equals(productId), cancellationToken);

            if (product is null)
            {
                _logger.LogWarning("Product with ID {ProductId} not found", productId);
                return _responseHandler.NotFound<PaymentResponse>("Product not found");
            }

            if (product.StockQuantity < request.Quantity)
            {
                _logger.LogWarning("Insufficient stock for ProductId={ProductId}. Requested: {RequestedQuantity}, Available: {AvailableQuantity}",
                    productId, request.Quantity, product.StockQuantity);
                return _responseHandler.BadRequest<PaymentResponse>("Insufficient stock quantity");
            }

            var amount = product.Price * request.Quantity;
            var amountInCents = (long)(amount * 100);

            var options = new PaymentIntentCreateOptions
            {
                Amount = amountInCents,
                Currency = "usd",
                Customer = request.CustomerEmail,
                Metadata = new Dictionary<string, string>
                {
                    ["product_id"] = productId.ToString(),
                    ["quantity"] = request.Quantity.ToString(),
                    ["customer_email"] = request.CustomerEmail
                }
            };

            var paymentIntent = await _paymentIntentService.CreateAsync(options, null, cancellationToken);

            var order = new Entities.Models.Order
            {
                BuyerId = request.CustomerEmail,
                Status = OrderStatus.Pending,
                TotalPrice = amount,
                OrderDate = DateTime.UtcNow
            };

            var orderItem = new OrderItem
            {
                ProductId = productId,
                Product = product,
                Quantity = request.Quantity,
                UnitPrice = product.Price,
                OrderId = order.Id
            };

            order.OrderItems.Add(orderItem);

            var payment = new Payment
            {
                Amount = amount,
                Currency = "USD",
                Status = Status.Pending,
                StripePaymentIntentId = paymentIntent.Id,
                CustomerEmail = request.CustomerEmail,
                OrderId = order.Id,
                Order = order
            };

            await _context.Orders.AddAsync(order, cancellationToken);
            await _context.Payments.AddAsync(payment, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Payment created successfully. PaymentId={PaymentId}, OrderId={OrderId}",
                payment.Id, order.Id);

            var paymentResponse = new PaymentResponse(
                true,
                payment.Id,
                order.Id,
                paymentIntent.ClientSecret,
                amount
            );

            return _responseHandler.Success(paymentResponse, "Payment created successfully");
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error while creating payment for ProductId={ProductId}", productId);
            return _responseHandler.InternalServerError<PaymentResponse>("Database error occurred while creating payment");
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Stripe error while creating payment for ProductId={ProductId}", productId);
            return _responseHandler.BadRequest<PaymentResponse>("Payment processing error occurred");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while creating payment for ProductId={ProductId}", productId);
            return _responseHandler.InternalServerError<PaymentResponse>("An unexpected error occurred while creating payment");
        }
    }

    public async Task<Response<PaymentResponse>> BuyCartAsync(Guid cartId, BuyCartRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("BuyCartAsync called for CartId={CartId}, CustomerEmail={CustomerEmail}",
                cartId, request.CustomerEmail);

            var cartItems = await _context.Set<CartItem>()
                .Include(ci => ci.Product)
                .Where(ci => ci.CartId == cartId)
                .ToListAsync(cancellationToken);

            if (!cartItems.Any())
            {
                _logger.LogWarning("Cart with ID {CartId} is empty or not found", cartId);
                return _responseHandler.NotFound<PaymentResponse>("Cart is empty or not found");
            }

            foreach (var item in cartItems)
            {
                if (item.Product.StockQuantity < item.Quantity)
                {
                    _logger.LogWarning("Insufficient stock for ProductId={ProductId} in cart. Requested: {RequestedQuantity}, Available: {AvailableQuantity}",
                        item.ProductId, item.Quantity, item.Product.StockQuantity);
                    return _responseHandler.BadRequest<PaymentResponse>($"Insufficient stock for product: {item.Product.Name}");
                }
            }

            var totalAmount = cartItems.Sum(item => item.Product.Price * item.Quantity);
            var amountInCents = (long)(totalAmount * 100);

            var options = new PaymentIntentCreateOptions
            {
                Amount = amountInCents,
                Currency = "usd",
                Customer = request.CustomerEmail,
                Metadata = new Dictionary<string, string>
                {
                    ["cart_id"] = cartId.ToString(),
                    ["customer_email"] = request.CustomerEmail,
                    ["items_count"] = cartItems.Count.ToString()
                }
            };

            var paymentIntent = await _paymentIntentService.CreateAsync(options, null, cancellationToken);

            var order = new Entities.Models.Order
            {
                BuyerId = request.CustomerEmail,
                Status = OrderStatus.Pending,
                TotalPrice = totalAmount,
                OrderDate = DateTime.UtcNow
            };

            foreach (var cartItem in cartItems)
            {
                var orderItem = new OrderItem
                {
                    ProductId = cartItem.ProductId,
                    Product = cartItem.Product,
                    Quantity = cartItem.Quantity,
                    UnitPrice = cartItem.Product.Price,
                    OrderId = order.Id
                };
                order.OrderItems.Add(orderItem);
            }

            var payment = new Payment
            {
                Amount = totalAmount,
                Currency = "USD",
                Status = Status.Pending,
                StripePaymentIntentId = paymentIntent.Id,
                CustomerEmail = request.CustomerEmail,
                OrderId = order.Id,
                Order = order
            };

            await _context.Orders.AddAsync(order, cancellationToken);
            await _context.Payments.AddAsync(payment, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Payment created successfully for cart. PaymentId={PaymentId}, OrderId={OrderId}",
                payment.Id, order.Id);

            var paymentResponse = new PaymentResponse(
                true,
                payment.Id,
                order.Id,
                paymentIntent.ClientSecret,
                totalAmount
            );

            return _responseHandler.Success(paymentResponse, "Cart payment created successfully");
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error while creating payment for CartId={CartId}", cartId);
            return _responseHandler.InternalServerError<PaymentResponse>("Database error occurred while creating payment");
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Stripe error while creating payment for CartId={CartId}", cartId);
            return _responseHandler.BadRequest<PaymentResponse>("Payment processing error occurred");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while creating payment for CartId={CartId}", cartId);
            return _responseHandler.InternalServerError<PaymentResponse>("An unexpected error occurred while creating payment");
        }
    }

    public async Task<Response<PaymentStatusResponse>> ConfirmPaymentAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("ConfirmPaymentAsync called for PaymentId={PaymentId}", id);

            var payment = await _context.Payments
                .Include(p => p.Order)
                .SingleOrDefaultAsync(p => p.Id.Equals(id), cancellationToken);

            if (payment is null)
            {
                _logger.LogWarning("Payment with ID {PaymentId} not found", id);
                return _responseHandler.NotFound<PaymentStatusResponse>("Payment not found");
            }

            var paymentIntent = await _paymentIntentService.GetAsync(payment.StripePaymentIntentId, null, cancellationToken: cancellationToken);

            var newStatus = paymentIntent.Status switch
            {
                "succeeded" => Status.Completed,
                "canceled" => Status.Cancelled,
                "requires_payment_method" => Status.Failed,
                "payment_failed" => Status.Failed,
                _ => Status.Pending
            };

            payment.Status = newStatus;

            if (newStatus == Status.Completed && payment.Order != null)
            {
                payment.Order.Status = OrderStatus.Confirmed;
                _logger.LogInformation("Payment {PaymentId} completed successfully. Order {OrderId} status updated to Confirmed",
                    payment.Id, payment.Order.Id);
            }
            else if ((newStatus == Status.Cancelled || newStatus == Status.Failed) && payment.Order != null)
            {
                payment.Order.Status = OrderStatus.Cancelled;
                _logger.LogInformation("Payment {PaymentId} failed/cancelled. Order {OrderId} status updated to Cancelled",
                    payment.Id, payment.Order.Id);
            }

            await _context.SaveChangesAsync(cancellationToken);

            var statusResponse = new PaymentStatusResponse(
                newStatus == Status.Completed,
                paymentIntent.Status,
                payment.OrderId
            );

            string message = newStatus switch
            {
                Status.Completed => "Payment confirmed successfully",
                Status.Failed => "Payment confirmation failed",
                Status.Cancelled => "Payment was cancelled",
                _ => "Payment is still processing"
            };

            return newStatus == Status.Completed
                ? _responseHandler.Success(statusResponse, message)
                : _responseHandler.BadRequest<PaymentStatusResponse>(message);
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Stripe error while confirming payment {PaymentId}", id);
            return _responseHandler.BadRequest<PaymentStatusResponse>("Error communicating with payment processor");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while confirming payment {PaymentId}", id);
            return _responseHandler.InternalServerError<PaymentStatusResponse>("An unexpected error occurred while confirming payment");
        }
    }

    public async Task<Response<PaymentStatusResponse>> GetPaymentStatusAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("GetPaymentStatusAsync called for PaymentId={PaymentId}", id);

            var payment = await _context.Payments
                .AsNoTracking()
                .SingleOrDefaultAsync(p => p.Id == id, cancellationToken);

            if (payment is null)
            {
                _logger.LogWarning("Payment with ID {PaymentId} not found", id);
                return _responseHandler.NotFound<PaymentStatusResponse>("Payment not found");
            }

            var paymentIntent = await _paymentIntentService.GetAsync(payment.StripePaymentIntentId, null, cancellationToken: cancellationToken);

            var statusResponse = new PaymentStatusResponse(
                paymentIntent.Status == "succeeded",
                paymentIntent.Status,
                payment.OrderId
            );

            return _responseHandler.Success(statusResponse, "Payment status retrieved successfully");
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Stripe error while getting payment status {PaymentId}", id);
            return _responseHandler.BadRequest<PaymentStatusResponse>("Error communicating with payment processor");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while getting payment status {PaymentId}", id);
            return _responseHandler.InternalServerError<PaymentStatusResponse>("An unexpected error occurred while getting payment status");
        }
    }

}