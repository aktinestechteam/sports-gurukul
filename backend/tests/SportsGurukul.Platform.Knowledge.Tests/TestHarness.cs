using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SportsGurukul.Platform.Knowledge.Configuration;
using SportsGurukul.Platform.Knowledge.Models;
using SportsGurukul.Platform.Knowledge.Security;
using Xunit;

namespace SportsGurukul.Platform.Knowledge.Tests;

internal static class TestHarness
{
    public static ServiceProvider CreateProvider(Action<KnowledgePlatformOptions>? configure = null)
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["KnowledgePlatform:Security:EncryptionKeyBase64"] = EncryptionService.GenerateKey()
            })
            .Build();

        var services = new ServiceCollection();
        services.AddKnowledgePlatform(config, options =>
        {
            options.Chunking.MinChunkSize = 1;
            configure?.Invoke(options);
        });
        return services.BuildServiceProvider();
    }

    public static KnowledgeDocument Document(
        string title,
        string text,
        string tenantId,
        string indexName,
        string ownerUserId = "user-1",
        Guid? id = null)
    {
        var documentId = id ?? Guid.NewGuid();
        var directory = Path.Combine(Path.GetTempPath(), "sportsgurukul-knowledge-tests");
        Directory.CreateDirectory(directory);
        var storagePath = Path.Combine(directory, $"{documentId}.txt");
        File.WriteAllText(storagePath, text);

        return new KnowledgeDocument(
            documentId,
            title,
            "text/plain",
            DocumentType.Text,
            FileName: $"{title}.txt",
            Language: "en",
            SizeBytes: text.Length,
            StoragePath: storagePath,
            TenantId: tenantId,
            OwnerUserId: ownerUserId,
            IndexName: indexName);
    }
}
