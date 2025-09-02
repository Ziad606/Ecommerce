using Ecommerce.Entities.DTO.Shared.Product;
using Ecommerce.Utilities.Enums;
using FluentValidation;

namespace Ecommerce.API.Validators.Products;

public class ProductFiltersValidator : AbstractValidator<ProductFilters<ProductSorting>> 
{
    public ProductFiltersValidator()
    {
        RuleFor(p => p.PriceStart)
            .GreaterThan(0)
            .WithMessage("Start price must be greater than 0.");
        RuleFor(p => p.PriceEnd)
            .GreaterThan(0)
            .WithMessage("End price must be greater than 0.");
        
        RuleFor(p => p.PriceEnd)
            .GreaterThan(p => p.PriceStart)
            .WithMessage("End price must be greater than Start price.");

        
    }
}