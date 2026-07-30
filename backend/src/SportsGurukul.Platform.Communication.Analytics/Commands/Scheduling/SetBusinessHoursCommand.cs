using MediatR;
using SportsGurukul.Platform.Communication.Analytics.Abstractions;
using SportsGurukul.Platform.Communication.Analytics.DTOs;

namespace SportsGurukul.Platform.Communication.Analytics.Commands.Scheduling;

public record SetBusinessHoursCommand(BusinessHoursDto Hours) : IRequest<BusinessHoursDto>;

public class SetBusinessHoursCommandHandler(ISchedulingEngine engine) : IRequestHandler<SetBusinessHoursCommand, BusinessHoursDto>
{
    public Task<BusinessHoursDto> Handle(SetBusinessHoursCommand command, CancellationToken ct)
        => engine.SetBusinessHoursAsync(command.Hours, ct);
}
