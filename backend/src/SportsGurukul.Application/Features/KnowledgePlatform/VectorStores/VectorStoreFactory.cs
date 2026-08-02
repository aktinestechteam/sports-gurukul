using SportsGurukul.Application.Features.KnowledgePlatform.Interfaces;
using SportsGurukul.Application.Features.KnowledgePlatform.Models;

namespace SportsGurukul.Application.Features.KnowledgePlatform.VectorStores;

public class VectorStoreFactory : IVectorStoreFactory
{
    private readonly Dictionary<VectorStoreType, IVectorStore> _stores;
    private readonly Dictionary<string, IVectorStore> _namedStores;

    public VectorStoreFactory(IEnumerable<IVectorStore> stores)
    {
        _stores = stores.ToDictionary(s => s.StoreType);
        _namedStores = stores.ToDictionary(s => s.StoreType.ToString(), StringComparer.OrdinalIgnoreCase);
    }

    public IVectorStore GetStore(VectorStoreType type) =>
        _stores.TryGetValue(type, out var store) ? store
        : throw new NotSupportedException($"No vector store registered for type: {type}");

    public IVectorStore GetStore(string storeName) =>
        _namedStores.TryGetValue(storeName, out var store) ? store
        : throw new NotSupportedException($"No vector store registered with name: {storeName}");

    public bool SupportsStore(VectorStoreType type) =>
        _stores.ContainsKey(type);
}
