using FluentValidation;
using SportsGurukul.Application.Features.FacilityManagement.Commands.UpdateFacilitySchedule;

namespace SportsGurukul.Application.Features.FacilityManagement.Validators;

public class UpdateFacilityScheduleValidator : AbstractValidator<UpdateFacilityScheduleCommand>
{
    public UpdateFacilityScheduleValidator()
    {
        RuleFor(x => x.FacilityId)
            .NotEmpty().WithMessage("Facility ID is required.");

        RuleFor(x => x.OpeningTime)
            .NotEmpty().WithMessage("Opening time is required.")
            .MaximumLength(10).WithMessage("Opening time must not exceed 10 characters.");

        RuleFor(x => x.ClosingTime)
            .NotEmpty().WithMessage("Closing time is required.")
            .MaximumLength(10).WithMessage("Closing time must not exceed 10 characters.");
    }
}
