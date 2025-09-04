using Ecommerce.DataAccess.ApplicationContext;

namespace Ecommerce.DataAccess.Services.Payments;
public class DiscountService(AuthContext context) : IDiscountService
{
    private readonly AuthContext _context = context;

    public async Task<(bool IsValid, decimal DiscountPercentage, string Message)> ValidatePromoCodeAsync(string promoCode)
    {
        await Task.CompletedTask;

        if (string.IsNullOrWhiteSpace(promoCode))
        {
            return (false, 0, "Promo code is required");
        }
        var existingPromo = await _context.PromoCodes.FindAsync(promoCode.Trim());
        if (existingPromo is not null)
        {
            return (true, existingPromo.DiscountPercentage, "Valid promo code applied");
        }

        return (false, 0, "Invalid promo code");
    }

    public decimal CalculateDiscount(decimal originalAmount, decimal discountPercentage)
    {
        return originalAmount * (discountPercentage / 100);
    }
}
