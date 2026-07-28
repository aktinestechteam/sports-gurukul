using FluentValidation;
using SportsGurukul.Application.Features.EventManagement.Queries.GetEventById;

namespace SportsGurukul.Application.Features.EventManagement.Validators;

public class GetEventByIdValidator : AbstractValidator<GetEventByIdQuery>
{
    public GetEventByIdValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty().WithMessage("Event ID is required.");
    }
}
