using FluentValidation;
using SportsGurukul.Application.Features.TournamentManagement.Commands.CreateTournament;

namespace SportsGurukul.Application.Features.TournamentManagement.Validators;

public class CreateTournamentCommandValidator : AbstractValidator<CreateTournamentCommand>
{
    public CreateTournamentCommandValidator()
    {
        RuleFor(x => x.TournamentName)
            .NotEmpty().WithMessage("Tournament name is required.")
            .MaximumLength(200).WithMessage("Tournament name must not exceed 200 characters.");

        RuleFor(x => x.AcademyId)
            .NotEmpty().WithMessage("Academy ID is required.");

        RuleFor(x => x.SportId)
            .NotEmpty().WithMessage("Sport ID is required.");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Start date is required.")
            .GreaterThan(DateTime.UtcNow).WithMessage("Start date must be in the future.");

        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("End date is required.")
            .GreaterThan(x => x.StartDate).WithMessage("End date must be after start date.");

        RuleFor(x => x.RegistrationOpenDate)
            .NotEmpty().WithMessage("Registration open date is required.");

        RuleFor(x => x.RegistrationCloseDate)
            .NotEmpty().WithMessage("Registration close date is required.")
            .LessThan(x => x.StartDate).WithMessage("Registration must close before tournament starts.");

        RuleFor(x => x.MaxParticipants)
            .GreaterThan(0).WithMessage("Max participants must be greater than 0.")
            .When(x => x.MaxParticipants.HasValue);

        RuleFor(x => x.MinParticipants)
            .GreaterThan(0).WithMessage("Min participants must be greater than 0.")
            .When(x => x.MinParticipants.HasValue);

        RuleFor(x => x.RegistrationFee)
            .GreaterThanOrEqualTo(0).WithMessage("Registration fee cannot be negative.")
            .When(x => x.RegistrationFee.HasValue);

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters.");

        RuleFor(x => x.ContactEmail)
            .EmailAddress().WithMessage("Invalid email address.")
            .When(x => !string.IsNullOrWhiteSpace(x.ContactEmail));

        RuleFor(x => x.ContactPhone)
            .MaximumLength(20).WithMessage("Contact phone must not exceed 20 characters.");
    }
}
