using Ecommerce.DataAccess.ApplicationContext;
using Ecommerce.Entities.DTO.Profile;
using Ecommerce.Entities.Models.Auth.Identity;
using Ecommerce.Entities.Shared.Bases;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Ecommerce.DataAccess.Services.Profile;
public class ProfileService(
    AuthContext authContext,
    ResponseHandler responseHandler,
    ILogger<ProfileService> logger,
    UserManager<User> userManager
    ) : IProfileService
{
    private readonly AuthContext _context = authContext;
    private readonly ResponseHandler _responseHandler = responseHandler;
    private readonly ILogger<ProfileService> _logger = logger;
    private readonly UserManager<User> _userManager = userManager;

    public async Task<Response<List<GetProfileAdminListResponse>>> GetAllProfilesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var users = await _userManager.Users
                .OrderByDescending(u => u.CreatedAt)
                .Select(u => new GetProfileAdminListResponse
                {
                    Id = u.Id,
                    Email = u.Email,
                    FullName = u.FirstName + " " + u.LastName,
                    IsActive = u.IsActive,
                    IsDeleted = u.IsDeleted,
                    CreatedAt = u.CreatedAt,
                    LastLoginDate = u.LastLoginDate
                })
                .ToListAsync(cancellationToken);

            if (!users.Any())
            {
                _logger.LogWarning("No users found in the system.");
                return _responseHandler.NotFound<List<GetProfileAdminListResponse>>("No users found.");
            }

            _logger.LogInformation("Retrieved {Count} users from the system.", users.Count);
            return _responseHandler.Success(users, "Users retrieved successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users.");
            return _responseHandler.InternalServerError<List<GetProfileAdminListResponse>>("An error occurred while retrieving users.");
        }
    }

    public async Task<Response<GetProfileAdminResponse>> GetProfileAdminByIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userManager.Users
                .Where(u => u.Id == userId)
                .Select(u => new GetProfileAdminResponse
                {
                    Id = u.Id,
                    Email = u.Email!,
                    PhoneNumber = u.PhoneNumber!,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    DateOfBirth = u.DateOfBirth,
                    IsActive = u.IsActive,
                    IsDeleted = u.IsDeleted,
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt,
                    LastLoginDate = u.LastLoginDate,
                    Roles = (from ur in _context.UserRoles
                             join r in _context.Roles on ur.RoleId equals r.Id
                             where ur.UserId == u.Id
                             select r.Name).ToList(),
                    OrdersCount = u.Orders.Count,
                    ReviewsCount = u.Reviews.Count,
                    WishlistCount = u.Wishlist != null ? 1 : 0,
                    CartItemsCount = u.Cart != null ? u.Cart.CartItems.Count : 0
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found.", userId);
                return _responseHandler.NotFound<GetProfileAdminResponse>("User not found.");
            }

            _logger.LogInformation("Retrieved details for user with ID {UserId}.", userId);
            return _responseHandler.Success(user, "User retrieved successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user with ID {UserId}.", userId);
            return _responseHandler.InternalServerError<GetProfileAdminResponse>("An error occurred while retrieving the user.");
        }
    }


    public async Task<Response<GetProfileResponse>> GetProfileByIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userManager.Users
                .Include(u => u.Orders)
                .Include(u => u.Wishlist)
                .Include(u => u.Reviews)
                .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted, cancellationToken);

            if (user == null)
            {
                _logger.LogWarning("Profile with ID {UserId} not found", userId);
                return _responseHandler.NotFound<GetProfileResponse>("Profile not found.");
            }

            var response = new GetProfileResponse
            {
                Email = user.Email!,
                PhoneNumber = user.PhoneNumber,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth,
                OrdersCount = user.Orders.Count,
                ReviewsCount = user.Reviews.Count,
                WishlistCount = user.Wishlist?.WishlistItems.Count ?? 0
            };

            _logger.LogInformation("Profile with ID {UserId} retrieved successfully", userId);
            return _responseHandler.Success(response, "Profile retrieved successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving profile with ID: {UserId}", userId);
            return _responseHandler.InternalServerError<GetProfileResponse>("An error occurred while retrieving the profile.");
        }
    }


}
