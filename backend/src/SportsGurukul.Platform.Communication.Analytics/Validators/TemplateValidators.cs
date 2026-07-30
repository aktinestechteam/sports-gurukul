using FluentValidation;
using SportsGurukul.Platform.Communication.Analytics.DTOs;

namespace SportsGurukul.Platform.Communication.Analytics.Validators;

public class CreateTemplateFullRequestValidator : AbstractValidator<CreateTemplateFullRequest>
{
    public CreateTemplateFullRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.ChannelType)
            .IsInEnum();

        RuleFor(x => x.Category)
            .IsInEnum();

        RuleFor(x => x.SubjectTemplate)
            .NotEmpty();

        RuleFor(x => x.BodyTemplate)
            .NotEmpty();
    }
}

public class UpdateTemplateFullRequestValidator : AbstractValidator<UpdateTemplateFullRequest>
{
    public UpdateTemplateFullRequestValidator()
    {
        RuleFor(x => x)
            .Must(x => x.Name is not null
                    || x.Description is not null
                    || x.Category is not null
                    || x.SubjectTemplate is not null
                    || x.BodyTemplate is not null
                    || x.Variables is not null
                    || x.Localizations is not null
                    || x.PartialNames is not null
                    || x.Attachments is not null
                    || x.Metadata is not null)
            .WithMessage("At least one property must be set for update.");
    }
}

public class CloneTemplateRequestValidator : AbstractValidator<CloneTemplateRequest>
{
    public CloneTemplateRequestValidator()
    {
        RuleFor(x => x.NewName)
            .NotEmpty()
            .MaximumLength(200);
    }
}

public class RollbackTemplateRequestValidator : AbstractValidator<RollbackTemplateRequest>
{
    public RollbackTemplateRequestValidator()
    {
        RuleFor(x => x.TargetVersion)
            .GreaterThanOrEqualTo(1);
    }
}

public class CreateLocalizationRequestValidator : AbstractValidator<CreateLocalizationRequest>
{
    public CreateLocalizationRequestValidator()
    {
        RuleFor(x => x.Locale)
            .NotEmpty()
            .Matches(@"^[a-z]{2}(-[A-Z]{2})?$")
            .WithMessage("Locale must be in format 'xx' or 'xx-XX'.");
    }
}

public class TemplateSearchCriteriaValidator : AbstractValidator<TemplateSearchCriteria>
{
    public TemplateSearchCriteriaValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100);
    }
}
