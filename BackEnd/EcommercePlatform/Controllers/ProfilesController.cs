using Ecommerce.DataAccess.Services.Profile;
using Ecommerce.Entities.DTO.Profile;
using Ecommerce.Entities.Shared.Bases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ecommerce.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ProfilesController(IProfileService profileService, ResponseHandler responseHandler, ILogger<ProfilesController> logger) : ControllerBase
{
    private readonly IProfileService _profileService = profileService;
    private readonly ResponseHandler _responseHandler = responseHandler;
    private readonly ILogger _logger = logger;

    /// <summary>
    /// Get all user profiles (Admin only)
    /// </summary>
    [HttpGet("")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetProfiles(CancellationToken cancellationToken)
    {
        var result = await _profileService.GetAllProfilesAsync(cancellationToken);
        return StatusCode((int)result.StatusCode, result);
    }

    /// <summary>
    /// Get any user profile details by ID (Admin only)
    /// </summary>
    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetProfileAdminById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(_responseHandler.HandleModelStateErrors(ModelState));

        var result = await _profileService.GetProfileByIdAsync(id.ToString(), cancellationToken);
        return StatusCode((int)result.StatusCode, result);
    }

    /// <summary>
    /// Get the authenticated user profile details
    /// </summary>
    [HttpGet("me")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> GetProfileById(CancellationToken cancellationToken)
    {
        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogWarning("Unauthorized profile update attempt");
            return StatusCode(401, _responseHandler.Unauthorized<object>("user unauthorized"));
        }
        var result = await _profileService.GetProfileByIdAsync(userId, cancellationToken);
        return StatusCode((int)result.StatusCode, result);
    }


    /// <summary>
    /// Update the authenticated user profile
    /// </summary>
    [HttpPut("me")]
    [Authorize(Roles = "User")]
    public async Task<ActionResult> UpdateProfile([FromBody] UpdateProfileRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(_responseHandler.HandleModelStateErrors(ModelState));
        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogWarning("Unauthorized profile update attempt");
            return StatusCode(401, _responseHandler.Unauthorized<object>("user unauthorized"));
        }
        var result = await _profileService.UpdateProfileAsync(userId, request, cancellationToken);
        return StatusCode((int)result.StatusCode, result);
    }

}
