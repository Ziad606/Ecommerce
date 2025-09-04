namespace Ecommerce.Entities.DTO.Profile;
public class GetProfileAdminResponse
{
    // Identity / Basic Info
    public string Id { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime? DateOfBirth { get; set; }

    // Account Status
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? LastLoginDate { get; set; }

    // Roles & Permissions
    public List<string> Roles { get; set; } = new List<string>();

    // Engagement / Activity
    public int OrdersCount { get; set; }
    public int ReviewsCount { get; set; }
    public int WishlistCount { get; set; }
    public int CartItemsCount { get; set; }
}
