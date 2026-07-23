using FluentValidation;
using SportsGurukul.Application.Features.UserManagement.Queries.GetPagedUsers;

namespace SportsGurukul.Application.Features.UserManagement.Validators;

public class GetPagedUsersValidator : AbstractValidator<GetPagedUsersQuery>
{
    private static readonly string[] AllowedSortFields =
        ["name", "email", "role", "createddate", "updateddate"];

    public GetPagedUsersValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1).WithMessage("Page must be at least 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("PageSize must be between 1 and 100.");

        RuleFor(x => x.SortBy)
            .Must(x => x == null || AllowedSortFields.Contains(x!.ToLower()))
            .WithMessage($"SortBy must be one of: {string.Join(", ", AllowedSortFields)}.");
    }
}
