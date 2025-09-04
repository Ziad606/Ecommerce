namespace Ecommerce.Entities.DTO.Profile;
public class GetProfileAdminListResponse
{
    public string Id { get; set; }
    public string Email { get; set; }
    public string FullName { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginDate { get; set; }
}
