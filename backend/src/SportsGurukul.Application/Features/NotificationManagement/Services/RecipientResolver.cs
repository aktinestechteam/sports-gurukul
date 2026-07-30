using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.NotificationManagement.Services;

public class RecipientResolver : IRecipientResolver
{
    private readonly ILogger<RecipientResolver> _logger;

    public RecipientResolver(ILogger<RecipientResolver> logger)
    {
        _logger = logger;
    }

    public Task<Result<List<ResolvedRecipient>>> ResolveAsync(
        Guid? userId,
        string channelType,
        string? destinationAddress,
        CancellationToken cancellationToken = default)
    {
        if (userId.HasValue)
        {
            _logger.LogInformation("Resolving recipient for user {UserId} via {ChannelType}", userId, channelType);
        }

        if (!string.IsNullOrEmpty(destinationAddress))
        {
            var resolved = new List<ResolvedRecipient>
            {
                new(userId, channelType, destinationAddress, null)
            };
            return Task.FromResult(Result<List<ResolvedRecipient>>.Success(resolved));
        }

        return Task.FromResult(Result<List<ResolvedRecipient>>.Failure("No resolution criteria provided"));
    }

    public Task<Result<List<ResolvedRecipient>>> ResolveByCriteriaAsync(
        string targetCriteria,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Resolving recipients by criteria: {TargetCriteria}", targetCriteria);
        return Task.FromResult(Result<List<ResolvedRecipient>>.Success(new List<ResolvedRecipient>()));
    }
}
