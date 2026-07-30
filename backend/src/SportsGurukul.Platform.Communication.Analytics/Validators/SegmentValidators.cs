using FluentValidation;
using SportsGurukul.Platform.Communication.Analytics.DTOs;

namespace SportsGurukul.Platform.Communication.Analytics.Validators;

public class SegmentRequestValidator : AbstractValidator<SegmentRequest>
{
    public SegmentRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty();

        RuleFor(x => x.Type)
            .IsInEnum();
    }
}

public class SegmentFilterValidator : AbstractValidator<SegmentFilterDto>
{
    private static readonly HashSet<string> ValidOperators =
    [
        "eq", "neq", "gt", "gte", "lt", "lte",
        "contains", "not_contains", "starts_with", "ends_with",
        "in", "not_in", "between", "is_null", "is_not_null"
    ];

    public SegmentFilterValidator()
    {
        RuleFor(x => x.Field)
            .NotEmpty();

        RuleFor(x => x.Operator)
            .NotEmpty()
            .Must(op => ValidOperators.Contains(op))
            .WithMessage($"Operator must be one of: {string.Join(", ", ValidOperators)}");
    }
}

public class SegmentPreviewRequestValidator : AbstractValidator<SegmentPreviewRequest>
{
    public SegmentPreviewRequestValidator()
    {
        RuleFor(x => x.Filters)
            .NotEmpty()
            .WithMessage("At least one filter is required.");
    }
}

public class SegmentSearchCriteriaValidator : AbstractValidator<SegmentSearchCriteria>
{
    public SegmentSearchCriteriaValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100);
    }
}
