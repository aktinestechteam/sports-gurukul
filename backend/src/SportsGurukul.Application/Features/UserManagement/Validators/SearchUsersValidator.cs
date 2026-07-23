using FluentValidation;
using SportsGurukul.Application.Features.UserManagement.Queries.SearchUsers;

namespace SportsGurukul.Application.Features.UserManagement.Validators;

public class SearchUsersValidator : AbstractValidator<SearchUsersQuery>
{
    private static readonly string[] AllowedSortFields =
        ["name", "email", "role", "createddate", "updateddate"];

    public SearchUsersValidator()
    {
        RuleFor(x => x.SearchTerm)
            .MaximumLength(200).WithMessage("Search term must not exceed 200 characters.");

        RuleFor(x => x.Name)
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

        RuleFor(x => x.Email)
            .MaximumLength(200).WithMessage("Email must not exceed 200 characters.");

        RuleFor(x => x.Mobile)
            .MaximumLength(20).WithMessage("Mobile must not exceed 20 characters.");

        RuleFor(x => x.City)
            .MaximumLength(100).WithMessage("City must not exceed 100 characters.");

        RuleFor(x => x.State)
            .MaximumLength(100).WithMessage("State must not exceed 100 characters.");

        RuleFor(x => x.Country)
            .MaximumLength(100).WithMessage("Country must not exceed 100 characters.");

        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1).WithMessage("Page must be at least 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("PageSize must be between 1 and 100.");

        RuleFor(x => x.SortBy)
            .Must(x => x == null || AllowedSortFields.Contains(x!.ToLower()))
            .WithMessage($"SortBy must be one of: {string.Join(", ", AllowedSortFields)}.");

        RuleFor(x => x.CreatedFrom)
            .LessThanOrEqualTo(x => x.CreatedTo)
            .WithMessage("CreatedFrom must be before or equal to CreatedTo.")
            .When(x => x.CreatedFrom.HasValue && x.CreatedTo.HasValue);

        RuleFor(x => x.UpdatedFrom)
            .LessThanOrEqualTo(x => x.UpdatedTo)
            .WithMessage("UpdatedFrom must be before or equal to UpdatedTo.")
            .When(x => x.UpdatedFrom.HasValue && x.UpdatedTo.HasValue);
    }
}
