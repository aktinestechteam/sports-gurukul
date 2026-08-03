using System.Text;
using SportsGurukul.Platform.Knowledge.Abstractions;
using SportsGurukul.Platform.Knowledge.Models;

namespace SportsGurukul.Platform.Knowledge.Processing.TextExtraction;

public sealed class PdfTextExtractor : IDocumentTextExtractor
{
    public DocumentType SupportedType => DocumentType.Pdf;

    public bool CanHandle(string contentType) =>
        DocumentTypeResolver.FromContentType(contentType) == DocumentType.Pdf;

    public Task<ExtractedDocumentText> ExtractAsync(
        KnowledgeDocument document,
        byte[] content,
        CancellationToken ct)
    {
        try
        {
            return Task.FromResult(ExtractFromStream(content, document));
        }
        catch (InvalidDataException)
        {
            throw new InvalidDataException(
                $"'{document.FileName}' is not a valid PDF document. " +
                "Provide a text-based PDF or register a third-party extractor/OCR engine for scanned documents.");
        }
    }

    internal static ExtractedDocumentText ExtractFromStream(byte[] content, KnowledgeDocument document)
    {
        var contentStream = content.AsSpan();

        if (!TryFindFirstMarker(contentStream, out var headerEnd))
        {
            throw new InvalidDataException("Missing PDF header.");
        }

        var offset = headerEnd;
        var body = new StringBuilder();
        var sections = new List<DocumentSection>();

        while (offset < content.Length)
        {
            if (TryFindMarker(contentStream, offset, "stream", out var streamStart))
            {
                if (TryFindMarker(contentStream, streamStart, "endstream", out var streamEnd))
                {
                    var length = streamEnd - streamStart;
                    if (length > 0)
                    {
                        var raw = contentStream.Slice(streamStart, length);
                        var decoded = DecodeStream(raw);
                        var clean = CleanText(decoded);
                        if (clean.Length > 0)
                        {
                            body.Append(clean).Append('\n');
                        }
                    }

                    offset = streamEnd;
                }
                else
                {
                    break;
                }
            }
            else
            {
                break;
            }
        }

        if (body.Length == 0)
        {
            throw new InvalidDataException("PDF contains no extractable text streams.");
        }

        var text = body.ToString();
        if (sections.Count == 0)
        {
            sections.Add(new DocumentSection("Document", 1, 0, text.Length) { Content = text });
        }

        return new ExtractedDocumentText(text, sections);
    }

    private static bool TryFindFirstMarker(ReadOnlySpan<byte> span, out int end)
    {
        for (var i = 0; i < span.Length - 1; i++)
        {
            if (span[i] == (byte)'\n' && span[i + 1] == (byte)'%')
            {
                end = i + 1;
                return true;
            }
        }

        end = 0;
        return false;
    }

    private static bool TryFindMarker(
        ReadOnlySpan<byte> span,
        int from,
        string marker,
        out int position)
    {
        var markerBytes = System.Text.Encoding.ASCII.GetBytes(marker);
        for (var i = from; i < span.Length - markerBytes.Length; i++)
        {
            var match = true;
            for (var j = 0; j < markerBytes.Length; j++)
            {
                if (span[i + j] != markerBytes[j])
                {
                    match = false;
                    break;
                }
            }

            if (match)
            {
                position = i + markerBytes.Length;
                return true;
            }
        }

        position = 0;
        return false;
    }

    private static string DecodeStream(ReadOnlySpan<byte> raw)
    {
        if (!HasFlatePrefix(raw))
        {
            return System.Text.Encoding.Latin1.GetString(raw.ToArray());
        }

        try
        {
            var data = raw[SkipPrefixBytes(raw)..];
            using var input = new MemoryStream(data.ToArray());
            using var zlib = new System.IO.Compression.ZLibStream(input, System.IO.Compression.CompressionMode.Decompress);
            using var reader = new StreamReader(zlib, System.Text.Encoding.Latin1);
            return reader.ReadToEnd();
        }
        catch
        {
            return string.Empty;
        }
    }

    private static bool HasFlatePrefix(ReadOnlySpan<byte> raw) =>
        StartsWithAscii(raw, "FLATE") || StartsWithAscii(raw, "FI") || raw.Length >= 2 && raw[0] == 0x78 && (raw[1] == 0x9C || raw[1] == 0x01 || raw[1] == 0xDA);

    private static int SkipPrefixBytes(ReadOnlySpan<byte> raw)
    {
        var i = 0;
        while (i < raw.Length && i < 64)
        {
            if (raw[i] >= (byte)'A' && raw[i] <= (byte)'Z')
            {
                i++;
            }
            else
            {
                break;
            }
        }

        while (i < raw.Length && (raw[i] == (byte)'\r' || raw[i] == (byte)'\n' || raw[i] == (byte)' '))
        {
            i++;
        }

        return i;
    }

    private static bool StartsWithAscii(ReadOnlySpan<byte> span, string value)
    {
        if (span.Length < value.Length)
        {
            return false;
        }

        for (var i = 0; i < value.Length; i++)
        {
            if (char.ToUpperInvariant((char)span[i]) != value[i])
            {
                return false;
            }
        }

        return true;
    }

    private static string CleanText(string raw)
    {
        var builder = new StringBuilder(raw.Length);
        var lastWasSpace = true;
        foreach (var c in raw)
        {
            if (c == '\r' || c == '\n')
            {
                if (!lastWasSpace)
                {
                    builder.Append(' ');
                    lastWasSpace = true;
                }
            }
            else if (char.IsControl(c))
            {
                builder.Append(' ');
            }
            else
            {
                builder.Append(c);
                lastWasSpace = c == ' ';
            }
        }

        return builder.ToString().Trim();
    }
}
