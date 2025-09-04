using Ecommerce.Entities.Models;

namespace Ecommerce.Entities.DTO.PromoCodes;
public class GetPromoCodeResponse
{
    public string Code { get; set; } = string.Empty;
    public DateTime StartAt { get; set; }
    public DateTime EndAt { get; set; }
    public decimal DiscountPercentage { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsActive { get; set; }

    public GetPromoCodeResponse(PromoCode promo)
    {
        Code = promo.Code;
        StartAt = promo.StartAt;
        EndAt = promo.EndAt;
        DiscountPercentage = promo.DiscountPercentage;
        IsDeleted = promo.IsDeleted;
        CreatedAt = promo.CreatedAt;
        UpdatedAt = promo.UpdatedAt;
        IsActive = promo.IsActive;
    }

    public GetPromoCodeResponse() { }
}
