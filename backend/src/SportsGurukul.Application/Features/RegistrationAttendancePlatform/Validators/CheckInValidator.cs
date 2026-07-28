using FluentValidation;
using SportsGurukul.Application.Features.RegistrationAttendancePlatform.Commands.CheckIn;

namespace SportsGurukul.Application.Features.RegistrationAttendancePlatform.Validators;

public class CheckInValidator : AbstractValidator<CheckInCommand>
{
    public CheckInValidator()
    {
        RuleFor(x => x.ProgramId)
            .NotEmpty().WithMessage("Program ID is required.");
        RuleFor(x => x.ParticipantId)
            .NotEmpty().WithMessage("Participant ID is required.");
    }
}
