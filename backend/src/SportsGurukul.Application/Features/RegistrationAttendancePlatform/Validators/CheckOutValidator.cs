using FluentValidation;
using SportsGurukul.Application.Features.RegistrationAttendancePlatform.Commands.CheckOut;

namespace SportsGurukul.Application.Features.RegistrationAttendancePlatform.Validators;

public class CheckOutValidator : AbstractValidator<CheckOutCommand>
{
    public CheckOutValidator()
    {
        RuleFor(x => x.ProgramId)
            .NotEmpty().WithMessage("Program ID is required.");
        RuleFor(x => x.ParticipantId)
            .NotEmpty().WithMessage("Participant ID is required.");
    }
}
