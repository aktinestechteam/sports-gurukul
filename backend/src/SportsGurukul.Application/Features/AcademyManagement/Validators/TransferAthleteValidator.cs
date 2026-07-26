using FluentValidation;
using SportsGurukul.Application.Features.AcademyManagement.Commands.TransferAthlete;

namespace SportsGurukul.Application.Features.AcademyManagement.Validators;

public class TransferAthleteValidator : AbstractValidator<TransferAthleteCommand>
{
    public TransferAthleteValidator()
    {
        RuleFor(x => x.FromAcademyId)
            .NotEmpty().WithMessage("Source academy ID is required.");

        RuleFor(x => x.ToAcademyId)
            .NotEmpty().WithMessage("Destination academy ID is required.");

        RuleFor(x => x.AthleteId)
            .NotEmpty().WithMessage("Athlete ID is required.");

        RuleFor(x => x)
            .Must(x => x.FromAcademyId != x.ToAcademyId)
            .WithMessage("Source and destination academy IDs must be different.");
    }
}
