using Ecommerce.DataAccess.Services.Payments;
using Ecommerce.Entities.DTO.Payments.Requests;
using Ecommerce.Entities.Shared.Bases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers;
[Route("api/[controller]")] // best practice payments = > small letters the api keyword optional but i too remove it 
[ApiController]
[Authorize] // add the role if we have or permissions in this controller.
public class PaymentsController(IPaymentService paymentService, ResponseHandler responseHandler) : ControllerBase
{
    private readonly IPaymentService _paymentService = paymentService;
    private readonly ResponseHandler _responseHandler = responseHandler; // TODO: Handle the response. (tomorrow!)

    [HttpPost("buy-product/{productId}")]
    public async Task<IActionResult> BuyProduct([FromRoute]Guid productId, [FromBody] BuyProductRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _paymentService.BuyProductAsync(productId, request, cancellationToken);

        if (!result.Success)
        {
            return BadRequest("Failed to create payment for product");
        }
        return Ok(result);
    }
    [HttpPost("buy-cart/{cartId}")]
    public async Task<IActionResult> BuyCart([FromRoute]Guid cartId, [FromBody] BuyCartRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _paymentService.BuyCartAsync(cartId, request, cancellationToken);

        if (!result.Success)
        {
            return BadRequest("Failed to create payment for cart");
        }

        return Ok(result);
    }
    [HttpPost("confirm/{paymentId}")]
    public async Task<IActionResult> ConfirmPayment([FromRoute]Guid paymentId, CancellationToken cancellationToken = default)
    {
        var result = await _paymentService.ConfirmPaymentAsync(paymentId, cancellationToken);

        return Accepted(result); // 202 for confirm 
    }
    [HttpGet("status/{paymentId}")]
    public async Task<IActionResult> GetPaymentStatus([FromRoute]Guid paymentId, CancellationToken cancellationToken = default)
    {
        var result = await _paymentService.GetPaymentStatusAsync(paymentId, cancellationToken);

        return Ok(result);
    }
}
