using FluentValidation;
using SportsGurukul.Application.Features.TournamentManagement.Commands.RegisterParticipant;

namespace SportsGurukul.Application.Features.TournamentManagement.Validators;

public class RegisterParticipantCommandValidator : AbstractValidator<RegisterParticipantCommand>
{
    public RegisterParticipantCommandValidator()
    {
        RuleFor(x => x.TournamentId)
            .NotEmpty().WithMessage("Tournament ID is required.");

        RuleFor(x => x.RegistrantName)
            .NotEmpty().WithMessage("Registrant name is required.")
            .MaximumLength(200).WithMessage("Registrant name must not exceed 200 characters.");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Invalid email address.")
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.Phone)
            .MaximumLength(20).WithMessage("Phone must not exceed 20 characters.");

        RuleFor(x => x.AthleteId)
            .NotEmpty().WithMessage("Athlete ID is required.")
            .When(x => x.ParticipantType == Domain.Enums.TournamentParticipantType.Athlete);

        RuleFor(x => x.TeamId)
            .NotEmpty().WithMessage("Team ID is required.")
            .When(x => x.ParticipantType == Domain.Enums.TournamentParticipantType.Team);
    }
}
