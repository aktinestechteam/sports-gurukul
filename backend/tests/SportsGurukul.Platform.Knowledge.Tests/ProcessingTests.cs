using System.Text;
using SportsGurukul.Platform.Knowledge.Models;
using SportsGurukul.Platform.Knowledge.Processing;
using SportsGurukul.Platform.Knowledge.Processing.TextExtraction;
using Xunit;

namespace SportsGurukul.Platform.Knowledge.Tests;

public class ProcessingTests
{
    [Fact]
    public void LanguageDetector_Detects_Devanagari_AsHindi()
    {
        var detector = new LanguageDetector();

        var language = detector.Detect("यह एक प्रशिक्षण दस्तावेज़ है जिसमें खिलाड़ियों की जानकारी है।");

        Assert.Equal("hi", language);
    }

    [Fact]
    public void LanguageDetector_Detects_English_ByStopWords()
    {
        var detector = new LanguageDetector();

        var language = detector.Detect("the quick brown fox and the lazy dog are over there");

        Assert.Equal("en", language);
    }

    [Fact]
    public async Task PiiDetector_Redacts_EmailAndPhone()
    {
        var detector = new PiiDetector();
        var text = "Contact coach at coach@example.com or call 9876543210 today.";

        var findings = await detector.DetectAsync(text);
        var redacted = detector.Redact(text, findings);

        Assert.Contains(findings, f => f.Type == "Email");
        Assert.Contains(findings, f => f.Type == "Phone");
        Assert.DoesNotContain("coach@example.com", redacted);
        Assert.DoesNotContain("9876543210", redacted);
        Assert.Contains("<[email redacted]>", redacted);
        Assert.Contains("<[phone redacted]>", redacted);
    }

    [Fact]
    public void Fingerprinter_Canonicalizes_WhitespaceAndCase()
    {
        var fingerprinter = new DocumentFingerprinter();

        var a = fingerprinter.Compute("  Cricket   Training\nManual  ");
        var b = fingerprinter.Compute("cricket training manual");

        Assert.Equal(a.Value, b.Value);
        Assert.Equal(64, a.Value.Length);
    }

    [Fact]
    public async Task Deduplicator_ReturnsMatchingFingerprint()
    {
        var deduplicator = new Deduplicator();
        var fingerprint = new DocumentFingerprint("sha256-normalized-v1", "abc123");

        var match = await deduplicator.FindDuplicateAsync(
            fingerprint, "sports", "t1", new[] { "abc123", "def456" });

        Assert.Equal("abc123", match);
    }

    [Fact]
    public async Task DocumentProcessor_ProcessesPlainText_AndRedactsPii()
    {
        var processor = BuildProcessor();
        var text = "The athlete's email is coach@example.com and the training schedule is weekly.";
        var document = TestHarness.Document("Player Notes", text, "t1", "sports");
        var content = Encoding.UTF8.GetBytes(text);

        var processed = await processor.ProcessAsync(document, content);

        Assert.Equal(DocumentIngestionState.Extracted, processed.State);
        Assert.NotEmpty(processed.SafeText);
        Assert.DoesNotContain("coach@example.com", processed.SafeText);
        Assert.Contains("<[email redacted]>", processed.SafeText);
        Assert.Equal("en", processed.Language);
        Assert.Equal("Sport", processed.Classification.Category);
        Assert.NotEmpty(processed.Fingerprint.Value);
        Assert.NotEmpty(processed.Metadata);
    }

    [Fact]
    public async Task DocumentProcessor_UnknownContentType_ReturnsFailed()
    {
        var processor = BuildProcessor();
        var document = new KnowledgeDocument(
            Guid.NewGuid(),
            "Doc",
            "application/octet-stream",
            DocumentType.Other,
            FileName: "doc.bin");

        var processed = await processor.ProcessAsync(document);

        Assert.Equal(DocumentIngestionState.Failed, processed.State);
        Assert.NotNull(processed.Error);
    }

    private static DocumentProcessor BuildProcessor() =>
        new(
            new TextExtractorRegistry(),
            new LanguageDetector(),
            new PiiDetector(),
            new ContentClassifier(),
            new DocumentFingerprinter());
}
