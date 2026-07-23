using FluentValidation;
using SportsGurukul.Application.Features.AthleteManagement.Commands.UpdateRanking;

namespace SportsGurukul.Application.Features.AthleteManagement.Validators;

public class UpdateRankingValidator : AbstractValidator<UpdateRankingCommand>
{
    public UpdateRankingValidator()
    {
        RuleFor(x => x.AthleteId)
            .NotEmpty().WithMessage("Athlete ID is required.");

        RuleFor(x => x.CurrentRank)
            .MaximumLength(50).WithMessage("Current rank must not exceed 50 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.CurrentRank));

        RuleFor(x => x.StateRank)
            .MaximumLength(50).WithMessage("State rank must not exceed 50 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.StateRank));

        RuleFor(x => x.NationalRank)
            .MaximumLength(50).WithMessage("National rank must not exceed 50 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.NationalRank));

        RuleFor(x => x.InternationalRank)
            .MaximumLength(50).WithMessage("International rank must not exceed 50 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.InternationalRank));

        RuleFor(x => x.RankingAuthority)
            .MaximumLength(200).WithMessage("Ranking authority must not exceed 200 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.RankingAuthority));
    }
}
