namespace SportsGurukul.Application.Features.AIManagement.ToolCalling;

public interface IToolRegistry
{
    void Register(ToolDescriptor descriptor);

    bool Unregister(string name);

    ToolDescriptor? Get(string name);

    IReadOnlyList<ToolDescriptor> GetAll();

    bool Contains(string name);
}
