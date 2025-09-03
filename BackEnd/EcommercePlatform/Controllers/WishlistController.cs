using Ecommerce.Entities.DTO.Wishlist;
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
    public class WishlistController(IWishlistService wishlistService, ResponseHandler responseHandler) : ControllerBase
    {
        private readonly IWishlistService _wishlistService = wishlistService;
        private readonly ResponseHandler _responseHandler = responseHandler;


        [HttpPost("items")]
        public async Task<IActionResult> AddItemToWishlist([FromBody] AddWishlistItemReq request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(_responseHandler.HandleModelStateErrors(ModelState));

            var buyerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _wishlistService.AddItemToWishlistAsync(request, buyerId!, cancellationToken);
            return StatusCode((int)result.StatusCode, result);
        }



        [HttpGet("")]
        public async Task<IActionResult> GetWishlist(CancellationToken cancellationToken)
        {
            var buyerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _wishlistService.GetWishlistAsync(buyerId!, cancellationToken);
            return StatusCode((int)result.StatusCode, result);
        }



        [HttpDelete("items/{itemId:guid}")]
        public async Task<IActionResult> RemoveItemFromWishlist([FromRoute] Guid itemId, CancellationToken cancellationToken)
        {
            var buyerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _wishlistService.RemoveItemFromWishlistAsync(buyerId!, itemId, cancellationToken);
            return StatusCode((int)result.StatusCode, result);
        }


        [HttpPost("items/{itemId:guid}/move-to-cart")]
        public async Task<IActionResult> MoveItemToCart(
            [FromRoute] Guid itemId,
            [FromBody] MoveToCartRequest request,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(_responseHandler.HandleModelStateErrors(ModelState));

            var buyerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _wishlistService.MoveItemToCartAsync(buyerId!, itemId, request.Quantity, cancellationToken);
            return StatusCode((int)result.StatusCode, result);
        }

    }
}
