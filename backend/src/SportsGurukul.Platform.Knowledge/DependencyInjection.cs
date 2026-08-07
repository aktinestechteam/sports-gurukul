using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SportsGurukul.Platform.Knowledge.Abstractions;
using SportsGurukul.Platform.Knowledge.Chunking;
using SportsGurukul.Platform.Knowledge.Configuration;
using SportsGurukul.Platform.Knowledge.Embedding;
using SportsGurukul.Platform.Knowledge.Indexing;
using SportsGurukul.Platform.Knowledge.Observability;
using SportsGurukul.Platform.Knowledge.Processing;
using SportsGurukul.Platform.Knowledge.Processing.TextExtraction;
using SportsGurukul.Platform.Knowledge.Retrieval;
using SportsGurukul.Platform.Knowledge.Security;
using SportsGurukul.Platform.Knowledge.VectorStores;

namespace SportsGurukul.Platform.Knowledge;

public static class DependencyInjection
{
    public static IServiceCollection AddKnowledgePlatform(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<KnowledgePlatformOptions>? configureOptions = null)
    {
        var options = new KnowledgePlatformOptions();
        configuration.GetSection(KnowledgePlatformOptions.SectionName).Bind(options);
        configureOptions?.Invoke(options);

        services.AddSingleton(options);
        services.AddSingleton(Options.Create(options));
        services.AddSingleton(options.Embedding);
        services.AddSingleton(options.VectorStore);
        services.AddSingleton(options.Chunking);
        services.AddSingleton(options.Retrieval);
        services.AddSingleton(options.Security);
        services.AddSingleton(options.Observability);

        services.AddLogging();

        RegisterProcessing(services);
        RegisterChunking(services);
        RegisterEmbedding(services);
        RegisterVectorStores(services);
        RegisterRetrieval(services);
        RegisterIndexing(services);
        RegisterSecurity(services);
        RegisterObservability(services);

        return services;
    }

    private static void RegisterProcessing(IServiceCollection services)
    {
        services.AddSingleton<ITextExtractorRegistry, TextExtractorRegistry>();
        services.AddSingleton<ILanguageDetector, LanguageDetector>();
        services.AddSingleton<IPiiDetector, PiiDetector>();
        services.AddSingleton<IContentClassifier, ContentClassifier>();
        services.AddSingleton<IDocumentFingerprinter, DocumentFingerprinter>();
        services.AddSingleton<IDeduplicator, Deduplicator>();
        services.AddSingleton<IDocumentProcessor, DocumentProcessor>();
    }

    private static void RegisterChunking(IServiceCollection services)
    {
        services.AddSingleton<IChunkingStrategyRegistry, ChunkingStrategyRegistry>();
        services.AddSingleton<IChunkingService, ChunkingService>();
    }

    private static void RegisterEmbedding(IServiceCollection services)
    {
        services.AddHttpClient("KnowledgePlatform.Embedding");
        services.AddSingleton<IEmbeddingProviderFactory, EmbeddingProviderFactory>();
        services.AddSingleton<IEmbeddingService, EmbeddingService>();
    }

    private static void RegisterVectorStores(IServiceCollection services)
    {
        services.AddHttpClient("KnowledgePlatform.Qdrant");
        services.AddSingleton<IVectorStoreFactory, VectorStoreFactory>();
    }

    private static void RegisterRetrieval(IServiceCollection services)
    {
        services.AddSingleton<IReranker, ScoreReranker>();
        services.AddSingleton<IReranker, RrfReranker>();
        services.AddSingleton<ICitationService, CitationService>();
        services.AddSingleton<IRetrievalService, RetrievalService>();
        services.AddSingleton<IKnowledgeSearchService, KnowledgeSearchService>();
    }

    private static void RegisterIndexing(IServiceCollection services)
    {
        services.AddSingleton<IKnowledgeIndexStore, InMemoryIndexStore>();
        services.AddSingleton<IKnowledgeIngestionService, KnowledgeIngestionService>();
        services.AddSingleton<IKnowledgeIndexService, KnowledgeIndexService>();
    }

    private static void RegisterSecurity(IServiceCollection services)
    {
        services.AddSingleton<IAccessPolicyEvaluator, AccessPolicyEvaluator>();
        services.AddSingleton<ITenantIsolationService, TenantIsolationService>();
        services.AddSingleton<IEncryptionService, EncryptionService>();
        services.AddSingleton<IKnowledgeAuditLogger, KnowledgeAuditLogger>();
    }

    private static void RegisterObservability(IServiceCollection services)
    {
        services.AddSingleton<IKnowledgeMetricsCollector, KnowledgeMetricsCollector>();
        services.AddSingleton<IKnowledgeHealthService, KnowledgeHealthService>();
    }
}
