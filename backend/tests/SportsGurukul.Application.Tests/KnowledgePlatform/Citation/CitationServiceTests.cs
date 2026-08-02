using FluentAssertions;
using SportsGurukul.Application.Features.KnowledgePlatform.Models;
using SportsGurukul.Application.Features.KnowledgePlatform.Services;
using ModelsCitation = SportsGurukul.Application.Features.KnowledgePlatform.Models.Citation;

namespace SportsGurukul.Application.Tests.KnowledgePlatform.Citation;

public class CitationServiceTests
{
    private readonly CitationService _citationService;

    public CitationServiceTests()
    {
        _citationService = new CitationService();
    }

    [Fact]
    public void CreateCitation_GeneratesCitationFromSearchResult()
    {
        var result = new SearchResult(
            DocumentId: "doc-1",
            ChunkId: "doc-1_chunk_0",
            Content: "Sports training methods and techniques for optimal athletic performance.",
            Score: 0.95,
            DocumentName: "Training Manual.pdf",
            Format: DocumentFormat.Pdf,
            PageNumber: 42,
            Section: "Chapter 3: Advanced Training",
            Metadata: null,
            Citation: new ModelsCitation("", null, null, "", 0, null, null)
        );

        var citation = _citationService.CreateCitation(result);

        citation.Should().NotBeNull();
        citation.DocumentName.Should().Be("Training Manual.pdf");
        citation.ChunkId.Should().Be("doc-1_chunk_0");
        citation.Section.Should().Be("Chapter 3: Advanced Training");
        citation.Excerpt.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void CreateCitations_GeneratesMultipleCitations()
    {
        var results = new List<SearchResult>
        {
            new("doc-1", "c1", "Content about training.", 0.95, "Doc1.pdf",
                DocumentFormat.Pdf, 1, "Intro", null,
                new ModelsCitation("", null, null, "", 0, null, null)),
            new("doc-2", "c2", "Content about nutrition.", 0.85, "Doc2.pdf",
                DocumentFormat.Pdf, 5, "Nutrition", null,
                new ModelsCitation("", null, null, "", 0, null, null))
        };

        var citations = _citationService.CreateCitations(results);

        citations.Should().HaveCount(2);
        citations[0].DocumentName.Should().Be("Doc1.pdf");
        citations[1].DocumentName.Should().Be("Doc2.pdf");
    }

    [Fact]
    public void ToMarkdown_FormatsCitationsAsMarkdown()
    {
        var citations = new List<ModelsCitation>
        {
            new("Training Manual.pdf", "Chapter 3", 42, "chunk_0", 0.95,
                "https://docs.example.com/training.pdf", "Excerpt content here...")
        };

        var markdown = _citationService.ToMarkdown(citations);

        markdown.Should().Contain("## References");
        markdown.Should().Contain("Training Manual.pdf");
        markdown.Should().Contain("Chapter 3");
        markdown.Should().Contain("95.0%");
    }

    [Fact]
    public void ToJson_FormatsCitationsAsJson()
    {
        var citations = new List<ModelsCitation>
        {
            new("Doc.pdf", "Section A", 10, "chunk_1", 0.92, null, "Sample excerpt")
        };

        var json = _citationService.ToJson(citations);

        json.Should().Contain("citations");
        json.Should().Contain("Doc.pdf");
        json.Should().Contain("Section A");
    }

    [Fact]
    public void CreateCitation_IncludesExcerptTruncatedTo200Chars()
    {
        var longContent = string.Join(" ", Enumerable.Repeat("This is a very long sentence to test citation excerpt truncation behavior.", 20));
        var result = new SearchResult(
            "doc-1", "chunk_0", longContent, 0.90, "LongDoc.pdf",
            DocumentFormat.Pdf, null, null, null,
            new ModelsCitation("", null, null, "", 0, null, null));

        var citation = _citationService.CreateCitation(result);

        citation.Excerpt.Should().NotBeNull();
        citation.Excerpt!.Length.Should().BeLessOrEqualTo(203);
    }
}
