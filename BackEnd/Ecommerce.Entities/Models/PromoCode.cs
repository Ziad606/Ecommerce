namespace Ecommerce.Entities.Models;
public class PromoCode
{
    public string Code { get; set; }

    public DateTime StartAt { get; set; }
    public DateTime EndAt { get; set; }

    public decimal DiscountPercentage { get; set; }

    public bool IsActive => DateTime.UtcNow >= StartAt && DateTime.UtcNow <= EndAt;

    public bool IsDeleted { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime CreatedAt { get; set; }

}
