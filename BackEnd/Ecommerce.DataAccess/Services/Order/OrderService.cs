using Ecommerce.DataAccess.ApplicationContext;
using Ecommerce.DataAccess.Services.ImageUploading;
using Ecommerce.DataAccess.Services.Products;
using Ecommerce.Entities.DTO.Orders;
using Ecommerce.Entities.Shared.Bases;
using Ecommerce.Services.Implementations;
using Ecommerce.Services.Interfaces;
using Ecommerce.Utilities.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.DataAccess.Services.Order
{
    public class OrderService(
        AuthContext context,
        ResponseHandler responseHandler,
        ILogger<OrderService> logger,
        ICartService cartService) : IOrderService
    {
        private readonly AuthContext _context = context;
        private readonly ResponseHandler _responseHandler = responseHandler;
        private readonly ICartService _cartService = cartService;
        private readonly ILogger<OrderService> _logger = logger;

        public async Task<Response<Guid>> CreateOrderAsync(CreateOrderRequest dto)
        {
            if (dto == null)
            {
                _logger.LogWarning("Order data was null.");
                return _responseHandler.BadRequest<Guid>("Order data is required.");
            }

            var buyer = await _context.Users.FirstOrDefaultAsync(u => u.Id.ToString() == dto.BuyerId.ToString());
            if (buyer == null)
            {
                _logger.LogWarning("Buyer with ID {BuyerId} not found.", dto.BuyerId);
                return _responseHandler.BadRequest<Guid>("Invalid buyer.");
            }


            var order = new Entities.Models.Order
            {
                Id = Guid.NewGuid(),
                BuyerId = dto.BuyerId,
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                TotalPrice = 0,

            };
            foreach (var item in dto.OrderItems)
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == item.ProductId && !p.IsDeleted);
                if (product == null)
                {
                    _logger.LogWarning("Product with ID {ProductId} not found.", item.ProductId);
                    return _responseHandler.BadRequest<Guid>($"Product with ID {item.ProductId} not found.");
                }
                if (product.StockQuantity < item.Quantity)
                {
                    _logger.LogWarning("Insufficient stock for product ID {ProductId}. Requested: {Requested}, Available: {Available}", item.ProductId, item.Quantity, product.StockQuantity);
                    return _responseHandler.BadRequest<Guid>($"Insufficient stock for product ID {item.ProductId}.");
                }
                order.TotalPrice += product.Price * item.Quantity;
                product.StockQuantity -= item.Quantity;

                order.OrderItems.Add(new Entities.Models.OrderItem
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price
                });
            }
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Order {OrderId} created successfully for buyer {BuyerId}", order.Id, dto.BuyerId);

            return _responseHandler.Created<Guid>(order.Id, "Order created successfully.");
        }
        public async Task<Response<Guid>> CreateOrderFromCartAsync(BuyerCreateOrderRequest dto, Guid buyerId)
        {
            var buyer = await _context.Users.FindAsync(buyerId.ToString());
            if (buyer == null)
            {
                _logger.LogWarning("Buyer with ID {BuyerId} not found.", buyerId);
                return _responseHandler.BadRequest<Guid>("Buyer not found.");
            }

            var cart = await _cartService.GetCartAsync(buyerId.ToString());

            if (cart == null)
            {
                _logger.LogWarning("Cart not found for buyer with ID {BuyerId}.", buyerId);
                return _responseHandler.BadRequest<Guid>("Cart not found.");
            }

            if (!cart.Items.Any())
            {
                _logger.LogWarning("Cart is empty.");
                return _responseHandler.BadRequest<Guid>("Cart is empty.");
            }

            var order = new Entities.Models.Order
            {
                Id = Guid.NewGuid(),
                BuyerId = buyerId.ToString(),
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                TotalPrice = 0,
                ShippingAddress = $"{dto.ShippingCity}, {dto.ShippingState}, {dto.ShippingCountry} {dto.ShippingZipCode}",
                // ShippingPrice, CourierService 
            };

            foreach (var item in cart.Items)
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == item.ProductId && !p.IsDeleted);
                if (product == null || product.StockQuantity < item.Quantity)
                {
                    _logger.LogWarning("Invalid product or insufficient stock for {ProductId}. Requested: {Requested}, Available: {Available}", item.ProductId, item.Quantity, product.StockQuantity);
                    return _responseHandler.BadRequest<Guid>($"Invalid product or insufficient stock for {item.ProductId}.");
                }

                order.TotalPrice += product.Price * item.Quantity;
                product.StockQuantity -= item.Quantity;

                order.OrderItems.Add(new Entities.Models.OrderItem
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price
                });
            }

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
            //await _cartService.ClearCartAsync(buyerId);

            _logger.LogInformation("Order {OrderId} created from cart for buyer {BuyerId}", order.Id, buyerId);
            return _responseHandler.Created(order.Id, "Order created successfully.");
        }
        public async Task<Response<Guid>> CancelOrderAsBuyerAsync(Guid orderId, DeleteOrderRequest dto, string buyerId)
        {
            if (dto == null || !dto.Confirm)
            {
                _logger.LogWarning("Cancellation not confirmed for order {OrderId}.", orderId);
                return _responseHandler.BadRequest<Guid>("Cancellation not confirmed.");
            }

            var order = await _context.Orders
                .Include(o => o.Buyer)
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == orderId && !o.IsDeleted);

            if (order == null)
            {
                _logger.LogWarning("Order with ID {OrderId} not found.", orderId);
                return _responseHandler.NotFound<Guid>("Order not found.");
            }

            if (order.BuyerId != buyerId)
            {
                _logger.LogWarning("Buyer {BuyerId} attempted to cancel non-owned order {OrderId}.", buyerId, orderId);
                return _responseHandler.BadRequest<Guid>("You can only cancel your own orders.");
            }

            if (!IsValidStatusTransition(order.Status, OrderStatus.Cancelled) || order.Status != OrderStatus.Pending)
            {
                _logger.LogWarning("Invalid cancellation: Order {OrderId} is in {CurrentStatus} status.", orderId, order.Status);
                return _responseHandler.BadRequest<Guid>("Cancellation only allowed for Pending orders.");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Restore stock (similar to delete logic)
                foreach (var orderItem in order.OrderItems)
                {
                    var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == orderItem.ProductId && !p.IsDeleted);
                    if (product != null)
                    {
                        product.StockQuantity += orderItem.Quantity;
                    }
                }

                order.Status = OrderStatus.Cancelled;
                order.CancelledDate = DateTime.UtcNow;

                // Mock notification
                Console.WriteLine($"Notification: Order {orderId} canceled by buyer {order.Buyer.FirstName} {order.Buyer.LastName}");

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Order {OrderId} canceled successfully by buyer {BuyerId}.", orderId, buyerId);
                return _responseHandler.Success(order.Id, "Order canceled successfully.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error canceling order {OrderId}.", orderId);
                return _responseHandler.InternalServerError<Guid>("Failed to cancel order: " + ex.Message);
            }
        }
        public async Task<Response<Guid>> DeleteOrderAsBuyerAsync(Guid orderId, DeleteOrderRequest dto, string buyerId)
        {
            if (!dto.Confirm)
            {
                _logger.LogWarning("Deletion not confirmed for order {OrderId}.", orderId);
                return _responseHandler.BadRequest<Guid>("Deletion not confirmed.");
            }

            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.Buyer)
                .FirstOrDefaultAsync(o => o.Id == orderId && !o.IsDeleted);

            if (order == null)
            {
                _logger.LogWarning("Order with ID {OrderId} not found.", orderId);
                return _responseHandler.NotFound<Guid>("Order not found.");
            }

            if (order.BuyerId != buyerId)
            {
                _logger.LogWarning("Buyer {BuyerId} attempted to delete non-owned order {OrderId}.", buyerId, orderId);
                return _responseHandler.BadRequest<Guid>("You can only delete your own orders.");
            }

            if (order.Status != OrderStatus.Cancelled)
            {
                _logger.LogWarning("Invalid deletion: Order {OrderId} is in {CurrentStatus} status.", orderId, order.Status);
                return _responseHandler.BadRequest<Guid>("Deletion only allowed for Canceled orders.");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Restore stock 
                foreach (var orderItem in order.OrderItems)
                {
                    var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == orderItem.ProductId && !p.IsDeleted);
                    if (product != null)
                    {
                        product.StockQuantity += orderItem.Quantity;
                    }
                }

                order.IsDeleted = true; // soft delete
                //_context.Orders.Remove(order); // Cascade deletes OrderItems
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // Mock notification
                Console.WriteLine($"Notification: Order {orderId} deleted by buyer {order.Buyer.FirstName} {order.Buyer.LastName}");

                _logger.LogInformation("Order {OrderId} deleted successfully by buyer {BuyerId}.", orderId, buyerId);
                return _responseHandler.Deleted<Guid>("Order deleted successfully.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error deleting order {OrderId}.", orderId);
                return _responseHandler.InternalServerError<Guid>("Failed to delete order: " + ex.Message);
            }
        }

        public async Task<Response<GetOrdersResponse>> GetOrdersAsync(GetOrdersRequest query, Guid? buyerId = null) {
            try
            {
                var queryable = _context.Orders
                    .Include(o => o.Buyer)
                    .AsQueryable();

                // Filter by buyer ID if provided

                if (buyerId.HasValue)
                { 
                    _logger.LogInformation("Filtering orders by buyer ID: {BuyerId}", buyerId.Value);
                    queryable = queryable.Where(o => o.BuyerId == buyerId.Value.ToString());
                }


                // Apply filtering
                if (query.Status.HasValue)
                    queryable = queryable.Where(o => o.Status == query.Status);

                if (!string.IsNullOrEmpty(query.SearchTerm))
                    queryable = queryable.Where(o => o.Id.ToString().Contains(query.SearchTerm) ||
                                                     o.Buyer.FirstName.Contains(query.SearchTerm) || 
                                                     o.Buyer.LastName.Contains(query.SearchTerm));

                // Get total count for pagination
                var totalItems = await queryable.CountAsync();

                // Apply pagination
                var items = await queryable
                    .OrderByDescending(o => o.OrderDate)  // Default sort per API contract
                    .Select(o => new OrderSummaryDto
                    {
                        OrderId = o.Id,
                        BuyerName = o.Buyer.FirstName +" " + o.Buyer.LastName,
                        Status = o.Status,
                        TotalPrice = o.TotalPrice
                    })
                    .ToListAsync();

                if (!items.Any())
                    return _responseHandler.NotFound<GetOrdersResponse>("No orders found.");

                var result = new GetOrdersResponse
                {
                    Orders = items,
                    TotalCount = totalItems,
                    PageNumber = query.PageNumber,
                    PageSize = query.PageSize
                };
                _logger.LogInformation("Retrieved {Count} orders out of {Total} total.", items.Count, totalItems);
                return _responseHandler.Success<GetOrdersResponse>(result, "Orders retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving orders.");
                return _responseHandler.InternalServerError<GetOrdersResponse>("Failed to retrieve orders: " + ex.Message);
            }
        }
        ///////
        public async Task<Response<OrderDetailsResponse>> UpdateOrderAsync(Guid id, UpdateOrderRequest dto)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.Buyer)
                    .FirstOrDefaultAsync(o => o.Id == id);

                if (order == null)
                {
                    _logger.LogWarning("Order with ID {OrderId} not found.", id);
                    return _responseHandler.NotFound<OrderDetailsResponse>("Order not found.");
                }

                // Validate status transition
                if (!IsValidStatusTransition(order.Status, dto.Status))
                {
                    _logger.LogWarning("Invalid status transition from {CurrentStatus} to {NewStatus}.", order.Status, dto.Status);
                    return _responseHandler.BadRequest<OrderDetailsResponse>("Invalid status transition.");
                }

                order.Status = dto.Status;
                if (dto.Status == OrderStatus.Delivered)
                    order.DeliveredDate = DateTime.UtcNow;
                else if (dto.Status == OrderStatus.Cancelled)
                    order.CancelledDate = DateTime.UtcNow;

                // Mock notification (e.g., log to console; replace with email service later)
                Console.WriteLine($"Notification: Order {id} status updated to {dto.Status} for buyer {order.Buyer.FirstName} {order.Buyer.LastName}");

                await _context.SaveChangesAsync();

                var OrderDetailsResponse = new OrderDetailsResponse
                {
                    OrderId = order.Id,
                    Status = order.Status,
                    TotalPrice = order.TotalPrice,
                    //ShippingPrice = order.ShippingPrice,
                    ShippingAddress = order.ShippingAddress,
                    CourierService = order.CourierService,
                    OrderDate = order.OrderDate
                };

                _logger.LogInformation("Order {OrderId} status updated to {Status} successfully.", id, dto.Status);
                return _responseHandler.Success(OrderDetailsResponse, "Order status updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order {OrderId}.", id);
                return _responseHandler.InternalServerError<OrderDetailsResponse>("Failed to update order: " + ex.Message);
            }
        }

        public async Task<Response<string>> DeleteOrderAsync(Guid id, DeleteOrderRequest dto)
        {
            try
            {
                if (!dto.Confirm)
                {
                    _logger.LogWarning("Deletion not confirmed.");
                    return _responseHandler.BadRequest<string>("Deletion not confirmed.");
                }

                var order = await _context.Orders
                    .Include(o => o.OrderItems)
                    .FirstOrDefaultAsync(o => o.Id == id);

                if (order == null)
                {
                    _logger.LogWarning("Order with ID {OrderId} not found.", id);
                    return _responseHandler.NotFound<string>("Order not found.");
                }

                using var transaction = await _context.Database.BeginTransactionAsync(); // Begin Transaction

                foreach (var orderItem in order.OrderItems)
                {
                    var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == orderItem.ProductId);
                    if (product != null)
                    {
                        product.StockQuantity += orderItem.Quantity;
                    }
                }
                _context.Orders.Remove(order);  // Cascade delete removes OrderItems
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return _responseHandler.Deleted<string>("Order deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting order {OrderId}.", id);
                return _responseHandler.InternalServerError<string>("Failed to delete order: " + ex.Message);
            }
        }





        private bool IsValidStatusTransition(OrderStatus currentStatus, OrderStatus newStatus)
        {
            var validTransitions = new Dictionary<OrderStatus, OrderStatus[]>
             {
                { OrderStatus.Pending, new[] { OrderStatus.Shipped, OrderStatus.Cancelled } },
                { OrderStatus.Shipped, new[] { OrderStatus.Delivered, OrderStatus.Cancelled } },
                { OrderStatus.Delivered, new OrderStatus[] { } },
                { OrderStatus.Cancelled, new OrderStatus[] { } }
             };

            return validTransitions.TryGetValue(currentStatus, out var allowed) && allowed.Contains(newStatus);
        }
    }
}