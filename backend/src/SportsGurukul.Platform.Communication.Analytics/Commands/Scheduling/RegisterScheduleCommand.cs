using MediatR;
using SportsGurukul.Platform.Communication.Analytics.Abstractions;
using SportsGurukul.Platform.Communication.Analytics.DTOs;

namespace SportsGurukul.Platform.Communication.Analytics.Commands.Scheduling;

public record RegisterScheduleCommand(
    Guid CampaignId,
    ScheduleDefinitionDto Schedule
) : IRequest<ScheduleJobDto>;

public class RegisterScheduleCommandHandler(ISchedulingEngine engine) : IRequestHandler<RegisterScheduleCommand, ScheduleJobDto>
{
    public Task<ScheduleJobDto> Handle(RegisterScheduleCommand command, CancellationToken ct)
        => engine.RegisterJobAsync(command.CampaignId, command.Schedule, ct);
}
