using Ecommerce.DataAccess.ApplicationContext;
using Ecommerce.Entities.DTO.Payments.Requests;
using Ecommerce.Entities.DTO.Payments.Responses;
using Ecommerce.Entities.Models;
using Ecommerce.Utilities.Enums;
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace Ecommerce.DataAccess.Services.Payments;

public class PaymentService(PaymentIntentService paymentIntentService, AuthContext context) : IPaymentService
{
    private readonly PaymentIntentService _paymentIntentService = paymentIntentService;
    private readonly AuthContext _context = context;

    public async Task<PaymentResponse> BuyProductAsync(Guid productId, BuyProductRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var product = await _context.Products
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id.Equals(productId), cancellationToken);

            if (product is null)
            {
                return new PaymentResponse(false, null, null, string.Empty, 0);
            }

            if (product.StockQuantity < request.Quantity)
            {
                return new PaymentResponse(false, null, null, string.Empty, 0);
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

            return new PaymentResponse(
                true,
                payment.Id,
                order.Id,
                paymentIntent.ClientSecret,
                amount
            );
        }
        catch (Exception)
        {
            return new PaymentResponse(false, null, null, string.Empty, 0);
        }
    }

    public async Task<PaymentResponse> BuyCartAsync(Guid cartId, BuyCartRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var cartItems = await _context.Set<CartItem>()
                .Include(ci => ci.Product)
                .Where(ci => ci.CartId == cartId)
                .ToListAsync(cancellationToken);

            if (!cartItems.Any())
            {
                return new PaymentResponse(false, null, null, string.Empty, 0);
            }

            foreach (var item in cartItems)
            {
                if (item.Product.StockQuantity < item.Quantity)
                {
                    return new PaymentResponse(false, null, null, string.Empty, 0);
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

            return new PaymentResponse(
                true,
                payment.Id,
                order.Id,
                paymentIntent.ClientSecret,
                totalAmount
            );
        }
        catch (Exception)
        {
            return new PaymentResponse(false, null, null, string.Empty, 0);
        }
    }

    public async Task<PaymentStatusResponse> ConfirmPaymentAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var payment = await _context.Payments
                .Include(p => p.Order)
                .SingleOrDefaultAsync(p => p.Id.Equals(id), cancellationToken);

            if (payment is null)
            {
                return new PaymentStatusResponse(false, "Payment not found", null);
            }

            var paymentIntent = await _paymentIntentService.GetAsync(payment.StripePaymentIntentId, null, cancellationToken:cancellationToken);

            var newStatus = paymentIntent.Status switch
            {
                "succeeded" => Status.Completed,
                "canceled" => Status.Cancelled,
                "requires_payment_method" => Status.Failed,
                _ => Status.Pending // default  تمام 
            };

            payment.Status = newStatus;

       
            if (newStatus == Status.Completed && payment.Order != null)
            {
                payment.Order.Status = OrderStatus.Confirmed;
            }
            else if (newStatus == Status.Cancelled && payment.Order != null)
            {
                payment.Order.Status = OrderStatus.Cancelled;
            }

            await _context.SaveChangesAsync(cancellationToken);

            return new PaymentStatusResponse(
                newStatus == Status.Completed,
                paymentIntent.Status,
                payment.OrderId
            );
        }
        catch (Exception)
        {
            return new PaymentStatusResponse(false, "Error confirming payment", null);
        }
    }

    public async Task<PaymentStatusResponse> GetPaymentStatusAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var payment = await _context.Payments
                .AsNoTracking()
                .SingleOrDefaultAsync(p => p.Id == id, cancellationToken);

            if (payment is null)
            {
                return new PaymentStatusResponse(false, "Payment not found", null);
            }

            var paymentIntent = await _paymentIntentService.GetAsync(payment.StripePaymentIntentId, null, cancellationToken: cancellationToken);

            return new PaymentStatusResponse(
                paymentIntent.Status == Status.Completed.ToString(),
                paymentIntent.Status,
                payment.OrderId
            );
        }
        catch (Exception)
        {
            return new PaymentStatusResponse(false, "Error getting payment status", null);
        }
    }
}