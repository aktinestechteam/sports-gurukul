using FluentValidation;
using SportsGurukul.Application.Features.UserManagement.Queries.SearchUsers;

namespace SportsGurukul.Application.Features.UserManagement.Validators;

public class SearchUsersValidator : AbstractValidator<SearchUsersQuery>
{
    public SearchUsersValidator()
    {
        RuleFor(x => x.SearchTerm)
            .MaximumLength(200).WithMessage("Search term must not exceed 200 characters.");

        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1).WithMessage("Page must be at least 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100.");

        RuleFor(x => x.SortBy)
            .Must(x => x == null || new[] { "name", "email", "sport", "status", "createdat" }.Contains(x!.ToLower()))
            .WithMessage("Sort by must be one of: name, email, sport, status, createdat.");
    }
}
