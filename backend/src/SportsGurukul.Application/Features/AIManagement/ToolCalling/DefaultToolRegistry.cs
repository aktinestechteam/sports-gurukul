using System.Collections.Concurrent;

namespace SportsGurukul.Application.Features.AIManagement.ToolCalling;

public class DefaultToolRegistry : IToolRegistry
{
    private readonly ConcurrentDictionary<string, ToolDescriptor> _tools = new(StringComparer.OrdinalIgnoreCase);

    public void Register(ToolDescriptor descriptor)
        => _tools[descriptor.Name] = descriptor;

    public bool Unregister(string name)
        => _tools.TryRemove(name, out _);

    public ToolDescriptor? Get(string name)
        => _tools.TryGetValue(name, out var descriptor) ? descriptor : null;

    public IReadOnlyList<ToolDescriptor> GetAll()
        => _tools.Values.ToList();

    public bool Contains(string name)
        => _tools.ContainsKey(name);
}
