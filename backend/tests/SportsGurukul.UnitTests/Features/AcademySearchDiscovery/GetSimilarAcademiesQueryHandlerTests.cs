using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.AcademySearchDiscovery.Queries.GetSimilarAcademies;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.UnitTests.Features.AcademySearchDiscovery;

public class GetSimilarAcademiesQueryHandlerTests
{
    private readonly Mock<IAcademySearchRepository> _academySearchRepositoryMock;
    private readonly Mock<IAcademyRepository> _academyRepositoryMock;
    private readonly Mock<ILogger<GetSimilarAcademiesQueryHandler>> _loggerMock;
    private readonly GetSimilarAcademiesQueryHandler _handler;

    private readonly Guid _sourceAcademyId = Guid.NewGuid();

    public GetSimilarAcademiesQueryHandlerTests()
    {
        _academySearchRepositoryMock = new Mock<IAcademySearchRepository>();
        _academyRepositoryMock = new Mock<IAcademyRepository>();
        _loggerMock = new Mock<ILogger<GetSimilarAcademiesQueryHandler>>();
        _handler = new GetSimilarAcademiesQueryHandler(
            _academySearchRepositoryMock.Object,
            _academyRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidAcademyId_ReturnsSimilarAcademies()
    {
        var sourceAcademy = CreateTestAcademy("Source Academy", new List<string> { "Cricket", "Football" });
        var candidates = new List<Academy>
        {
            CreateTestAcademy("Similar A", new List<string> { "Cricket", "Football" }),
            CreateTestAcademy("Similar B", new List<string> { "Cricket" }),
            CreateTestAcademy("Similar C", new List<string> { "Football" })
        };

        _academyRepositoryMock
            .Setup(r => r.GetByIdWithDetailsAsync(_sourceAcademyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sourceAcademy);
        _academySearchRepositoryMock
            .Setup(r => r.GetSimilarAcademiesAsync(_sourceAcademyId, 5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(candidates);

        var result = await _handler.Handle(
            new GetSimilarAcademiesQuery { AcademyId = _sourceAcademyId, Limit = 5 },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(3);
    }

    [Fact]
    public async Task Handle_WithNonExistentAcademy_ReturnsFailure()
    {
        _academyRepositoryMock
            .Setup(r => r.GetByIdWithDetailsAsync(_sourceAcademyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Academy?)null);

        var result = await _handler.Handle(
            new GetSimilarAcademiesQuery { AcademyId = _sourceAcademyId },
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Source academy not found.");
    }

    private static Academy CreateTestAcademy(string name, List<string> sportNames) => new()
    {
        Id = Guid.NewGuid(),
        Name = name,
        AcademyCode = $"ACD-{Guid.NewGuid().ToString()[8..16].ToUpper()}",
        Email = $"{name.ToLower().Replace(" ", "")}@example.com",
        Phone = "9876543210",
        Status = AcademyStatus.Active,
        VerificationStatus = VerificationStatus.Verified,
        AcademySports = sportNames.Select(s => new AcademySport
        {
            Id = Guid.NewGuid(),
            Sport = new Sport { Id = Guid.NewGuid(), Name = s, Code = s[..3].ToUpper(), SportCategoryId = Guid.NewGuid() }
        }).ToList(),
        Facilities = new List<AcademyFacility>
        {
            new() { Id = Guid.NewGuid(), FacilityName = "Main Court", FacilityType = AcademyFacilityType.Court }
        },
        Contact = new AcademyContact
        {
            Id = Guid.NewGuid(),
            City = "Mumbai",
            State = "Maharashtra",
            Country = "India"
        }
    };
}
