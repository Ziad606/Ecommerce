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
}
