using FluentValidation;
using SportsGurukul.Application.Features.FacilityManagement.Commands.ScheduleMaintenance;

namespace SportsGurukul.Application.Features.FacilityManagement.Validators;

public class ScheduleMaintenanceValidator : AbstractValidator<ScheduleMaintenanceCommand>
{
    public ScheduleMaintenanceValidator()
    {
        RuleFor(x => x.EquipmentId)
            .NotEmpty().WithMessage("Equipment ID is required.");

        RuleFor(x => x.ScheduledDate)
            .NotEmpty().WithMessage("Scheduled date is required.")
            .GreaterThanOrEqualTo(DateTime.UtcNow.AddDays(-1)).WithMessage("Scheduled date cannot be in the past.");

        RuleFor(x => x.MaintenanceType)
            .NotEmpty().WithMessage("Maintenance type is required.")
            .MaximumLength(100).WithMessage("Maintenance type must not exceed 100 characters.");
    }
}
