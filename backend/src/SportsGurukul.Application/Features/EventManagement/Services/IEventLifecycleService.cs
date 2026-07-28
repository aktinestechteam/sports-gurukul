using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.EventManagement.Services;

public interface IEventLifecycleService
{
    Task<string> GenerateEventCodeAsync(CancellationToken cancellationToken = default);
    Task<EventStatus> ValidateStateTransitionAsync(EventStatus current, EventStatus target, CancellationToken cancellationToken = default);
    Task<bool> CanPublishAsync(Event evt, CancellationToken cancellationToken = default);
    Task<bool> CanStartAsync(Event evt, CancellationToken cancellationToken = default);
    Task<bool> CanCompleteAsync(Event evt, CancellationToken cancellationToken = default);
    Task<bool> CanArchiveAsync(Event evt, CancellationToken cancellationToken = default);
    Task<bool> CanCancelAsync(Event evt, CancellationToken cancellationToken = default);
}
