using Ecommerce.DataAccess.Services.Review;
using Ecommerce.Entities.DTO.Reviews;
using Ecommerce.Entities.Shared.Bases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ecommerce.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        private readonly ResponseHandler _responseHandler;

        public ReviewsController(IReviewService reviewService, ResponseHandler responseHandler)
        {
            _reviewService = reviewService;
            _responseHandler = responseHandler;
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> CreateReview([FromForm] CreateReviewRequest dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(_responseHandler.HandleModelStateErrors(ModelState));

            var buyerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(buyerId))
                return Unauthorized();

            var result = await _reviewService.CreateReviewAsync(dto, buyerId);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> UpdateReview(Guid id, [FromBody] UpdateReviewRequest dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(_responseHandler.HandleModelStateErrors(ModelState));

            var buyerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(buyerId))
                return Unauthorized();

            var result = await _reviewService.UpdateReviewAsync(id, dto, buyerId);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> DeleteReview(Guid id, [FromBody] DeleteReviewRequest dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(_responseHandler.HandleModelStateErrors(ModelState));

            var buyerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(buyerId))
                return Unauthorized();

            var result = await _reviewService.DeleteReviewAsync(id, dto, buyerId);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllReviews([FromQuery] GetReviewsRequest dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(_responseHandler.HandleModelStateErrors(ModelState));

            var result = await _reviewService.GetAllReviewsAsync(dto);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetReviewById(Guid id)
        {
            var result = await _reviewService.GetReviewByIdAsync(id);
            return StatusCode((int)result.StatusCode, result);
        }
    }
}
