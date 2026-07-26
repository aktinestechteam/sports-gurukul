using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.AcademySearchDiscovery.Queries.NearbyAcademies;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.UnitTests.Features.AcademySearchDiscovery;

public class NearbyAcademiesQueryHandlerTests
{
    private readonly Mock<IAcademySearchRepository> _academySearchRepositoryMock;
    private readonly Mock<ILogger<NearbyAcademiesQueryHandler>> _loggerMock;
    private readonly NearbyAcademiesQueryHandler _handler;

    public NearbyAcademiesQueryHandlerTests()
    {
        _academySearchRepositoryMock = new Mock<IAcademySearchRepository>();
        _loggerMock = new Mock<ILogger<NearbyAcademiesQueryHandler>>();
        _handler = new NearbyAcademiesQueryHandler(
            _academySearchRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidCoordinates_ReturnsNearbyAcademies()
    {
        var academies = new List<Academy>
        {
            CreateTestAcademy("Academy A", 19.0760m, 72.8777m),
            CreateTestAcademy("Academy B", 19.0800m, 72.8800m),
            CreateTestAcademy("Academy C", 19.0900m, 72.8900m)
        };

        _academySearchRepositoryMock
            .Setup(r => r.GetNearbyAcademiesAsync(
                19.0760m, 72.8777m, 10m, 20, It.IsAny<CancellationToken>()))
            .ReturnsAsync(academies);

        var result = await _handler.Handle(
            new NearbyAcademiesQuery
            {
                Latitude = 19.0760m,
                Longitude = 72.8777m,
                RadiusKm = 10m,
                Limit = 20
            },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(3);
    }

    [Fact]
    public async Task Handle_WithNoNearbyAcademies_ReturnsEmptyList()
    {
        _academySearchRepositoryMock
            .Setup(r => r.GetNearbyAcademiesAsync(
                19.0760m, 72.8777m, 10m, 20, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Academy>());

        var result = await _handler.Handle(
            new NearbyAcademiesQuery
            {
                Latitude = 19.0760m,
                Longitude = 72.8777m,
                RadiusKm = 10m,
                Limit = 20
            },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    private static Academy CreateTestAcademy(string name, decimal latitude, decimal longitude) => new()
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
            Latitude = latitude,
            Longitude = longitude,
            City = "Mumbai",
            State = "Maharashtra",
            Country = "India"
        }
    };
}
