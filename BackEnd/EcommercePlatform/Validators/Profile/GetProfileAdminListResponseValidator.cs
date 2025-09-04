using Ecommerce.Entities.DTO.Profile;
using FluentValidation;

namespace Ecommerce.API.Validators.User;

public class GetProfileAdminListResponseValidator : AbstractValidator<GetProfileAdminListResponse>
{
    public GetProfileAdminListResponseValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required.")
            .MaximumLength(200).WithMessage("Full name cannot exceed 200 characters.");
    }
}