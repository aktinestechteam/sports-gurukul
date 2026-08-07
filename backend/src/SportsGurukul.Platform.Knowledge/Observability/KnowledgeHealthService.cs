using SportsGurukul.Platform.Knowledge.Models;

using SportsGurukul.Platform.Knowledge.Abstractions;

namespace SportsGurukul.Platform.Knowledge.Observability;

internal sealed class KnowledgeHealthService : IKnowledgeHealthService
{
    private readonly IEmbeddingProviderFactory _embeddingFactory;
    private readonly IVectorStoreFactory _vectorStoreFactory;

    public KnowledgeHealthService(
        IEmbeddingProviderFactory embeddingFactory,
        IVectorStoreFactory vectorStoreFactory)
    {
        _embeddingFactory = embeddingFactory;
        _vectorStoreFactory = vectorStoreFactory;
    }

    public async Task<KnowledgeHealthReport> GetHealthAsync(CancellationToken ct = default)
    {
        var components = new Dictionary<string, KnowledgeComponentHealth>(StringComparer.Ordinal);

        try
        {
            var embeddingProvider = _embeddingFactory.GetProvider();
            var embeddingHealthy = await embeddingProvider.IsHealthyAsync(ct);
            components["embedding"] = new KnowledgeComponentHealth(
                embeddingProvider.Name,
                embeddingHealthy,
                embeddingHealthy ? null : "Embedding provider is unreachable or unhealthy.");
        }
        catch (NotSupportedException ex)
        {
            components["embedding"] = new KnowledgeComponentHealth("embedding", false, ex.Message);
        }
        catch (Exception ex)
        {
            components["embedding"] = new KnowledgeComponentHealth("embedding", false, ex.Message);
        }

        try
        {
            var store = _vectorStoreFactory.GetStore();
            var storeHealthy = await store.IsHealthyAsync(ct);
            components["vectorStore"] = new KnowledgeComponentHealth(
                store.Name,
                storeHealthy,
                storeHealthy ? null : "Vector store is unreachable or unhealthy.");
        }
        catch (Exception ex)
        {
            components["vectorStore"] = new KnowledgeComponentHealth("vectorStore", false, ex.Message);
        }

        var allHealthy = components.Values.All(c => c.Healthy);
        var state = allHealthy ? KnowledgeHealthState.Healthy : KnowledgeHealthState.Degraded;
        return new KnowledgeHealthReport(
            state,
            allHealthy ? "All knowledge platform components are healthy." : "One or more knowledge platform components are degraded.",
            components);
    }
}
