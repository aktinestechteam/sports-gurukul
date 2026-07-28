using FluentValidation;
using SportsGurukul.Application.Features.RegistrationAttendancePlatform.Commands.RejectRegistration;

namespace SportsGurukul.Application.Features.RegistrationAttendancePlatform.Validators;

public class RejectRegistrationValidator : AbstractValidator<RejectRegistrationCommand>
{
    public RejectRegistrationValidator()
    {
        RuleFor(x => x.RegistrationId)
            .NotEmpty().WithMessage("Registration ID is required.");
        RuleFor(x => x.Reason)
            .MaximumLength(500).WithMessage("Rejection reason must not exceed 500 characters.");
    }
}
