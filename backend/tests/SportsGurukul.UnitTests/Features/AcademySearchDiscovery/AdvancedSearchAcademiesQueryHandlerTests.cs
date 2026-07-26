using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.AcademySearchDiscovery.Queries.AdvancedSearchAcademies;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.UnitTests.Features.AcademySearchDiscovery;

public class AdvancedSearchAcademiesQueryHandlerTests
{
    private readonly Mock<IAcademySearchRepository> _academySearchRepositoryMock;
    private readonly Mock<ILogger<AdvancedSearchAcademiesQueryHandler>> _loggerMock;
    private readonly AdvancedSearchAcademiesQueryHandler _handler;

    public AdvancedSearchAcademiesQueryHandlerTests()
    {
        _academySearchRepositoryMock = new Mock<IAcademySearchRepository>();
        _loggerMock = new Mock<ILogger<AdvancedSearchAcademiesQueryHandler>>();
        _handler = new AdvancedSearchAcademiesQueryHandler(
            _academySearchRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidFilters_ReturnsFilteredResults()
    {
        var academies = new List<Academy>
        {
            CreateTestAcademy("Academy A", "Mumbai"),
            CreateTestAcademy("Academy B", "Delhi")
        };

        _academySearchRepositoryMock
            .Setup(r => r.SearchAcademiesAsync(
                It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(),
                It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(),
                It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(),
                It.IsAny<decimal?>(), It.IsAny<decimal?>(), It.IsAny<decimal?>(),
                It.IsAny<string?>(), It.IsAny<string?>(),
                It.IsAny<bool?>(), It.IsAny<bool?>(), It.IsAny<bool?>(),
                It.IsAny<bool?>(), It.IsAny<bool?>(), It.IsAny<bool?>(),
                It.IsAny<bool?>(), It.IsAny<bool?>(), It.IsAny<bool?>(),
                It.IsAny<bool?>(), It.IsAny<bool?>(), It.IsAny<bool?>(),
                It.IsAny<int?>(),
                It.IsAny<decimal?>(), It.IsAny<decimal?>(), It.IsAny<decimal?>(),
                It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<int?>(),
                It.IsAny<bool?>(), It.IsAny<bool?>(),
                It.IsAny<string?>(),
                It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((academies, 2));

        var result = await _handler.Handle(
            new AdvancedSearchAcademiesQuery
            {
                SearchTerm = "cricket",
                City = "Mumbai",
                Page = 1,
                PageSize = 20
            },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().HaveCount(2);
        result.Value.TotalRecords.Should().Be(2);
    }

    [Fact]
    public async Task Handle_WithNoResults_ReturnsEmptyList()
    {
        _academySearchRepositoryMock
            .Setup(r => r.SearchAcademiesAsync(
                It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(),
                It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(),
                It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(),
                It.IsAny<decimal?>(), It.IsAny<decimal?>(), It.IsAny<decimal?>(),
                It.IsAny<string?>(), It.IsAny<string?>(),
                It.IsAny<bool?>(), It.IsAny<bool?>(), It.IsAny<bool?>(),
                It.IsAny<bool?>(), It.IsAny<bool?>(), It.IsAny<bool?>(),
                It.IsAny<bool?>(), It.IsAny<bool?>(), It.IsAny<bool?>(),
                It.IsAny<bool?>(), It.IsAny<bool?>(), It.IsAny<bool?>(),
                It.IsAny<int?>(),
                It.IsAny<decimal?>(), It.IsAny<decimal?>(), It.IsAny<decimal?>(),
                It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<int?>(),
                It.IsAny<bool?>(), It.IsAny<bool?>(),
                It.IsAny<string?>(),
                It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<Academy>(), 0));

        var result = await _handler.Handle(
            new AdvancedSearchAcademiesQuery { SearchTerm = "nonexistent" },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().BeEmpty();
        result.Value.TotalRecords.Should().Be(0);
    }

    [Fact]
    public async Task Handle_WithPagination_ReturnsCorrectPageInfo()
    {
        var academies = new List<Academy>
        {
            CreateTestAcademy("Academy A", "Mumbai")
        };

        _academySearchRepositoryMock
            .Setup(r => r.SearchAcademiesAsync(
                It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(),
                It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(),
                It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(),
                It.IsAny<decimal?>(), It.IsAny<decimal?>(), It.IsAny<decimal?>(),
                It.IsAny<string?>(), It.IsAny<string?>(),
                It.IsAny<bool?>(), It.IsAny<bool?>(), It.IsAny<bool?>(),
                It.IsAny<bool?>(), It.IsAny<bool?>(), It.IsAny<bool?>(),
                It.IsAny<bool?>(), It.IsAny<bool?>(), It.IsAny<bool?>(),
                It.IsAny<bool?>(), It.IsAny<bool?>(), It.IsAny<bool?>(),
                It.IsAny<int?>(),
                It.IsAny<decimal?>(), It.IsAny<decimal?>(), It.IsAny<decimal?>(),
                It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<int?>(),
                It.IsAny<bool?>(), It.IsAny<bool?>(),
                It.IsAny<string?>(),
                It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((academies, 45));

        var result = await _handler.Handle(
            new AdvancedSearchAcademiesQuery { Page = 2, PageSize = 20 },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.TotalPages.Should().Be(3);
        result.Value.CurrentPage.Should().Be(2);
        result.Value.PageSize.Should().Be(20);
        result.Value.HasNext.Should().BeTrue();
        result.Value.HasPrevious.Should().BeTrue();
    }

    private static Academy CreateTestAcademy(string name, string city) => new()
    {
        Id = Guid.NewGuid(),
        Name = name,
        AcademyCode = $"ACD-{Guid.NewGuid().ToString()[..8].ToUpper()}",
        Email = $"{name.ToLower().Replace(" ", "")}@example.com",
        Phone = "9876543210",
        Status = AcademyStatus.Active,
        VerificationStatus = VerificationStatus.Verified,
        Contact = new AcademyContact
        {
            Id = Guid.NewGuid(),
            City = city,
            State = "Maharashtra",
            Country = "India"
        }
    };
}
