using FluentValidation;
using SportsGurukul.Application.Features.EventManagement.Commands.ApproveFeedback;

namespace SportsGurukul.Application.Features.EventManagement.Validators;

public class ApproveFeedbackValidator : AbstractValidator<ApproveFeedbackCommand>
{
    public ApproveFeedbackValidator()
    {
        RuleFor(x => x.FeedbackId)
            .NotEmpty().WithMessage("Feedback ID is required.");
    }
}
