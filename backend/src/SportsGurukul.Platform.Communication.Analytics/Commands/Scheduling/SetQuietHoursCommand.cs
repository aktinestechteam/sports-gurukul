using MediatR;
using SportsGurukul.Platform.Communication.Analytics.Abstractions;
using SportsGurukul.Platform.Communication.Analytics.DTOs;

namespace SportsGurukul.Platform.Communication.Analytics.Commands.Scheduling;

public record SetQuietHoursCommand(QuietHoursDto QuietHours) : IRequest<QuietHoursDto>;

public class SetQuietHoursCommandHandler(ISchedulingEngine engine) : IRequestHandler<SetQuietHoursCommand, QuietHoursDto>
{
    public Task<QuietHoursDto> Handle(SetQuietHoursCommand command, CancellationToken ct)
        => engine.SetQuietHoursAsync(command.QuietHours, ct);
}
