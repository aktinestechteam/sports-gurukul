using FluentValidation;
using SportsGurukul.Application.Features.EventManagement.Commands.RejectFeedback;

namespace SportsGurukul.Application.Features.EventManagement.Validators;

public class RejectFeedbackValidator : AbstractValidator<RejectFeedbackCommand>
{
    public RejectFeedbackValidator()
    {
        RuleFor(x => x.FeedbackId)
            .NotEmpty().WithMessage("Feedback ID is required.");
    }
}
