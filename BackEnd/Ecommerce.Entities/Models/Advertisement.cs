using Ecommerce.Utilities.Enums;

namespace Ecommerce.Entities.Models;
public class Advertisement
{
    public Guid Id { get; set; }
    public string ImageLink { get; set; }
    public ImageOrientation ImageOrientation { get; set; }
    public Guid? ProductId { get; set; }
    public DateTime StartDate { get; set; } = DateTime.UtcNow;
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = true;

    public DateTime CreatedAt { get; set; }
    public DateTime DeletedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation property
    public Product? Product { get; set; }
}
