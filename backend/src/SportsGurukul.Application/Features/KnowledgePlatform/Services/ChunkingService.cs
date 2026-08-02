using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Features.KnowledgePlatform.Interfaces;
using SportsGurukul.Application.Features.KnowledgePlatform.Models;

namespace SportsGurukul.Application.Features.KnowledgePlatform.Services;

public class ChunkingService : IChunkingService
{
    private readonly IChunkingStrategyFactory _strategyFactory;
    private readonly ILogger<ChunkingService> _logger;

    public ChunkingService(
        IChunkingStrategyFactory strategyFactory,
        ILogger<ChunkingService> logger)
    {
        _strategyFactory = strategyFactory;
        _logger = logger;
    }

    public async Task<List<DocumentChunk>> ChunkDocumentAsync(ExtractedDocument document, ChunkingOptions options, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Chunking document {DocumentId} using {Strategy} strategy", document.Id, options.Strategy);

        var strategy = _strategyFactory.GetStrategy(options.Strategy);
        var chunks = await strategy.ChunkAsync(document, options, cancellationToken);

        _logger.LogInformation("Document {DocumentId} split into {ChunkCount} chunks", document.Id, chunks.Count);

        return chunks;
    }
}
