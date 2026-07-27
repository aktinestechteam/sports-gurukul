using FluentValidation;
using SportsGurukul.Application.Features.TournamentManagement.Commands.UpdateTournament;

namespace SportsGurukul.Application.Features.TournamentManagement.Validators;

public class UpdateTournamentCommandValidator : AbstractValidator<UpdateTournamentCommand>
{
    public UpdateTournamentCommandValidator()
    {
        RuleFor(x => x.TournamentId)
            .NotEmpty().WithMessage("Tournament ID is required.");

        RuleFor(x => x.TournamentName)
            .MaximumLength(200).WithMessage("Tournament name must not exceed 200 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.TournamentName));

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters.");

        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate).WithMessage("End date must be after start date.")
            .When(x => x.StartDate.HasValue && x.EndDate.HasValue);

        RuleFor(x => x.MaxParticipants)
            .GreaterThan(0).WithMessage("Max participants must be greater than 0.")
            .When(x => x.MaxParticipants.HasValue);

        RuleFor(x => x.RegistrationFee)
            .GreaterThanOrEqualTo(0).WithMessage("Registration fee cannot be negative.")
            .When(x => x.RegistrationFee.HasValue);
    }
}
