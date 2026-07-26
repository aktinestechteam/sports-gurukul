using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.AcademySearchDiscovery.Queries.GetAcademySuggestions;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.UnitTests.Features.AcademySearchDiscovery;

public class GetAcademySuggestionsQueryHandlerTests
{
    private readonly Mock<IAcademySearchRepository> _academySearchRepositoryMock;
    private readonly Mock<ILogger<GetAcademySuggestionsQueryHandler>> _loggerMock;
    private readonly GetAcademySuggestionsQueryHandler _handler;

    public GetAcademySuggestionsQueryHandlerTests()
    {
        _academySearchRepositoryMock = new Mock<IAcademySearchRepository>();
        _loggerMock = new Mock<ILogger<GetAcademySuggestionsQueryHandler>>();
        _handler = new GetAcademySuggestionsQueryHandler(
            _academySearchRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidPrefix_ReturnsSuggestions()
    {
        var academies = new List<Academy>
        {
            CreateTestAcademy("Cricket Academy"),
            CreateTestAcademy("Cricket Pro Academy"),
            CreateTestAcademy("Cricket Zone")
        };

        _academySearchRepositoryMock
            .Setup(r => r.GetAutocompleteSuggestionsAsync("cricket", 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(academies);

        var result = await _handler.Handle(
            new GetAcademySuggestionsQuery { Prefix = "cricket", Limit = 10 },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(3);
        result.Value!.First().Name.Should().Be("Cricket Academy");
    }

    [Fact]
    public async Task Handle_WithEmptyPrefix_ReturnsEmptyList()
    {
        _academySearchRepositoryMock
            .Setup(r => r.GetAutocompleteSuggestionsAsync(string.Empty, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Academy>());

        var result = await _handler.Handle(
            new GetAcademySuggestionsQuery { Prefix = string.Empty },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    private static Academy CreateTestAcademy(string name) => new()
    {
        Id = Guid.NewGuid(),
        Name = name,
        AcademyCode = $"ACD-{Guid.NewGuid().ToString()[8..16].ToUpper()}",
        Email = $"{name.ToLower().Replace(" ", "")}@example.com",
        Phone = "9876543210",
        Status = AcademyStatus.Active,
        VerificationStatus = VerificationStatus.Verified,
        Contact = new AcademyContact
        {
            Id = Guid.NewGuid(),
            City = "Mumbai",
            State = "Maharashtra",
            Country = "India"
        }
    };
}
