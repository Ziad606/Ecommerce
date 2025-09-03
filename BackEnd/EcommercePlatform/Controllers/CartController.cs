
using Ecommerce.Entities.DTO.Cart;
using Ecommerce.Entities.DTO.CartDTOs;
using Ecommerce.Entities.Shared.Bases;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ecommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "User")]
    public class CartController(ICartService cartService, ResponseHandler responseHandler) : ControllerBase
    {
        private readonly ICartService _cartService = cartService;
        private readonly ResponseHandler _responseHandler = responseHandler;

        [HttpPost("items")]
        public async Task<IActionResult> AddItemToCart([FromBody] AddCartReq request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(_responseHandler.HandleModelStateErrors(ModelState));

            var buyerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _cartService.AddItemToCartAsync(request, buyerId!, cancellationToken);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpGet("")]
        public async Task<IActionResult> GetCart(CancellationToken cancellationToken)
        {
            var buyerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _cartService.GetCartAsync(buyerId!, cancellationToken);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPut("items/{cartItemId:guid}")]
        public async Task<IActionResult> UpdateCartItemQuantity(
            [FromRoute] Guid cartItemId,
            [FromBody] UpdateCartRequest request,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(_responseHandler.HandleModelStateErrors(ModelState));

            var buyerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _cartService.UpdateCartItemQuantityAsync(buyerId!, cartItemId, request.Quantity, cancellationToken);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpDelete("items/{cartItemId:guid}")]
        public async Task<IActionResult> RemoveItemFromCart([FromRoute] Guid cartItemId, CancellationToken cancellationToken)
        {
            var buyerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _cartService.RemoveItemFromCartAsync(buyerId!, cartItemId, cancellationToken);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpDelete("")]
        public async Task<IActionResult> ClearCart(CancellationToken cancellationToken)
        {
            var buyerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _cartService.ClearCartAsync(buyerId!, cancellationToken);
            return StatusCode((int)result.StatusCode, result);
        }
    }
}