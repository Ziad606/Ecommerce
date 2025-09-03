using Ecommerce.API.Validators.Products;
using Ecommerce.Entities.DTO.Advertisement;
using FluentValidation;

namespace Ecommerce.API.Validators.Advertisement;

public class UpdateAdvertisementRequestValidator : AbstractValidator<UpdateAdvertisementRequest>
{
    public UpdateAdvertisementRequestValidator()
    {

        When(x => x.StartDate.HasValue && x.EndDate.HasValue, () =>
        {
            RuleFor(x => x)
                .Must(x => x.StartDate >= DateTime.UtcNow && x.StartDate <= x.EndDate)
                .WithMessage("Start date cannot be in the past and must be before or equal to end date.");
        });



        When(x => x.Image != null, () =>
        {
            RuleFor(x => x.Image)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("Product images are required.");
            RuleFor(x => x.Image)
                .Cascade(CascadeMode.Stop)
                .Must(x => x.Length > 0).WithMessage("Image file cannot be empty.");

            RuleFor(x => x.Image)
                .Cascade(CascadeMode.Stop)
                .SetValidator(new ProductImageFileValidator());
        });

    }



}
