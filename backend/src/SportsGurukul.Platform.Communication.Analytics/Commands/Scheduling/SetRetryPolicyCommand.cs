using MediatR;
using SportsGurukul.Platform.Communication.Analytics.Abstractions;
using SportsGurukul.Platform.Communication.Analytics.DTOs;

namespace SportsGurukul.Platform.Communication.Analytics.Commands.Scheduling;

public record SetRetryPolicyCommand(RetryWindowDto Policy) : IRequest<RetryWindowDto>;

public class SetRetryPolicyCommandHandler(ISchedulingEngine engine) : IRequestHandler<SetRetryPolicyCommand, RetryWindowDto>
{
    public Task<RetryWindowDto> Handle(SetRetryPolicyCommand command, CancellationToken ct)
        => engine.SetRetryPolicyAsync(command.Policy, ct);
}
