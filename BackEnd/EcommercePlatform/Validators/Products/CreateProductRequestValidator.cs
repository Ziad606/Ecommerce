using Ecommerce.API.Validators.Products;
using Ecommerce.Entities.DTO.Products;
using FluentValidation;

namespace Ecommerce.API.Validators.Products;

public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
    {
        public CreateProductRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Product name is required.")
                .MaximumLength(100).WithMessage("Product name must not exceed 100 characters.");

            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("Category must be selected.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Product description is required.")
                .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than zero.");

            RuleFor(x => x.Dimensions)
                .MaximumLength(100).WithMessage("Dimensions must not exceed 100 characters.")
                .When(x => !string.IsNullOrEmpty(x.Dimensions));
            
            RuleFor(x => x.Material)
                .MaximumLength(100).WithMessage("Material must not exceed 100 characters.")
                .When(x => !string.IsNullOrEmpty(x.Material));
            
            RuleFor(x => x.SKU)
                .Matches(@"^[A-Za-z0-9\-]+$").WithMessage("SKU must be alphanumeric or hyphens only.")
                .MaximumLength(50).WithMessage("SKU must not exceed 50 characters.")
                .When(x => !string.IsNullOrEmpty(x.SKU));

            RuleFor(x => x.StockQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("Stock quantity cannot be negative.");

            // Consolidated Images validation with Cascade mode
            RuleFor(x => x.Images)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("Product images are required.")
                .Must(images => images.Count >= 1).WithMessage("At least one product image is required.")
                .Must(images => images.Count <= 10).WithMessage("Maximum 10 images are allowed.");

            When(x => x.Images != null && x.Images.Any(), () =>
            {
                RuleForEach(x => x.Images)
                    .SetValidator(new ProductImageFileValidator());
            });
        }
    }