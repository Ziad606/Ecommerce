using Ecommerce.API.Validators.Products;
using Ecommerce.Entities.DTO.Advertisement;
using FluentValidation;

namespace Ecommerce.API.Validators.Advertisement;

public class CreateAdvertisementRequestValidator : AbstractValidator<CreateAdvertisementRequest>
{
    public CreateAdvertisementRequestValidator()
    {
        // ProductId validation - optional but if provided must be positive
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Product Id is required.");

        // StartDate validation
        RuleFor(x => x.StartDate)
            .NotEmpty()
            .WithMessage("Start date is required.")
            .Must(BeAValidDate)
            .WithMessage("Start date must be a valid date.")
            .GreaterThanOrEqualTo(DateTime.Today)
            .WithMessage("Start date cannot be in the past.");

        // EndDate validation
        RuleFor(x => x.EndDate)
            .NotEmpty()
            .WithMessage("End date is required.")
            .Must(BeAValidDate)
            .WithMessage("End date must be a valid date.")
            .GreaterThan(x => x.StartDate)
            .WithMessage("End date must be after the start date.");

        // ImageOrientation validation
        RuleFor(x => x.ImageOrientation)
            .IsInEnum()
            .WithMessage("Invalid image orientation. Must be Horizontal, Vertical, or Square.");

        // Image file validation
        RuleFor(x => x.Image)
            .NotNull()
            .WithMessage("Image file is required.")
            .SetValidator(new ProductImageFileValidator());

        RuleFor(x => x.Image)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("Product images are required.")
                .Must(x => x.Length > 0).WithMessage("Image file cannot be empty.");


        When(x => x.Image != null, () =>
        {
            RuleFor(x => x.Image)
                .SetValidator(new ProductImageFileValidator());
        });


        // Additional business rule: campaign duration validation
        RuleFor(x => x)
            .Must(HaveValidCampaignDuration)
            .WithMessage("Campaign duration must be between 1 day and 1 year.")
            .OverridePropertyName("EndDate");
    }

    private bool BeAValidDate(DateTime date)
    {
        return date != default(DateTime) && date > DateTime.MinValue && date < DateTime.MaxValue;
    }


    private bool HaveValidCampaignDuration(CreateAdvertisementRequest request)
    {
        if (request.StartDate == default || request.EndDate == default)
            return true; // Let other validators handle null/default dates

        var duration = request.EndDate - request.StartDate;
        return duration.TotalDays >= 1 && duration.TotalDays <= 365;
    }
}
