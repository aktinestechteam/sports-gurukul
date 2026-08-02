using MediatR;

namespace SportsGurukul.Platform.AI.Events;

public interface IDomainEventPublisher
{
    Task PublishAsync(INotification @event, CancellationToken cancellationToken = default);
}
