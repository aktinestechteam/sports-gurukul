using FluentValidation;
using SportsGurukul.Application.Features.CoachManagement.Commands.DeleteEducation;

namespace SportsGurukul.Application.Features.CoachManagement.Validators;

public class DeleteEducationValidator : AbstractValidator<DeleteEducationCommand>
{
    public DeleteEducationValidator()
    {
        RuleFor(x => x.EducationId)
            .NotEmpty().WithMessage("Education ID is required.");
    }
}
