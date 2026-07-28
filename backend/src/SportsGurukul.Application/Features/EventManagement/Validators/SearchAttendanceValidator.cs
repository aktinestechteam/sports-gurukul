using FluentValidation;
using SportsGurukul.Application.Features.EventManagement.Commands.SearchAttendance;

namespace SportsGurukul.Application.Features.EventManagement.Validators;

public class SearchAttendanceValidator : AbstractValidator<SearchAttendanceQuery>
{
    public SearchAttendanceValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty().WithMessage("Event ID is required.");

        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1).WithMessage("Page must be at least 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100.");
    }
}
