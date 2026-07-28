using FluentValidation;
using SportsGurukul.Application.Features.RegistrationAttendancePlatform.Queries.GetRegistrationStatus;

namespace SportsGurukul.Application.Features.RegistrationAttendancePlatform.Validators;

public class GetRegistrationStatusValidator : AbstractValidator<GetRegistrationStatusQuery>
{
    public GetRegistrationStatusValidator()
    {
        RuleFor(x => x.RegistrationId)
            .NotEmpty().WithMessage("Registration ID is required.");
    }
}
