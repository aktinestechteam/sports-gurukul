using FluentValidation;
using SportsGurukul.Application.Features.FacilityManagement.Commands.UpdateEquipment;

namespace SportsGurukul.Application.Features.FacilityManagement.Validators;

public class UpdateEquipmentValidator : AbstractValidator<UpdateEquipmentCommand>
{
    public UpdateEquipmentValidator()
    {
        RuleFor(x => x.EquipmentId)
            .NotEmpty().WithMessage("Equipment ID is required.");

        RuleFor(x => x.EquipmentName)
            .MaximumLength(200).WithMessage("Equipment name must not exceed 200 characters.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0.")
            .When(x => x.Quantity.HasValue);
    }
}
