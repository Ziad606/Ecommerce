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
        Task<Response<GetOrdersResponse>> GetOrdersAsync(GetOrdersRequest dto);

    }
}
