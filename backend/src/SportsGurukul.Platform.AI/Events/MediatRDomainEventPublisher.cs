using MediatR;
using Microsoft.Extensions.Logging;

namespace SportsGurukul.Platform.AI.Events;

public class MediatRDomainEventPublisher : IDomainEventPublisher
{
    private readonly IPublisher _publisher;
    private readonly ILogger<MediatRDomainEventPublisher> _logger;

    public MediatRDomainEventPublisher(IPublisher publisher, ILogger<MediatRDomainEventPublisher>? logger = null)
    {
        _publisher = publisher;
        _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<MediatRDomainEventPublisher>.Instance;
    }

    public async Task PublishAsync(INotification @event, CancellationToken cancellationToken = default)
    {
        try
        {
            await _publisher.Publish(@event, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish domain event '{EventType}'", @event.GetType().Name);
            throw;
        }
    }
}
