using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.DataAccess.Services.Payments;
public class DiscountService : IDiscountService
{
    private const string _validPromoCode = "SAVE20";
    private const decimal _discountPercentage = 20m;

    public async Task<(bool IsValid, decimal DiscountPercentage, string Message)> ValidatePromoCodeAsync(string promoCode)
    {
        await Task.CompletedTask;

        if (string.IsNullOrWhiteSpace(promoCode))
        {
            return (false, 0, "Promo code is required");
        }

        if (promoCode.Trim().Equals(_validPromoCode, StringComparison.CurrentCultureIgnoreCase))
        {
            return (true, _discountPercentage, "Valid promo code applied");
        }

        return (false, 0, "Invalid promo code");
    }

    public decimal CalculateDiscount(decimal originalAmount, decimal discountPercentage)
    {
        return originalAmount * (discountPercentage / 100);
    }
}
