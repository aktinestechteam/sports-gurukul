using SportsGurukul.Application.Common.Interfaces.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Features.AIManagement.ToolCalling;

public class ToolResolver : IToolResolver
{
    private readonly IToolRegistry _registry;
    private readonly IAgentRepository _agentRepository;

    public ToolResolver(IToolRegistry registry, IAgentRepository agentRepository)
    {
        _registry = registry;
        _agentRepository = agentRepository;
    }

    public Task<ToolDescriptor?> ResolveAsync(string name, CancellationToken cancellationToken = default)
        => Task.FromResult(_registry.Get(name));

    public async Task<IReadOnlyList<ToolDescriptor>> ResolveForAgentAsync(Guid agentId, CancellationToken cancellationToken = default)
    {
        var agent = await _agentRepository.GetByIdWithToolsAsync(agentId, cancellationToken);
        if (agent is null)
            return new List<ToolDescriptor>();

        var descriptors = new List<ToolDescriptor>();
        foreach (var tool in agent.Tools.Where(t => t.IsActive))
        {
            descriptors.Add(new ToolDescriptor(
                tool.Id,
                tool.Name,
                tool.ToolType,
                tool.Description,
                tool.InputSchemaJson,
                tool.OutputSchemaJson,
                tool.IsSystemTool,
                tool.RequiresApproval,
                null));
        }

        descriptors.AddRange(_registry.GetAll()
            .Where(d => d.IsSystemTool && !descriptors.Any(x => x.Name.Equals(d.Name, StringComparison.OrdinalIgnoreCase))));

        return descriptors;
    }
}
