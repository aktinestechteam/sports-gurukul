using System.Diagnostics;
using SportsGurukul.Platform.Knowledge.Configuration;
using SportsGurukul.Platform.Knowledge.Models;

using SportsGurukul.Platform.Knowledge.Abstractions;
using SportsGurukul.Platform.Knowledge.Security;

namespace SportsGurukul.Platform.Knowledge.Retrieval;

internal sealed class KnowledgeSearchService : IKnowledgeSearchService
{
    private readonly IRetrievalService _retrievalService;
    private readonly IAccessPolicyEvaluator _accessPolicyEvaluator;
    private readonly ITenantIsolationService _tenantIsolationService;
    private readonly ICitationService _citationService;
    private readonly IKnowledgeAuditLogger _auditLogger;
    private readonly IKnowledgeMetricsCollector _metrics;
    private readonly KnowledgePlatformOptions _options;

    public KnowledgeSearchService(
        IRetrievalService retrievalService,
        IAccessPolicyEvaluator accessPolicyEvaluator,
        ITenantIsolationService tenantIsolationService,
        ICitationService citationService,
        IKnowledgeAuditLogger auditLogger,
        IKnowledgeMetricsCollector metrics,
        KnowledgePlatformOptions options)
    {
        _retrievalService = retrievalService;
        _accessPolicyEvaluator = accessPolicyEvaluator;
        _tenantIsolationService = tenantIsolationService;
        _citationService = citationService;
        _auditLogger = auditLogger;
        _metrics = metrics;
        _options = options;
    }

    public async Task<KnowledgeSearchResponse> SearchAsync(KnowledgeSearchRequest request, CancellationToken ct = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var principal = new KnowledgePrincipal(
            request.ActorUserId ?? string.Empty,
            request.TenantId ?? string.Empty,
            request.Roles ?? Array.Empty<string>(),
            !string.IsNullOrEmpty(request.ActorUserId));

        var scoped = ScopedRequest(request, principal);
        var decision = _accessPolicyEvaluator.Evaluate(
            principal,
            BuildPolicy(request),
            AccessPermission.Read);

        if (!decision.Allowed)
        {
            _metrics.RecordAccessDenied(request.IndexName);
            await _auditLogger.LogAsync(CreateAuditEvent(
                KnowledgeAuditAction.AccessDenied,
                request.ActorUserId ?? string.Empty,
                request.TenantId ?? string.Empty,
                request.IndexName,
                entityId: null,
                succeeded: false,
                reason: decision.Reason), ct);

            throw new KnowledgeAccessDeniedException(
                decision.Reason ?? "Access denied.",
                request.IndexName,
                request.ActorUserId ?? string.Empty);
        }

        var result = await _retrievalService.SearchAsync(scoped, ct);
        var citations = request.IncludeCitations
            ? _citationService.BuildCitations(result.Chunks)
            : Array.Empty<Citation>();

        stopwatch.Stop();
        await _auditLogger.LogAsync(CreateAuditEvent(
            KnowledgeAuditAction.Search,
            request.ActorUserId ?? string.Empty,
            request.TenantId ?? string.Empty,
            request.IndexName,
            entityId: null,
            succeeded: true,
            reason: null,
            context: new Dictionary<string, string>
            {
                ["mode"] = scoped.Mode.ToString(),
                ["topK"] = scoped.TopK.ToString(),
                ["results"] = result.Chunks.Count.ToString()
            }), ct);

        return new KnowledgeSearchResponse(
            request.Query,
            request.IndexName,
            result.Mode,
            result.ElapsedMs,
            result.TotalCandidates,
            result.Chunks,
            citations);
    }

    public async Task<KnowledgeSearchResponse> SearchMultiKnowledgeAsync(
        MultiKnowledgeSearchRequest request,
        CancellationToken ct = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var responses = new List<KnowledgeSearchResponse>();

        foreach (var indexName in request.IndexNames)
        {
            ct.ThrowIfCancellationRequested();
            var single = new KnowledgeSearchRequest(
                request.Query,
                indexName,
                request.TenantId,
                request.ActorUserId,
                request.Roles,
                request.Mode,
                request.TopKPerIndex,
                request.MinScore,
                IncludeCitations: false);

            responses.Add(await SearchAsync(single, ct));
        }

        var combined = responses.SelectMany(r => r.Chunks).ToList();
        var merged = MergeAcrossIndexes(combined, request.FinalTopK);
        var citations = request.IncludeCitations
            ? _citationService.BuildCitations(merged)
            : Array.Empty<Citation>();

        stopwatch.Stop();
        return new KnowledgeSearchResponse(
            request.Query,
            string.Join(",", request.IndexNames),
            request.Mode,
            stopwatch.ElapsedMilliseconds,
            responses.Sum(r => r.TotalCandidates),
            merged,
            citations);
    }

    private KnowledgeSearchRequest ScopedRequest(KnowledgeSearchRequest request, KnowledgePrincipal principal)
    {
        var filter = _tenantIsolationService.ScopeFilter(
            new VectorFilter(request.IndexName, request.TenantId),
            principal);
        return request with { TenantId = filter.TenantId ?? request.TenantId };
    }

    private static AccessPolicy BuildPolicy(KnowledgeSearchRequest request)
    {
        if (string.IsNullOrEmpty(request.ActorUserId))
        {
            return new AccessPolicy(AccessScopeType.Public);
        }

        return new AccessPolicy(AccessScopeType.Authenticated);
    }

    private static IReadOnlyList<RetrievedChunk> MergeAcrossIndexes(IReadOnlyList<RetrievedChunk> chunks, int topK)
    {
        return chunks
            .GroupBy(c => c.Chunk.Id)
            .Select(g => g.OrderByDescending(c => c.Score).First())
            .OrderByDescending(c => c.Score)
            .Take(Math.Max(0, topK))
            .Select((c, i) => c with { Rank = i })
            .ToList();
    }

    private static KnowledgeAuditEvent CreateAuditEvent(
        KnowledgeAuditAction action,
        string actorUserId,
        string tenantId,
        string indexName,
        string? entityId,
        bool succeeded,
        string? reason,
        IReadOnlyDictionary<string, string>? context = null) =>
        new(
            Guid.NewGuid(),
            DateTime.UtcNow,
            action,
            actorUserId,
            tenantId,
            indexName,
            entityId,
            "knowledge",
            succeeded,
            reason,
            context);
}
