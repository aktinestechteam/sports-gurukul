using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.NotificationManagement.BusinessRules.Rules;

public interface IBusinessRule
{
    Task<Result<bool>> ValidateAsync<T>(T request, CancellationToken cancellationToken = default);
}
