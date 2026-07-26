using FluentValidation;
using SportsGurukul.Application.Features.AcademySearchDiscovery.Queries.AdvancedSearchAcademies;

namespace SportsGurukul.Application.Features.AcademySearchDiscovery.Validators;

public class AdvancedSearchAcademiesValidator : AbstractValidator<AdvancedSearchAcademiesQuery>
{
    private static readonly string[] AllowedSortFields =
        ["nearest", "highestrated", "mostpopular", "newest", "alphabetical", "lowestmembershipcost", "mostcoaches"];

    public AdvancedSearchAcademiesValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1).WithMessage("Page must be at least 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("PageSize must be between 1 and 100.");

        RuleFor(x => x.RadiusKm)
            .GreaterThan(0).WithMessage("RadiusKm must be greater than 0.")
            .When(x => x.RadiusKm.HasValue);

        RuleFor(x => x.MinMembershipPrice)
            .GreaterThanOrEqualTo(0).WithMessage("MinMembershipPrice must be non-negative.")
            .When(x => x.MinMembershipPrice.HasValue);

        RuleFor(x => x.MaxMembershipPrice)
            .GreaterThanOrEqualTo(x => x.MinMembershipPrice ?? 0)
            .WithMessage("MaxMembershipPrice must be greater than or equal to MinMembershipPrice.")
            .When(x => x.MaxMembershipPrice.HasValue && x.MinMembershipPrice.HasValue);

        RuleFor(x => x.MinRating)
            .InclusiveBetween(0, 5).WithMessage("MinRating must be between 0 and 5.")
            .When(x => x.MinRating.HasValue);

        RuleFor(x => x.SearchTerm)
            .MaximumLength(200).WithMessage("SearchTerm must not exceed 200 characters.");

        RuleFor(x => x.Name)
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

        RuleFor(x => x.AcademyCode)
            .MaximumLength(50).WithMessage("AcademyCode must not exceed 50 characters.");

        RuleFor(x => x.SortBy)
            .Must(x => x == null || AllowedSortFields.Contains(x!.ToLower()))
            .WithMessage($"SortBy must be one of: {string.Join(", ", AllowedSortFields)}.");
    }
}
