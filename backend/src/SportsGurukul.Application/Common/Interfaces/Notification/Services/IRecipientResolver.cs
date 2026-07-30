using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Common.Interfaces.Notification.Services;

public interface IRecipientResolver
{
    Task<Result<List<ResolvedRecipient>>> ResolveAsync(
        Guid? userId,
        string channelType,
        string? destinationAddress,
        CancellationToken cancellationToken = default);

    Task<Result<List<ResolvedRecipient>>> ResolveByCriteriaAsync(
        string targetCriteria,
        CancellationToken cancellationToken = default);
}

public record ResolvedRecipient(
    Guid? UserId,
    string ChannelType,
    string DestinationAddress,
    string? RecipientName
);
