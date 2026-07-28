using FluentValidation;
using SportsGurukul.Application.Features.RegistrationAttendancePlatform.Queries.GetAttendanceRecord;

namespace SportsGurukul.Application.Features.RegistrationAttendancePlatform.Validators;

public class GetAttendanceRecordValidator : AbstractValidator<GetAttendanceRecordQuery>
{
    public GetAttendanceRecordValidator()
    {
        RuleFor(x => x.ParticipantId)
            .NotEmpty().WithMessage("Participant ID is required.");
        RuleFor(x => x.ProgramId)
            .NotEmpty().WithMessage("Program ID is required.");
    }
}
