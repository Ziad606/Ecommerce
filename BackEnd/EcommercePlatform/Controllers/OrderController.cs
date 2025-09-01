using Ecommerce.DataAccess.Services.Order;
using Ecommerce.Entities.DTO.Orders;
using Ecommerce.Entities.Shared.Bases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController(
        IOrderService productservice,ResponseHandler responseHandler) : ControllerBase
    {
        private readonly IOrderService _orderService = productservice;
        private readonly ResponseHandler _responseHandler = responseHandler;
        [HttpPost("")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(_responseHandler.HandleModelStateErrors(ModelState));
            var result = await _orderService.CreateOrderAsync(request);
            return StatusCode((int)result.StatusCode, result);
        }
    }
}
