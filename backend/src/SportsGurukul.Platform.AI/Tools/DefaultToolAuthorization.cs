using SportsGurukul.Platform.AI.Interfaces.Tools;
using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Tools;

public class DefaultToolAuthorization : IToolAuthorization
{
    public Task<ToolAuthorizationDecision> AuthorizeAsync(ITool tool, ToolExecutionContext context, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (tool.RequiresApproval)
        {
            return Task.FromResult(ToolAuthorizationDecision.Allow("Approval required for tool.", requiresApproval: true));
        }

        if (string.IsNullOrWhiteSpace(context.TenantId) && tool.Permission == "tenant-scoped")
        {
            return Task.FromResult(ToolAuthorizationDecision.Deny("Tenant context is required for tenant-scoped tools."));
        }

        return Task.FromResult(ToolAuthorizationDecision.Allow());
    }
}
