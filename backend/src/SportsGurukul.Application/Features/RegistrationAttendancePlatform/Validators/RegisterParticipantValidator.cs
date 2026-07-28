using FluentValidation;
using SportsGurukul.Application.Features.RegistrationAttendancePlatform.Commands.RegisterParticipant;

namespace SportsGurukul.Application.Features.RegistrationAttendancePlatform.Validators;

public class RegisterParticipantValidator : AbstractValidator<RegisterParticipantCommand>
{
    public RegisterParticipantValidator()
    {
        RuleFor(x => x.ProgramId)
            .NotEmpty().WithMessage("Program ID is required.");
        RuleFor(x => x.AthleteId)
            .NotEmpty().When(x => x.UserId == null).WithMessage("Either Athlete ID or User ID is required.");
        RuleFor(x => x.UserId)
            .NotEmpty().When(x => x.AthleteId == null).WithMessage("Either Athlete ID or User ID is required.");
    }
}
