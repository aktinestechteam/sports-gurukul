using FluentValidation;
using SportsGurukul.Application.Features.LiveScoringManagement.Queries.MatchStatistics;

namespace SportsGurukul.Application.Features.LiveScoringManagement.Validators;

public class MatchStatisticsQueryValidator : AbstractValidator<MatchStatisticsQuery>
{
    public MatchStatisticsQueryValidator()
    {
        RuleFor(x => x.MatchId)
            .NotEmpty().WithMessage("Match ID is required.");
    }
}
