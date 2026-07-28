using FluentValidation;
using SportsGurukul.Application.Features.EventManagement.Queries.GetEventStatistics;

namespace SportsGurukul.Application.Features.EventManagement.Validators;

public class GetEventStatisticsValidator : AbstractValidator<GetEventStatisticsQuery>
{
    public GetEventStatisticsValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty().WithMessage("Event ID is required.");
    }
}
