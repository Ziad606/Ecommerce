using Ecommerce.DataAccess.Services.PromoCode;
using Ecommerce.Entities.DTO.PromoCodes;
using Ecommerce.Entities.Shared.Bases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class PromoCodesController(ResponseHandler responseHandler, IPromoCodeService promoCodeService) : ControllerBase
{
    private readonly ResponseHandler _responseHandler = responseHandler;
    private readonly IPromoCodeService _promoCodeService = promoCodeService;

    [HttpPost("")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreatePromoCode([FromBody] CreatePromoCodeRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(_responseHandler.HandleModelStateErrors(ModelState));

        var result = await _promoCodeService.CreatePromoCodeAsync(request, cancellationToken);
        return StatusCode((int)result.StatusCode, result);
    }

    // Get PromoCode by Code
    [HttpGet("{code}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetPromoCodeByCode(string code, CancellationToken cancellationToken)
    {
        var result = await _promoCodeService.GetPromoCodeByCodeAsync(code, cancellationToken);
        return StatusCode((int)result.StatusCode, result);
    }

    // Get All PromoCodes
    [HttpGet("")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllPromoCodes(CancellationToken cancellationToken)
    {
        var result = await _promoCodeService.GetAllPromoCodesAsync(cancellationToken);
        return StatusCode((int)result.StatusCode, result);
    }

    // Update PromoCode
    [HttpPut("")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdatePromoCode([FromBody] UpdatePromoCodeRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(_responseHandler.HandleModelStateErrors(ModelState));

        var result = await _promoCodeService.UpdatePromoCodeAsync(request, cancellationToken);
        return StatusCode((int)result.StatusCode, result);
    }

    // Delete PromoCode
    [HttpDelete("{code}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeletePromoCode(string code, CancellationToken cancellationToken)
    {
        var result = await _promoCodeService.DeletePromoCodeAsync(code, cancellationToken);
        return StatusCode((int)result.StatusCode, result);
    }
}
