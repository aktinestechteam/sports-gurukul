using SportsGurukul.Platform.Knowledge.Abstractions;
using SportsGurukul.Platform.Knowledge.Models;

namespace SportsGurukul.Platform.Knowledge.Processing;

public sealed class Deduplicator : IDeduplicator
{
    public string Name => "FingerprintDeduplicator";

    public Task<string?> FindDuplicateAsync(
        DocumentFingerprint fingerprint,
        string indexName,
        string tenantId,
        IReadOnlyList<string> existingFingerprints,
        CancellationToken ct = default)
    {
        if (fingerprint.Value.Length == 0 || existingFingerprints.Count == 0)
        {
            return Task.FromResult<string?>(null);
        }

        foreach (var candidate in existingFingerprints)
        {
            if (string.Equals(candidate, fingerprint.Value, StringComparison.Ordinal))
            {
                return Task.FromResult<string?>(candidate);
            }
        }

        return Task.FromResult<string?>(null);
    }
}
