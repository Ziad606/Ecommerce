namespace Ecommerce.Entities.DTO.Profile;
public class GetProfileResponse
{
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }

    // Personal Info
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }

    // Stats
    public int OrdersCount { get; set; }
    public int ReviewsCount { get; set; }
    public int WishlistCount { get; set; }
}
