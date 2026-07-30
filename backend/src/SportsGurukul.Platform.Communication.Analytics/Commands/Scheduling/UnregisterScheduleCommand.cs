using MediatR;
using SportsGurukul.Platform.Communication.Analytics.Abstractions;

namespace SportsGurukul.Platform.Communication.Analytics.Commands.Scheduling;

public record UnregisterScheduleCommand(Guid JobId) : IRequest<bool>;

public class UnregisterScheduleCommandHandler(ISchedulingEngine engine) : IRequestHandler<UnregisterScheduleCommand, bool>
{
    public Task<bool> Handle(UnregisterScheduleCommand command, CancellationToken ct)
        => engine.UnregisterJobAsync(command.JobId, ct);
}
