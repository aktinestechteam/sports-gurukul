using System.Text;
using System.Text.RegularExpressions;
using SportsGurukul.Platform.Knowledge.Abstractions;
using SportsGurukul.Platform.Knowledge.Models;

namespace SportsGurukul.Platform.Knowledge.Chunking;

internal sealed partial class HeadingBasedChunker : ChunkingStrategyBase
{
    public override ChunkingStrategyType Type => ChunkingStrategyType.HeadingBased;

    public override IReadOnlyList<DocumentChunk> Chunk(
        KnowledgeDocument document,
        string text,
        ChunkingOptions options,
        IEmbeddingProvider? embeddingProvider = null,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return Array.Empty<DocumentChunk>();
        }

        var (sections, bodySegments) = SplitByHeadings(text);
        var chunks = new List<DocumentChunk>();
        var order = 0;

        for (var i = 0; i < sections.Count; i++)
        {
            ct.ThrowIfCancellationRequested();
            var (heading, level) = sections[i];
            var content = bodySegments[i];
            var joined = heading.Length > 0 ? $"{heading}\n{content}" : content;

            if (Measure(joined, options) <= options.ChunkSize)
            {
                if (joined.Length >= options.MinChunkSize)
                {
                    chunks.Add(CreateChunk(document, joined, order, options, heading: heading));
                    order++;
                }
            }
            else
            {
                foreach (var piece in SplitOversized(document, joined, heading, options, order, ref ct))
                {
                    chunks.Add(piece);
                    order++;
                }
            }
        }

        return chunks;
    }

    private static (IReadOnlyList<(string Heading, int Level)> Sections, IReadOnlyList<string> Bodies) SplitByHeadings(
        string text)
    {
        var sections = new List<(string Heading, int Level)>();
        var bodies = new List<string>();
        var currentHeading = string.Empty;
        var currentLevel = 0;
        var currentBody = new StringBuilder();

        void Flush()
        {
            if (currentBody.Length > 0 || sections.Count > 0)
            {
                sections.Add((currentHeading, currentLevel));
                bodies.Add(currentBody.ToString().Trim());
                currentBody.Clear();
            }
        }

        var lines = text.Replace("\r\n", "\n").Split('\n');
        foreach (var line in lines)
        {
            if (TryParseHeading(line, out var heading, out var level))
            {
                Flush();
                currentHeading = heading;
                currentLevel = level;
            }
            else
            {
                currentBody.AppendLine(line);
            }
        }

        Flush();

        if (sections.Count == 0)
        {
            sections.Add((string.Empty, 0));
            bodies.Add(text.Trim());
        }

        return (sections, bodies);
    }

    private static bool TryParseHeading(string line, out string heading, out int level)
    {
        var trimmed = line.Trim();
        if (trimmed.StartsWith("#"))
        {
            var l = trimmed.TakeWhile(c => c == '#').Count();
            var h = trimmed[l..].Trim();
            if (h.Length > 0)
            {
                heading = h;
                level = l;
                return true;
            }
        }

        var numbered = Regex.Match(trimmed, @"^(\d{1,3})[.)]\s+(.{3,})$");
        if (numbered.Success)
        {
            heading = numbered.Groups[2].Value.Trim();
            level = 1;
            return true;
        }

        if (trimmed.Length >= 3 && trimmed.Length <= 80
            && !trimmed.Contains('.')
            && trimmed == trimmed.ToUpperInvariant()
            && trimmed.Any(char.IsLetter))
        {
            heading = trimmed;
            level = 2;
            return true;
        }

        heading = string.Empty;
        level = 0;
        return false;
    }

    private static IReadOnlyList<DocumentChunk> SplitOversized(
        KnowledgeDocument document,
        string joined,
        string heading,
        ChunkingOptions options,
        int startOrder,
        ref CancellationToken ct)
    {
        var pieces = new List<DocumentChunk>();
        var chunkSize = Math.Max(1, options.ChunkSize);
        var prefix = heading.Length > 0 ? heading + "\n" : string.Empty;
        var prefixLen = prefix.Length;
        var offset = prefixLen;
        var order = startOrder;

        while (offset < joined.Length)
        {
            ct.ThrowIfCancellationRequested();
            var length = Math.Min(chunkSize, joined.Length - offset);
            var content = joined.Substring(offset, length).Trim();
            if (content.Length >= options.MinChunkSize)
            {
                pieces.Add(CreateChunk(document, content, order, options, heading: heading));
                order++;
            }

            offset += chunkSize;
        }

        return pieces;
    }
}
