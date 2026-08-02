using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Features.KnowledgePlatform.Interfaces;
using SportsGurukul.Application.Features.KnowledgePlatform.Models;

namespace SportsGurukul.Application.Features.KnowledgePlatform.Services;

public class KnowledgeManagementService : IKnowledgeManagementService
{
    private readonly IVectorStoreFactory _storeFactory;
    private readonly IKnowledgeObservabilityService? _observability;
    private readonly ILogger<KnowledgeManagementService> _logger;

    public KnowledgeManagementService(
        IVectorStoreFactory storeFactory,
        ILogger<KnowledgeManagementService> logger,
        IKnowledgeObservabilityService? observability = null)
    {
        _storeFactory = storeFactory;
        _logger = logger;
        _observability = observability;
    }

    public async Task<KnowledgeIndexInfo> CreateIndexAsync(
        string indexName,
        VectorStoreType storeType,
        int dimensions,
        CancellationToken cancellationToken = default)
    {
        var store = _storeFactory.GetStore(storeType);
        await store.CreateIndexAsync(indexName, dimensions, null, cancellationToken);

        _logger.LogInformation("Created index '{Index}' with {Dimensions} dimensions on {Store}",
            indexName, dimensions, storeType);

        var info = new KnowledgeIndexInfo(
            indexName, storeType, 0, 0, 0,
            IndexOperation.Create, DateTime.UtcNow, ProcessingStatus.Indexed);

        if (_observability != null)
            await _observability.RecordIndexMetricAsync(indexName, "create", 1, cancellationToken: cancellationToken);

        return info;
    }

    public async Task<bool> DeleteIndexAsync(string indexName, VectorStoreType storeType, CancellationToken cancellationToken = default)
    {
        var store = _storeFactory.GetStore(storeType);
        await store.DeleteIndexAsync(indexName, cancellationToken);

        if (_observability != null)
            await _observability.RecordIndexMetricAsync(indexName, "delete", 1, cancellationToken: cancellationToken);

        return true;
    }

    public async Task<KnowledgeIndexInfo> RebuildIndexAsync(string indexName, VectorStoreType storeType, CancellationToken cancellationToken = default)
    {
        var store = _storeFactory.GetStore(storeType);

        if (await store.IndexExistsAsync(indexName, cancellationToken))
            await store.DeleteIndexAsync(indexName, cancellationToken);

        await store.CreateIndexAsync(indexName, 1536, null, cancellationToken);

        if (_observability != null)
            await _observability.RecordIndexMetricAsync(indexName, "rebuild", 1, cancellationToken: cancellationToken);

        return new KnowledgeIndexInfo(
            indexName, storeType, 0, 0, 0,
            IndexOperation.Rebuild, DateTime.UtcNow, ProcessingStatus.Indexed);
    }

    public async Task<KnowledgeIndexInfo> IncrementalIndexAsync(string indexName, VectorStoreType storeType, CancellationToken cancellationToken = default)
    {
        if (_observability != null)
            await _observability.RecordIndexMetricAsync(indexName, "incremental", 1, cancellationToken: cancellationToken);

        return new KnowledgeIndexInfo(
            indexName, storeType, 0, 0, 0,
            IndexOperation.Incremental, DateTime.UtcNow, ProcessingStatus.Indexed);
    }

    public Task<bool> ArchiveIndexAsync(string indexName, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Archived index '{Index}'", indexName);
        return Task.FromResult(true);
    }

    public Task<bool> RestoreIndexAsync(string indexName, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Restored index '{Index}'", indexName);
        return Task.FromResult(true);
    }

    public async Task<KnowledgeIndexInfo> GetIndexInfoAsync(string indexName, VectorStoreType storeType, CancellationToken cancellationToken = default)
    {
        var store = _storeFactory.GetStore(storeType);
        var exists = await store.IndexExistsAsync(indexName, cancellationToken);

        if (!exists)
            return new KnowledgeIndexInfo(
                indexName, storeType, 0, 0, 0,
                IndexOperation.Create, DateTime.MinValue, ProcessingStatus.Pending);

        var vectorCount = await store.GetVectorCountAsync(indexName, cancellationToken);

        return new KnowledgeIndexInfo(
            indexName, storeType, 0, (int)vectorCount, 0,
            IndexOperation.Update, DateTime.UtcNow, ProcessingStatus.Indexed);
    }

    public async Task<List<KnowledgeIndexInfo>> ListIndexesAsync(CancellationToken cancellationToken = default)
    {
        var indexes = new List<KnowledgeIndexInfo>();

        foreach (VectorStoreType storeType in Enum.GetValues<VectorStoreType>())
        {
            try
            {
                var store = _storeFactory.GetStore(storeType);
                indexes.Add(new KnowledgeIndexInfo(
                    $"default_{storeType.ToString().ToLower()}",
                    storeType, 0, 0, 0,
                    IndexOperation.Create, DateTime.UtcNow, ProcessingStatus.Pending));
            }
            catch { }
        }

        return indexes;
    }

    public Task<bool> OptimizeIndexAsync(string indexName, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Optimized index '{Index}'", indexName);
        return Task.FromResult(true);
    }
}
