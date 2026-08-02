using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Features.KnowledgePlatform.Interfaces;
using SportsGurukul.Application.Features.KnowledgePlatform.Models;

namespace SportsGurukul.Application.Features.KnowledgePlatform.Services;

public class KnowledgeAccessService : IKnowledgeAccessService
{
    private readonly ILogger<KnowledgeAccessService> _logger;
    private readonly Dictionary<string, KnowledgeAccessPolicy> _policies = new();

    public KnowledgeAccessService(ILogger<KnowledgeAccessService> logger)
    {
        _logger = logger;
    }

    public Task<bool> CanAccessDocumentAsync(string userId, string documentId, CancellationToken cancellationToken = default)
    {
        var knowledgeBaseId = documentId.Split('_')[0];
        return CanAccessKnowledgeBaseAsync(userId, knowledgeBaseId, cancellationToken);
    }

    public Task<bool> CanAccessKnowledgeBaseAsync(string userId, string knowledgeBaseId, CancellationToken cancellationToken = default)
    {
        if (!_policies.TryGetValue(knowledgeBaseId, out var policy))
        {
            _logger.LogWarning("No access policy found for knowledge base {KnowledgeBaseId}", knowledgeBaseId);
            return Task.FromResult(false);
        }

        if (policy.IsPublic)
            return Task.FromResult(true);

        if (policy.AllowedUsers.Contains(userId))
            return Task.FromResult(true);

        return Task.FromResult(false);
    }

    public Task<KnowledgeAccessPolicy> GetAccessPolicyAsync(string knowledgeBaseId, CancellationToken cancellationToken = default)
    {
        if (_policies.TryGetValue(knowledgeBaseId, out var policy))
            return Task.FromResult(policy);

        return Task.FromResult(new KnowledgeAccessPolicy(
            knowledgeBaseId, new List<string>(), new List<string>(),
            false, false));
    }

    public Task SetAccessPolicyAsync(string knowledgeBaseId, KnowledgeAccessPolicy policy, CancellationToken cancellationToken = default)
    {
        _policies[knowledgeBaseId] = policy;
        _logger.LogInformation("Set access policy for knowledge base {KnowledgeBaseId}: Public={IsPublic}",
            knowledgeBaseId, policy.IsPublic);
        return Task.CompletedTask;
    }
}
