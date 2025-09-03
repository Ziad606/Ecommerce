using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.DataAccess.Services.Payments;
public interface IDiscountService
{
    Task<(bool IsValid, decimal DiscountPercentage, string Message)> ValidatePromoCodeAsync(string promoCode);
    decimal CalculateDiscount(decimal originalAmount, decimal discountPercentage);
}
