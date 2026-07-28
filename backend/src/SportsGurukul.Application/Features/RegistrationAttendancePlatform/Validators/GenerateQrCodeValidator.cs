using FluentValidation;
using SportsGurukul.Application.Features.RegistrationAttendancePlatform.Commands.GenerateQrCode;

namespace SportsGurukul.Application.Features.RegistrationAttendancePlatform.Validators;

public class GenerateQrCodeValidator : AbstractValidator<GenerateQrCodeCommand>
{
    public GenerateQrCodeValidator()
    {
        RuleFor(x => x.ProgramId)
            .NotEmpty().WithMessage("Program ID is required.");
        RuleFor(x => x.ParticipantId)
            .NotEmpty().WithMessage("Participant ID is required.");
    }
}
