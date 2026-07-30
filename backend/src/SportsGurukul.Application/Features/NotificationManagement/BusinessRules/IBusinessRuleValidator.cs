using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.NotificationManagement.BusinessRules;

public interface IBusinessRuleValidator
{
    Task<Result<bool>> ValidateAsync<T>(T request, CancellationToken cancellationToken = default);
}
