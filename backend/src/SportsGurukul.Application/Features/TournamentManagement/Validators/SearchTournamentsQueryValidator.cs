using FluentValidation;
using SportsGurukul.Application.Features.TournamentManagement.Queries.SearchTournaments;

namespace SportsGurukul.Application.Features.TournamentManagement.Validators;

public class SearchTournamentsQueryValidator : AbstractValidator<SearchTournamentsQuery>
{
    public SearchTournamentsQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Page must be greater than 0.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100.");
    }
}
