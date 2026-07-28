using FluentValidation;
using SportsGurukul.Application.Features.RegistrationAttendancePlatform.Commands.ApproveRegistration;

namespace SportsGurukul.Application.Features.RegistrationAttendancePlatform.Validators;

public class ApproveRegistrationValidator : AbstractValidator<ApproveRegistrationCommand>
{
    public ApproveRegistrationValidator()
    {
        RuleFor(x => x.RegistrationId)
            .NotEmpty().WithMessage("Registration ID is required.");
    }
}
