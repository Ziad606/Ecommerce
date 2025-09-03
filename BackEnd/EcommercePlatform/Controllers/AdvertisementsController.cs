using Ecommerce.DataAccess.ApplicationContext;
using Ecommerce.DataAccess.Services.Advertisement;
using Ecommerce.Entities.DTO.Advertisement;
using Ecommerce.Entities.Shared.Bases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AdvertisementsController(AuthContext context, ResponseHandler responseHandler, IAdvertisementService advertisementService) : ControllerBase
{
    private readonly AuthContext _context = context;
    private readonly ResponseHandler _responseHandler = responseHandler;
    private readonly IAdvertisementService _advertisementService = advertisementService;

    [HttpPost("")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateAdvertisement([FromForm] CreateAdvertisementRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(_responseHandler.HandleModelStateErrors(ModelState));
        }
        var result = await _advertisementService.CreateAdvertisementAsync(request, cancellationToken);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpGet("")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllAdvertisements(CancellationToken cancellationToken)
    {
        var result = await _advertisementService.GetAllAdvertisementsAsync(cancellationToken);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllAdvertisements(Guid id, CancellationToken cancellationToken)
    {
        var result = await _advertisementService.GetAdvertisementByIdAsync(id, cancellationToken);
        return StatusCode((int)result.StatusCode, result);
    }


    [HttpPut("")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateAdvertisement([FromForm] UpdateAdvertisementRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(_responseHandler.HandleModelStateErrors(ModelState));
        }
        var result = await _advertisementService.UpdateAdvertisementAsync(request, cancellationToken);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteAdvertisements(Guid id, CancellationToken cancellationToken)
    {
        var result = await _advertisementService.DeleteAdvertisementAsync(id, cancellationToken);
        return StatusCode((int)result.StatusCode, result);
    }


}
