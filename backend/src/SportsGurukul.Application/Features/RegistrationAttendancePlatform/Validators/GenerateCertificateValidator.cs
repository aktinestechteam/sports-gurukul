using FluentValidation;
using SportsGurukul.Application.Features.RegistrationAttendancePlatform.Commands.GenerateCertificate;

namespace SportsGurukul.Application.Features.RegistrationAttendancePlatform.Validators;

public class GenerateCertificateValidator : AbstractValidator<GenerateCertificateCommand>
{
    public GenerateCertificateValidator()
    {
        RuleFor(x => x.ProgramId)
            .NotEmpty().WithMessage("Program ID is required.");
        RuleFor(x => x.ParticipantId)
            .NotEmpty().WithMessage("Participant ID is required.");
        RuleFor(x => x.AttendanceRate)
            .InclusiveBetween(0, 100).WithMessage("Attendance rate must be between 0 and 100.");
    }
}
