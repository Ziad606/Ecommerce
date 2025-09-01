using Ecommerce.API.Validators.Products;
using Ecommerce.Entities.DTO.Product;
using Ecommerce.Utilities.Enums;
using FluentValidation;

namespace Ecommerce.API.Validators
{
    public class UpdateproductRequestValidator : AbstractValidator<UpdateProductRequest>
    {
        public UpdateproductRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Product name cannot be empty if provided.")
                .MaximumLength(100).WithMessage("Product name must not exceed 100 characters.")
                .When(x => x.Name != null);
            
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Product description cannot be empty if provided.")
                .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.")
                .When(x => x.Description != null);
            
           
            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than zero.")
                .When(x => x.Price == 0); 


            RuleFor(x => x.StockQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("Stock quantity cannot be negative.")
                .When(x => x.StockQuantity == 0);
            
            When(x => x.Images != null && x.Images.Any(), () =>
            {
                RuleFor(x => x.Images)
                    .Must(images => images.Count <= 10)
                    .WithMessage("Maximum 10 images are allowed.");

                RuleForEach(x => x.Images)
                    .SetValidator(new ProductImageFileValidator());
            });

        }
        
    }
}
