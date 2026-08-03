namespace SportsGurukul.Application.Features.AIManagement.ToolCalling;

public interface IToolResolver
{
    Task<ToolDescriptor?> ResolveAsync(string name, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ToolDescriptor>> ResolveForAgentAsync(Guid agentId, CancellationToken cancellationToken = default);
}
