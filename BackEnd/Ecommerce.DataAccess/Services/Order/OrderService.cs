using Ecommerce.DataAccess.ApplicationContext;
using Ecommerce.DataAccess.Services.ImageUploading;
using Ecommerce.DataAccess.Services.Products;
using Ecommerce.Entities.DTO.Orders;
using Ecommerce.Entities.Shared.Bases;
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
        ILogger<ProductService> logger) : IOrderService
    {
        private readonly AuthContext _context = context;
        private readonly ResponseHandler _responseHandler = responseHandler;
        private readonly ILogger<ProductService> _logger = logger;

        public async Task<Response<Guid>> CreateOrderAsync(CreateOrderRequest dto)
        {
            if(dto == null)
            {
                _logger.LogWarning("Order data was null.");
                return _responseHandler.BadRequest<Guid>("Order data is required.");
            }

            var buyer = await _context.Users.FirstOrDefaultAsync(u => u.Id.ToString() == dto.BuyerId.ToString());
            if(buyer == null)
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

        }
    }
}
