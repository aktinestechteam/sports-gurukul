using Microsoft.Extensions.DependencyInjection;
using SportsGurukul.Application.Features.KnowledgePlatform.Chunking;
using SportsGurukul.Application.Features.KnowledgePlatform.Embedding;
using SportsGurukul.Application.Features.KnowledgePlatform.Interfaces;
using SportsGurukul.Application.Features.KnowledgePlatform.Parsers;
using SportsGurukul.Application.Features.KnowledgePlatform.Services;
using SportsGurukul.Application.Features.KnowledgePlatform.VectorStores;

namespace SportsGurukul.Application.Features.KnowledgePlatform;

public static class DependencyInjection
{
    public static IServiceCollection AddKnowledgePlatform(this IServiceCollection services)
    {
        services.AddSingleton<IDocumentParserFactory, DocumentParserFactory>();
        services.AddTransient<IDocumentParser, PdfParser>();
        services.AddTransient<IDocumentParser, WordParser>();
        services.AddTransient<IDocumentParser, MarkdownParser>();
        services.AddTransient<IDocumentParser, HtmlParser>();
        services.AddTransient<IDocumentParser, TxtParser>();
        services.AddTransient<IDocumentParser, CsvParser>();
        services.AddTransient<IDocumentParser, JsonParser>();
        services.AddTransient<IDocumentParser, XmlParser>();

        services.AddSingleton<IChunkingStrategyFactory, ChunkingStrategyFactory>();
        services.AddTransient<IChunkingStrategy, FixedSizeChunker>();
        services.AddTransient<IChunkingStrategy, SemanticChunker>();
        services.AddTransient<IChunkingStrategy, HeadingBasedChunker>();
        services.AddTransient<IChunkingStrategy, SlidingWindowChunker>();
        services.AddTransient<IChunkingStrategy, RecursiveChunker>();
        services.AddTransient<IChunkingStrategy, ParentChildChunker>();

        services.AddSingleton<IEmbeddingProviderFactory, EmbeddingProviderFactory>();
        services.AddTransient<IEmbeddingProvider, OpenAIEmbeddingProvider>();
        services.AddTransient<IEmbeddingProvider, AzureOpenAIEmbeddingProvider>();
        services.AddTransient<IEmbeddingProvider, GeminiEmbeddingProvider>();
        services.AddTransient<IEmbeddingProvider, CohereEmbeddingProvider>();
        services.AddTransient<IEmbeddingProvider, SentenceTransformersEmbeddingProvider>();
        services.AddTransient<IEmbeddingProvider, OllamaEmbeddingProvider>();

        services.AddSingleton<IVectorStoreFactory, VectorStoreFactory>();
        services.AddTransient<IVectorStore, QdrantVectorStore>();
        services.AddTransient<IVectorStore, AzureAISearchVectorStore>();
        services.AddTransient<IVectorStore, PineconeVectorStore>();
        services.AddTransient<IVectorStore, WeaviateVectorStore>();
        services.AddTransient<IVectorStore, MilvusVectorStore>();
        services.AddTransient<IVectorStore, FaissVectorStore>();
        services.AddTransient<IVectorStore, ChromaVectorStore>();
        services.AddTransient<IVectorStore, PgVectorVectorStore>();

        services.AddTransient<IDocumentProcessor, DocumentProcessingService>();
        services.AddTransient<IChunkingService, ChunkingService>();
        services.AddTransient<IEmbeddingService, EmbeddingService>();
        services.AddTransient<IVectorStoreService, VectorStoreService>();
        services.AddTransient<IRetrievalService, RetrievalService>();
        services.AddTransient<IRerankerService, RerankerService>();
        services.AddTransient<ICitationService, CitationService>();

        services.AddTransient<IKnowledgeIngestionService, KnowledgeIngestionService>();
        services.AddTransient<IKnowledgeSearchService, KnowledgeSearchService>();
        services.AddTransient<IKnowledgeManagementService, KnowledgeManagementService>();
        services.AddTransient<IKnowledgeAccessService, KnowledgeAccessService>();
        services.AddTransient<IKnowledgeObservabilityService, KnowledgeObservabilityService>();

        return services;
    }
}
