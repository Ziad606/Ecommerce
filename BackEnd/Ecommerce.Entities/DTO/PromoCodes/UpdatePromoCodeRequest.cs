namespace Ecommerce.Entities.DTO.PromoCodes;
public class UpdatePromoCodeRequest
{
    public string Code { get; set; } = string.Empty; // used as PK
    public DateTime? StartAt { get; set; }
    public DateTime? EndAt { get; set; }
    public decimal? DiscountPercentage { get; set; }
    public bool? IsDeleted { get; set; }
}
