using FluentValidation;
using SportsGurukul.Application.Features.AthleteManagement.Queries.AdvancedSearchAthletes;

namespace SportsGurukul.Application.Features.AthleteManagement.Validators;

public class AdvancedSearchAthletesValidator : AbstractValidator<AdvancedSearchAthletesQuery>
{
    private static readonly string[] AllowedSortFields =
        ["name", "athletecode", "level", "experience", "createddate", "updateddate",
         "ranking", "achievementcount", "recentlyupdated", "newest", "oldest"];

    public AdvancedSearchAthletesValidator()
    {
        RuleFor(x => x.SearchTerm)
            .MaximumLength(200).WithMessage("Search term must not exceed 200 characters.");

        RuleFor(x => x.Name)
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

        RuleFor(x => x.AthleteCode)
            .MaximumLength(50).WithMessage("Athlete code must not exceed 50 characters.");

        RuleFor(x => x.Email)
            .MaximumLength(200).WithMessage("Email must not exceed 200 characters.");

        RuleFor(x => x.Mobile)
            .MaximumLength(20).WithMessage("Mobile must not exceed 20 characters.");

        RuleFor(x => x.SportName)
            .MaximumLength(100).WithMessage("Sport name must not exceed 100 characters.");

        RuleFor(x => x.SportCategory)
            .MaximumLength(100).WithMessage("Sport category must not exceed 100 characters.");

        RuleFor(x => x.City)
            .MaximumLength(100).WithMessage("City must not exceed 100 characters.");

        RuleFor(x => x.State)
            .MaximumLength(100).WithMessage("State must not exceed 100 characters.");

        RuleFor(x => x.Country)
            .MaximumLength(100).WithMessage("Country must not exceed 100 characters.");

        RuleFor(x => x.District)
            .MaximumLength(100).WithMessage("District must not exceed 100 characters.");

        RuleFor(x => x.PostalCode)
            .MaximumLength(20).WithMessage("Postal code must not exceed 20 characters.");

        RuleFor(x => x.Ranking)
            .MaximumLength(50).WithMessage("Ranking must not exceed 50 characters.");

        RuleFor(x => x.StateRank)
            .MaximumLength(50).WithMessage("State rank must not exceed 50 characters.");

        RuleFor(x => x.NationalRank)
            .MaximumLength(50).WithMessage("National rank must not exceed 50 characters.");

        RuleFor(x => x.InternationalRank)
            .MaximumLength(50).WithMessage("International rank must not exceed 50 characters.");

        RuleFor(x => x.MinHeight)
            .MaximumLength(20).WithMessage("Min height must not exceed 20 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.MinHeight));

        RuleFor(x => x.MaxHeight)
            .MaximumLength(20).WithMessage("Max height must not exceed 20 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.MaxHeight));

        RuleFor(x => x.MinWeight)
            .MaximumLength(20).WithMessage("Min weight must not exceed 20 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.MinWeight));

        RuleFor(x => x.MaxWeight)
            .MaximumLength(20).WithMessage("Max weight must not exceed 20 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.MaxWeight));

        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1).WithMessage("Page must be at least 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("PageSize must be between 1 and 100.");

        RuleFor(x => x.SortBy)
            .Must(x => x == null || AllowedSortFields.Contains(x!.ToLower()))
            .WithMessage($"SortBy must be one of: {string.Join(", ", AllowedSortFields)}.");

        RuleFor(x => x.MinAge)
            .GreaterThanOrEqualTo(0).WithMessage("Min age must be non-negative.")
            .LessThanOrEqualTo(120).WithMessage("Min age must be 120 or less.")
            .When(x => x.MinAge.HasValue);

        RuleFor(x => x.MaxAge)
            .GreaterThanOrEqualTo(0).WithMessage("Max age must be non-negative.")
            .LessThanOrEqualTo(120).WithMessage("Max age must be 120 or less.")
            .When(x => x.MaxAge.HasValue);

        RuleFor(x => x.MinExperience)
            .GreaterThanOrEqualTo(0).WithMessage("Min experience must be non-negative.")
            .When(x => x.MinExperience.HasValue);

        RuleFor(x => x.MaxExperience)
            .GreaterThanOrEqualTo(0).WithMessage("Max experience must be non-negative.")
            .When(x => x.MaxExperience.HasValue);

        RuleFor(x => x.CreatedFrom)
            .LessThanOrEqualTo(x => x.CreatedTo)
            .WithMessage("CreatedFrom must be before or equal to CreatedTo.")
            .When(x => x.CreatedFrom.HasValue && x.CreatedTo.HasValue);

        RuleFor(x => x.SportIds)
            .Must(ids => ids == null || ids.Count > 0)
            .WithMessage("Sport IDs list cannot be empty.")
            .When(x => x.SportIds is not null);

        RuleFor(x => x.SportIds)
            .Must(ids => ids == null || ids.Count <= 20)
            .WithMessage("Cannot filter by more than 20 sports at once.")
            .When(x => x.SportIds is not null);
    }
}
