namespace Ecommerce.Entities.DTO.PromoCodes;
public class CreatePromoCodeRequest
{
    public string Code { get; set; } = string.Empty;
    public DateTime StartAt { get; set; }
    public DateTime EndAt { get; set; }
    public decimal DiscountPercentage { get; set; }
}
