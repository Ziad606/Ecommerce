using Ecommerce.DataAccess.Services.Order;
using Ecommerce.Entities.DTO.Orders;
using Ecommerce.Entities.Shared.Bases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ecommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController(
        IOrderService orderService,ResponseHandler responseHandler) : ControllerBase
    {
        private readonly IOrderService _orderService = orderService;
        private readonly ResponseHandler _responseHandler = responseHandler;

        [HttpPost("admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(_responseHandler.HandleModelStateErrors(ModelState));
            var result = await _orderService.CreateOrderAsync(request);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetOrdersAsync([FromQuery]GetOrdersRequest dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(_responseHandler.HandleModelStateErrors(ModelState));

            var result = await _orderService.GetOrdersAsync(dto);
            return StatusCode((int)result.StatusCode, result);
        }
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateOrder(Guid id, [FromBody] UpdateOrderRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(_responseHandler.HandleModelStateErrors(ModelState));

            var result = await _orderService.UpdateOrderAsync(id, request);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpDelete("admin/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteOrder(Guid id, [FromBody] DeleteOrderRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(_responseHandler.HandleModelStateErrors(ModelState));

            var result = await _orderService.DeleteOrderAsync(id, request);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPost("")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> CreateMyOrder([FromBody] BuyerCreateOrderRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(_responseHandler.HandleModelStateErrors(ModelState));

            var buyerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "");
            var result = await _orderService.CreateOrderFromCartAsync(request, buyerId);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpGet("")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetMyOrders([FromQuery] GetOrdersRequest dto)
        {
            if (!ModelState.IsValid) return BadRequest(_responseHandler.HandleModelStateErrors(ModelState));

            var buyerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "");
            var result = await _orderService.GetOrdersAsync(dto, buyerId);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPost("cancel/{id}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> CancelOrder(Guid id, [FromBody] DeleteOrderRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(_responseHandler.HandleModelStateErrors(ModelState));

            var buyerId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(buyerId))
                return Unauthorized();

            var result = await _orderService.CancelOrderAsBuyerAsync(id, request, buyerId);
            return StatusCode((int)result.StatusCode, result);
        }

        [Route("{id}")]
        [HttpDelete]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> DeleteOrderAsBuyer(Guid id, [FromBody] DeleteOrderRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(_responseHandler.HandleModelStateErrors(ModelState));
            ////
            var buyerId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(buyerId))
                return Unauthorized();

            var result = await _orderService.DeleteOrderAsBuyerAsync(id, request, buyerId);
            return StatusCode((int)result.StatusCode, result);
        }
    }
}
