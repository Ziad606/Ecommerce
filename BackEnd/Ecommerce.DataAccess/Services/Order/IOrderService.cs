using Ecommerce.Entities.DTO.Orders;
using Ecommerce.Entities.Shared.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.DataAccess.Services.Order
{
    public interface IOrderService
    {
        Task<Response<Guid>> CreateOrderAsync(CreateOrderRequest dto);

        Task<Response<Guid>> CreateOrderFromCartAsync(BuyerCreateOrderRequest dto, Guid buyerId);  
                                                                                              
        Task<Response<GetOrdersResponse>> GetOrdersAsync(GetOrdersRequest query, Guid? buyerId = null);

        Task<Response<OrderDetailsResponse>> UpdateOrderAsync(Guid id, UpdateOrderRequest dto);

        Task<Response<string>> DeleteOrderAsync(Guid id, DeleteOrderRequest dto);

        Task<Response<Guid>> CancelOrderAsBuyerAsync(Guid orderId, DeleteOrderRequest dto, string buyerId);

        Task<Response<Guid>> DeleteOrderAsBuyerAsync(Guid orderId, DeleteOrderRequest dto, string buyerId);


    }

}

