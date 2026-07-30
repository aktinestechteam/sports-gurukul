using MediatR;
using SportsGurukul.Platform.Communication.Analytics.Abstractions;
using SportsGurukul.Platform.Communication.Analytics.DTOs;

namespace SportsGurukul.Platform.Communication.Analytics.Commands.Scheduling;

public record SetHolidayCalendarCommand(HolidayCalendarDto Calendar) : IRequest<HolidayCalendarDto>;

public class SetHolidayCalendarCommandHandler(ISchedulingEngine engine) : IRequestHandler<SetHolidayCalendarCommand, HolidayCalendarDto>
{
    public Task<HolidayCalendarDto> Handle(SetHolidayCalendarCommand command, CancellationToken ct)
        => engine.SetHolidayCalendarAsync(command.Calendar, ct);
}
