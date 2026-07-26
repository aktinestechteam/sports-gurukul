using FluentValidation;
using SportsGurukul.Application.Features.FacilityManagement.Commands.AddEquipment;

namespace SportsGurukul.Application.Features.FacilityManagement.Validators;

public class AddEquipmentValidator : AbstractValidator<AddEquipmentCommand>
{
    public AddEquipmentValidator()
    {
        RuleFor(x => x.FacilityId)
            .NotEmpty().WithMessage("Facility ID is required.");

        RuleFor(x => x.EquipmentName)
            .NotEmpty().WithMessage("Equipment name is required.")
            .MaximumLength(200).WithMessage("Equipment name must not exceed 200 characters.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0.");
    }
}
