using FluentValidation;
using SportsGurukul.Application.Features.RegistrationAttendancePlatform.Queries.GetWaitlistPosition;

namespace SportsGurukul.Application.Features.RegistrationAttendancePlatform.Validators;

public class GetWaitlistPositionValidator : AbstractValidator<GetWaitlistPositionQuery>
{
    public GetWaitlistPositionValidator()
    {
        RuleFor(x => x.ProgramId)
            .NotEmpty().WithMessage("Program ID is required.");
        RuleFor(x => x.ParticipantId)
            .NotEmpty().WithMessage("Participant ID is required.");
    }
}
