using Ecommerce.Entities.DTO.CartDTOs;
using Ecommerce.Entities.Shared.Bases;
using Ecommerce.Services.Interfaces;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	//[Authorize] // فعّل الـ Authorization
	public class CartController : ControllerBase
	{
		private readonly ICartService _cartService;
		private readonly ResponseHandler _responseHandler;
		private readonly IValidator<AddCartReq> _addCartItemValidator;

		public CartController(
			ICartService cartService,
			ResponseHandler responseHandler,
			IValidator<AddCartReq> addCartItemValidator)
		{
			_cartService = cartService;
			_responseHandler = responseHandler;
			_addCartItemValidator = addCartItemValidator;
		}

		/// <summary>
		/// Add item to the current user's cart
		/// </summary>
		[HttpPost("items")]
		public async Task<IActionResult> AddItemToCart([FromBody] AddCartReq dto)
		{
			// Validate input
			ValidationResult validationResult = await _addCartItemValidator.ValidateAsync(dto);
			if (!validationResult.IsValid)
			{
				string errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
				var badResponse = _responseHandler.BadRequest<object>(errors);
				return StatusCode((int)badResponse.StatusCode, badResponse);
			}

			try
			{
				// Get buyer id from claims
				var buyerId = User?.Identity?.Name;
				//if (string.IsNullOrEmpty(buyerId))
				//{
				//	var unauthorized = _responseHandler.Unauthorized<string>("User not authenticated");
				//	return StatusCode((int)unauthorized.StatusCode, unauthorized);
				//}

				// Call service - الـ Service هيرجع الـ Response DTO جاهز
				var cartItemResponse = await _cartService.AddItemToCartAsync(dto, buyerId);

				if (cartItemResponse == null)
				{
					var badResponse = _responseHandler.BadRequest<object>("Product not available or insufficient stock");
					return StatusCode((int)badResponse.StatusCode, badResponse);
				}

				// إرجاع الاستجابة الناجحة
				var successResponse = _responseHandler.Created(cartItemResponse, "Item added to cart successfully");
				return StatusCode((int)successResponse.StatusCode, successResponse);
			}
			catch (Exception ex)
			{
				var errorResponse = _responseHandler.ServerError<string>(ex.Message);
				return StatusCode((int)errorResponse.StatusCode, errorResponse);
			}
		}

		/// <summary>
		/// Get current user's cart
		/// </summary>
		[HttpGet]
		public async Task<IActionResult> GetCart()
		{
			try
			{
				// Get buyer id from claims
				var buyerId = User?.Identity?.Name;
				//if (string.IsNullOrEmpty(buyerId))
				//{
				//	var unauthorized = _responseHandler.Unauthorized<string>("User not authenticated");
				//	return StatusCode((int)unauthorized.StatusCode, unauthorized);
				//}

				// Call service
				var cartResponse = await _cartService.GetCartAsync(buyerId);

				if (cartResponse == null)
				{
					// إرجاع كارت فاضي لو مافيش كارت
					var emptyCart = new GetCartResponse
					{
						Id = Guid.Empty,
						Items = new List<CartItemDetailsDto>(),
						TotalItems = 0,
						TotalPrice = 0,
						CreatedAt = DateTime.UtcNow,
						UpdatedAt = null
					};
					var emptyResponse = _responseHandler.Success(emptyCart, "Your cart is empty");
					return StatusCode((int)emptyResponse.StatusCode, emptyResponse);
				}

				// إرجاع الكارت مع البيانات
				var successResponse = _responseHandler.Success(cartResponse, "Cart retrieved successfully");
				return StatusCode((int)successResponse.StatusCode, successResponse);
			}
			catch (Exception ex)
			{
				var errorResponse = _responseHandler.ServerError<string>(ex.Message);
				return StatusCode((int)errorResponse.StatusCode, errorResponse);
			}
		}
	}
}