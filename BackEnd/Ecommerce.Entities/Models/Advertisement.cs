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

    // Navigation property
    public Product? Product { get; set; }
}
