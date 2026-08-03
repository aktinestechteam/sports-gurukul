using Microsoft.Extensions.DependencyInjection;
using SportsGurukul.Platform.Knowledge.Abstractions;
using SportsGurukul.Platform.Knowledge.Configuration;
using Xunit;

namespace SportsGurukul.Platform.Knowledge.Tests;

public class DependencyInjectionTests
{
    [Fact]
    public void AddKnowledgePlatform_Resolves_CoreServices()
    {
        using var provider = TestHarness.CreateProvider();

        Assert.NotNull(provider.GetRequiredService<IKnowledgeIndexService>());
        Assert.NotNull(provider.GetRequiredService<IKnowledgeIngestionService>());
        Assert.NotNull(provider.GetRequiredService<IKnowledgeSearchService>());
        Assert.NotNull(provider.GetRequiredService<IRetrievalService>());
        Assert.NotNull(provider.GetRequiredService<IVectorStoreFactory>());
        Assert.NotNull(provider.GetRequiredService<IEmbeddingProviderFactory>());
        Assert.NotNull(provider.GetRequiredService<IEmbeddingService>());
        Assert.NotNull(provider.GetRequiredService<IKnowledgeHealthService>());
        Assert.NotNull(provider.GetRequiredService<IKnowledgeMetricsCollector>());
        Assert.NotNull(provider.GetRequiredService<IKnowledgeAuditLogger>());
    }

    [Fact]
    public void AddKnowledgePlatform_Resolves_DeterministicProvider_ByDefault()
    {
        using var provider = TestHarness.CreateProvider();
        var factory = provider.GetRequiredService<IEmbeddingProviderFactory>();

        Assert.Equal("deterministic", factory.GetProvider().Name);
    }

    [Fact]
    public void AddKnowledgePlatform_Resolves_InMemoryStore_ByDefault()
    {
        using var provider = TestHarness.CreateProvider();
        var factory = provider.GetRequiredService<IVectorStoreFactory>();

        Assert.Equal("inmemory", factory.GetStore().Name);
        Assert.True(factory.GetStore().Capabilities.SupportsVector);
        Assert.True(factory.GetStore().Capabilities.SupportsKeyword);
    }

    [Fact]
    public void AddKnowledgePlatform_Honors_ConfigureOptions()
    {
        using var provider = TestHarness.CreateProvider(options =>
        {
            options.Embedding.Dimensions = 128;
            options.Retrieval.Reranker = "rrf";
        });

        var embedding = provider.GetRequiredService<IEmbeddingService>();
        var options = provider.GetRequiredService<KnowledgePlatformOptions>();

        Assert.Equal(128, embedding.Provider.Dimensions);
        Assert.Equal("rrf", options.Retrieval.Reranker);
    }
}
