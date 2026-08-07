using SportsGurukul.Platform.Knowledge.Models;

namespace SportsGurukul.Platform.Knowledge.Abstractions;

public interface IRetrievalService
{
    Task<SearchResult> SearchAsync(KnowledgeSearchRequest request, CancellationToken ct = default);
    IAsyncEnumerable<RetrievedChunk> StreamAsync(KnowledgeSearchRequest request, CancellationToken ct = default);
}

public interface IReranker
{
    string Name { get; }
    Task<IReadOnlyList<RetrievedChunk>> RerankAsync(
        string query,
        IReadOnlyList<RetrievedChunk> candidates,
        CancellationToken ct = default);
}

public interface ICitationService
{
    IReadOnlyList<Citation> BuildCitations(IReadOnlyList<RetrievedChunk> chunks);
}

public interface IKnowledgeSearchService
{
    Task<KnowledgeSearchResponse> SearchAsync(KnowledgeSearchRequest request, CancellationToken ct = default);
    Task<KnowledgeSearchResponse> SearchMultiKnowledgeAsync(
        MultiKnowledgeSearchRequest request,
        CancellationToken ct = default);
}
