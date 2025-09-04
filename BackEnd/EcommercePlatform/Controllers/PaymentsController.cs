using Ecommerce.DataAccess.Services.Payments;
using Ecommerce.Entities.DTO.Payments.Requests;
using Ecommerce.Entities.Shared.Bases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Ecommerce.API.Controllers;

/// <summary>
/// Handles payment operations including product purchases, cart purchases, payment confirmation and promo code validation
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize]
[Produces("application/json")]
public class PaymentsController(IPaymentService paymentService, ResponseHandler responseHandler) : ControllerBase
{
    private readonly IPaymentService _paymentService = paymentService;
    private readonly ResponseHandler _responseHandler = responseHandler;

    /// <summary>
    /// Initiates a payment process for purchasing a specific product
    /// </summary>
    /// <param name="productId">The unique identifier of the product to purchase</param>
    /// <param name="request">Payment details including payment method, billing information, and quantity</param>
    /// <param name="cancellationToken">Cancellation token for the async operation</param>
    /// <returns>Payment initiation result with payment ID and status</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST /api/payments/buy-product/3fa85f64-5717-4562-b3fc-2c963f66afa6
    ///     {
    ///         "quantity": 1,
    ///         "paymentMethodId": "pm_1234567890",
    ///         "billingAddress": {
    ///             "street": "123 Main St",
    ///             "city": "New York",
    ///             "state": "NY",
    ///             "zipCode": "10001",
    ///             "country": "US"
    ///         },
    ///         "promoCode": "SAVE10"
    ///     }
    /// 
    /// </remarks>
    /// <response code="200">Payment successfully initiated</response>
    /// <response code="400">Invalid request data or validation errors</response>
    /// <response code="401">Unauthorized - authentication required</response>
    /// <response code="404">Product not found</response>
    /// <response code="422">Payment processing failed</response>
    /// <response code="500">Internal server error</response>
    [HttpPost("buy-product/{productId}")]
    [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(object), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(object), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(object), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(object), (int)HttpStatusCode.UnprocessableEntity)]
    [ProducesResponseType(typeof(object), (int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> BuyProduct([FromRoute] Guid productId, [FromBody] BuyProductRequest request, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return BadRequest(_responseHandler.HandleModelStateErrors(ModelState));

        var result = await _paymentService.BuyProductAsync(productId, request, cancellationToken);
        return StatusCode((int)result.StatusCode, result);
    }

    /// <summary>
    /// Initiates a payment process for purchasing all items in a shopping cart
    /// </summary>
    /// <param name="cartId">The unique identifier of the shopping cart to purchase</param>
    /// <param name="request">Payment details including payment method and billing information</param>
    /// <param name="cancellationToken">Cancellation token for the async operation</param>
    /// <returns>Payment initiation result with payment ID and total amount</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST /api/payments/buy-cart/3fa85f64-5717-4562-b3fc-2c963f66afa6
    ///     {
    ///         "paymentMethodId": "pm_1234567890",
    ///         "billingAddress": {
    ///             "street": "123 Main St",
    ///             "city": "New York",
    ///             "state": "NY",
    ///             "zipCode": "10001",
    ///             "country": "US"
    ///         },
    ///         "shippingAddress": {
    ///             "street": "456 Oak Ave",
    ///             "city": "Los Angeles",
    ///             "state": "CA",
    ///             "zipCode": "90210",
    ///             "country": "US"
    ///         },
    ///         "promoCode": "FREESHIP"
    ///     }
    /// 
    /// </remarks>
    /// <response code="200">Payment successfully initiated</response>
    /// <response code="400">Invalid request data or validation errors</response>
    /// <response code="401">Unauthorized - authentication required</response>
    /// <response code="404">Cart not found or empty</response>
    /// <response code="422">Payment processing failed</response>
    /// <response code="500">Internal server error</response>
    [HttpPost("buy-cart/{cartId}")]
    [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(object), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(object), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(object), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(object), (int)HttpStatusCode.UnprocessableEntity)]
    [ProducesResponseType(typeof(object), (int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> BuyCart([FromRoute] Guid cartId, [FromBody] BuyCartRequest request, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return BadRequest(_responseHandler.HandleModelStateErrors(ModelState));

        var result = await _paymentService.BuyCartAsync(cartId, request, cancellationToken);
        return StatusCode((int)result.StatusCode, result);
    }

    /// <summary>
    /// Confirms a pending payment after successful payment provider processing
    /// </summary>
    /// <param name="paymentId">The unique identifier of the payment to confirm</param>
    /// <param name="cancellationToken">Cancellation token for the async operation</param>
    /// <returns>Payment confirmation result with updated status</returns>
    /// <remarks>
    /// This endpoint should be called after receiving confirmation from the payment provider (e.g., Stripe webhook).
    /// It updates the payment status and triggers order fulfillment processes.
    /// 
    /// Sample request:
    /// 
    ///     POST /api/payments/confirm/3fa85f64-5717-4562-b3fc-2c963f66afa6
    /// 
    /// </remarks>
    /// <response code="200">Payment successfully confirmed</response>
    /// <response code="401">Unauthorized - authentication required</response>
    /// <response code="404">Payment not found</response>
    /// <response code="409">Payment already confirmed or in invalid state</response>
    /// <response code="500">Internal server error</response>
    [HttpPost("confirm/{paymentId}")]
    [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(object), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(object), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(object), (int)HttpStatusCode.Conflict)]
    [ProducesResponseType(typeof(object), (int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> ConfirmPayment([FromRoute] Guid paymentId, CancellationToken cancellationToken = default)
    {
        var result = await _paymentService.ConfirmPaymentAsync(paymentId, cancellationToken);
        return StatusCode((int)result.StatusCode, result);
    }

    /// <summary>
    /// Retrieves the current status of a payment transaction
    /// </summary>
    /// <param name="paymentId">The unique identifier of the payment to check</param>
    /// <param name="cancellationToken">Cancellation token for the async operation</param>
    /// <returns>Payment status information including current state and transaction details</returns>
    /// <remarks>
    /// Returns detailed payment information including:
    /// - Payment status (Pending, Confirmed, Failed, Cancelled)
    /// - Transaction amount and currency
    /// - Payment method used
    /// - Timestamps for key events
    /// 
    /// Sample request:
    /// 
    ///     GET /api/payments/status/3fa85f64-5717-4562-b3fc-2c963f66afa6
    /// 
    /// </remarks>
    /// <response code="200">Payment status retrieved successfully</response>
    /// <response code="401">Unauthorized - authentication required</response>
    /// <response code="404">Payment not found</response>
    /// <response code="500">Internal server error</response>
    [HttpGet("status/{paymentId}")]
    [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(object), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(object), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(object), (int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> GetPaymentStatus([FromRoute] Guid paymentId, CancellationToken cancellationToken = default)
    {
        var result = await _paymentService.GetPaymentStatusAsync(paymentId, cancellationToken);
        return StatusCode((int)result.StatusCode, result);
    }

    /// <summary>
    /// Validates a promotional code and returns applicable discount information
    /// </summary>
    /// <param name="request">Promo code validation request containing the code and optional cart/product context</param>
    /// <param name="cancellationToken">Cancellation token for the async operation</param>
    /// <returns>Promo code validation result with discount details</returns>
    /// <remarks>
    /// Validates promotional codes against current promotions and user eligibility.
    /// Returns discount amount, percentage, or free shipping information if valid.
    /// 
    /// Sample request:
    /// 
    ///     POST /api/payments/validate-promo
    ///     {
    ///         "promoCode": "SAVE10",
    ///         "cartId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///         "totalAmount": 100.00
    ///     }
    /// 
    /// </remarks>
    /// <response code="200">Promo code validation completed (valid or invalid)</response>
    /// <response code="400">Invalid request data or validation errors</response>
    /// <response code="401">Unauthorized - authentication required</response>
    /// <response code="500">Internal server error</response>
    [HttpPost("validate-promo")]
    [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(object), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(object), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(object), (int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> ValidatePromoCode([FromBody] ValidatePromoRequest request, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return BadRequest(_responseHandler.HandleModelStateErrors(ModelState));

        var result = await _paymentService.ValidatePromoCodeAsync(request, cancellationToken);
        return StatusCode((int)result.StatusCode, result);
    }
}