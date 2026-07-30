using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.BusinessRules.Rules;

namespace SportsGurukul.Application.Features.NotificationManagement.BusinessRules;

public class BusinessRuleValidator : IBusinessRuleValidator
{
    private readonly IEnumerable<IBusinessRule> _rules;
    private readonly ILogger<BusinessRuleValidator> _logger;

    public BusinessRuleValidator(
        IEnumerable<IBusinessRule> rules,
        ILogger<BusinessRuleValidator> logger)
    {
        _rules = rules;
        _logger = logger;
    }

    public async Task<Result<bool>> ValidateAsync<T>(T request, CancellationToken cancellationToken = default)
    {
        var errors = new List<string>();

        foreach (var rule in _rules)
        {
            var result = await rule.ValidateAsync(request, cancellationToken);
            if (!result.IsSuccess)
                errors.AddRange(result.Errors);
        }

        if (errors.Count > 0)
        {
            _logger.LogWarning("Business rule validation failed: {Errors}", string.Join("; ", errors));
            return Result<bool>.Failure(errors);
        }

        return Result<bool>.Success(true);
    }
}
