using Ecommerce.Entities.Models;
using Ecommerce.Utilities.Enums;

namespace Ecommerce.API.Validators.Products;

public static class ProductValidationHelpers
{
    public static bool IsSupportedImageType(string contentType)
    {
        var supported = new[] { "image/jpeg", "image/png" };
        return Array.Exists(supported, ct => ct.Equals(contentType, StringComparison.OrdinalIgnoreCase));
    }

    public static bool IsPrimaryImage(ProductImage image)
    {
        return image != null && image.IsPrimary;
    }


}
