using FluentValidation;
using SportsGurukul.Platform.Communication.Analytics.DTOs;

namespace SportsGurukul.Platform.Communication.Analytics.Validators;

public class CreateCampaignFullRequestValidator : AbstractValidator<CreateCampaignFullRequest>
{
    public CreateCampaignFullRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty();

        RuleFor(x => x.CampaignType)
            .IsInEnum();

        RuleFor(x => x.TemplateId)
            .NotEmpty()
            .Unless(x => x.CampaignType == CampaignType.Triggered);
    }
}

public class UpdateCampaignRequestValidator : AbstractValidator<UpdateCampaignRequest>
{
    public UpdateCampaignRequestValidator()
    {
        RuleFor(x => x)
            .Must(x => x.Name is not null
                    || x.Description is not null
                    || x.CampaignType is not null
                    || x.TemplateId is not null
                    || x.ChannelType is not null
                    || x.Schedule is not null
                    || x.Audience is not null
                    || x.Metadata is not null)
            .WithMessage("At least one property must be set for update.");
    }
}

public class CampaignCloneRequestValidator : AbstractValidator<CampaignCloneRequest>
{
    public CampaignCloneRequestValidator()
    {
        RuleFor(x => x.NewName)
            .NotEmpty();
    }
}

public class CampaignSearchCriteriaValidator : AbstractValidator<CampaignSearchCriteria>
{
    public CampaignSearchCriteriaValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100);
    }
}

public class ScheduleDefinitionDtoValidator : AbstractValidator<ScheduleDefinitionDto>
{
    public ScheduleDefinitionDtoValidator()
    {
        RuleFor(x => x.CronExpression)
            .Must(BeValidCronExpression)
            .When(x => x.CronExpression is not null)
            .WithMessage("Cron expression is not in a valid format.");

        When(x => x.Pattern is not null, () =>
        {
            RuleFor(x => x.StartDate)
                .NotNull()
                .WithMessage("Start date is required when recurrence pattern is specified.");

            RuleFor(x => x.Pattern)
                .NotNull()
                .WithMessage("Recurrence pattern is required when start date is specified.");
        });

        When(x => x.StartDate is not null && x.EndDate is not null, () =>
        {
            RuleFor(x => x.EndDate)
                .GreaterThan(x => x.StartDate!.Value)
                .WithMessage("End date must be after start date.");
        });
    }

    private static bool BeValidCronExpression(string? expression)
    {
        if (string.IsNullOrWhiteSpace(expression))
            return false;

        var parts = expression.Split(' ');
        return parts.Length is 5 or 6;
    }
}

public class AudienceDefinitionDtoValidator : AbstractValidator<AudienceDefinitionDto>
{
    public AudienceDefinitionDtoValidator()
    {
        RuleFor(x => x)
            .Must(x => x.SegmentIds is { Count: > 0 }
                    || x.UserIds is { Count: > 0 }
                    || x.RoleNames is { Count: > 0 }
                    || x.TagFilters is { Count: > 0 }
                    || !string.IsNullOrWhiteSpace(x.CustomQuery)
                    || x.IncludeAllUsers
                    || x.DynamicFilters is { Count: > 0 })
            .WithMessage("At least one audience criteria must be provided.");
    }
}
