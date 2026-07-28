using FluentValidation;
using SportsGurukul.Application.Features.RegistrationAttendancePlatform.Commands.PromoteWaitlist;

namespace SportsGurukul.Application.Features.RegistrationAttendancePlatform.Validators;

public class PromoteWaitlistValidator : AbstractValidator<PromoteWaitlistCommand>
{
    public PromoteWaitlistValidator()
    {
        RuleFor(x => x.ProgramId)
            .NotEmpty().WithMessage("Program ID is required.");
    }
}
