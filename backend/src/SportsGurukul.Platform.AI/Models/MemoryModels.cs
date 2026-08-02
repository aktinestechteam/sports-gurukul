namespace SportsGurukul.Platform.AI.Models;

public enum MemoryCategory
{
    Working,
    Session,
    LongTerm,
    Semantic,
    Episodic
}

public enum MemoryImportance
{
    Low,
    Medium,
    High,
    Critical
}

public class MemoryEntry
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public MemoryCategory Category { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public MemoryImportance Importance { get; set; } = MemoryImportance.Medium;
    public string? SessionId { get; set; }
    public string? TenantId { get; set; }
    public string? Tags { get; set; }
    public IReadOnlyList<float>? Embedding { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiresAt { get; set; }
    public int AccessCount { get; set; }
}

public class MemoryQuery
{
    public string? Subject { get; set; }
    public MemoryCategory? Category { get; set; }
    public string? SessionId { get; set; }
    public string? TenantId { get; set; }
    public MemoryImportance? MinImportance { get; set; }
    public int Limit { get; set; } = 20;
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
}

public class MemorySnapshot
{
    public string? SessionId { get; set; }
    public IReadOnlyList<MemoryEntry> Working { get; set; } = [];
    public IReadOnlyList<MemoryEntry> Session { get; set; } = [];
    public IReadOnlyList<MemoryEntry> Semantic { get; set; } = [];
    public IReadOnlyList<MemoryEntry> Episodic { get; set; } = [];
    public DateTime CapturedAt { get; set; } = DateTime.UtcNow;
}

public class MemorySearchResult
{
    public IReadOnlyList<MemoryEntry> Entries { get; set; } = [];
    public long DurationMs { get; set; }
}

public class MemoryStats
{
    public int Working { get; set; }
    public int Session { get; set; }
    public int LongTerm { get; set; }
    public int Semantic { get; set; }
    public int Episodic { get; set; }
}
