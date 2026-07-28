using FluentValidation;
using SportsGurukul.Application.Features.RegistrationAttendancePlatform.Queries.GetCapacityInfo;

namespace SportsGurukul.Application.Features.RegistrationAttendancePlatform.Validators;

public class GetCapacityInfoValidator : AbstractValidator<GetCapacityInfoQuery>
{
    public GetCapacityInfoValidator()
    {
        RuleFor(x => x.ProgramId)
            .NotEmpty().WithMessage("Program ID is required.");
    }
}
