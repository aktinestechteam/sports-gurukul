using FluentValidation;
using SportsGurukul.Application.Features.UserManagement.Commands.CreateUserProfile;

namespace SportsGurukul.Application.Features.UserManagement.Validators;

public class CreateUserProfileValidator : AbstractValidator<CreateUserProfileCommand>
{
    public CreateUserProfileValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(x => x.DateOfBirth)
            .LessThanOrEqualTo(DateTime.UtcNow.AddYears(-13))
                .WithMessage("User must be at least 13 years old.");

        RuleFor(x => x.Bio)
            .MaximumLength(2000).WithMessage("Bio must not exceed 2000 characters.");

        RuleFor(x => x.Height)
            .MaximumLength(20).WithMessage("Height must not exceed 20 characters.");

        RuleFor(x => x.Weight)
            .MaximumLength(20).WithMessage("Weight must not exceed 20 characters.");

        RuleFor(x => x.PreferredSport)
            .MaximumLength(100).WithMessage("Preferred sport must not exceed 100 characters.");

        RuleFor(x => x.ExperienceLevel)
            .MaximumLength(50).WithMessage("Experience level must not exceed 50 characters.");

        RuleFor(x => x.PrimaryPhoneNumber)
            .MaximumLength(15).WithMessage("Phone number must not exceed 15 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.PrimaryPhoneNumber));

        RuleFor(x => x.PrimaryPhoneCountryCode)
            .MaximumLength(5).WithMessage("Country code must not exceed 5 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.PrimaryPhoneCountryCode));

        RuleFor(x => x.AddressLine1)
            .MaximumLength(200).WithMessage("Address line 1 must not exceed 200 characters.");

        RuleFor(x => x.AddressLine2)
            .MaximumLength(200).WithMessage("Address line 2 must not exceed 200 characters.");

        RuleFor(x => x.City)
            .MaximumLength(100).WithMessage("City must not exceed 100 characters.");

        RuleFor(x => x.State)
            .MaximumLength(100).WithMessage("State must not exceed 100 characters.");

        RuleFor(x => x.Country)
            .MaximumLength(100).WithMessage("Country must not exceed 100 characters.");

        RuleFor(x => x.PostalCode)
            .MaximumLength(20).WithMessage("Postal code must not exceed 20 characters.");

        RuleFor(x => x.PrimaryPhoneNumber)
            .NotEmpty().WithMessage("Phone number is required when address is provided.")
            .When(x => !string.IsNullOrWhiteSpace(x.AddressLine1) || !string.IsNullOrWhiteSpace(x.City));
    }
}
