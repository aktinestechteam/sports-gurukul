using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.AcademyManagement.Queries.GetFacilities;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.UnitTests.Features.AcademyManagement.Queries;

public class GetFacilitiesQueryHandlerTests
{
    private readonly Mock<IAcademyFacilityRepository> _facilityRepositoryMock;
    private readonly Mock<ILogger<GetFacilitiesQueryHandler>> _loggerMock;
    private readonly GetFacilitiesQueryHandler _handler;

    public GetFacilitiesQueryHandlerTests()
    {
        _facilityRepositoryMock = new Mock<IAcademyFacilityRepository>();
        _loggerMock = new Mock<ILogger<GetFacilitiesQueryHandler>>();
        _handler = new GetFacilitiesQueryHandler(
            _facilityRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_HasFacilities_ReturnsList()
    {
        var academyId = Guid.NewGuid();
        var facilities = new List<AcademyFacility>
        {
            new()
            {
                Id = Guid.NewGuid(),
                AcademyId = academyId,
                FacilityName = "Basketball Court",
                FacilityType = AcademyFacilityType.Court,
                IndoorOutdoor = "Indoor",
                Capacity = 30,
                Available = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                AcademyId = academyId,
                FacilityName = "Swimming Pool",
                FacilityType = AcademyFacilityType.Pool,
                IndoorOutdoor = "Outdoor",
                Capacity = 50,
                Available = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        _facilityRepositoryMock
            .Setup(r => r.GetByAcademyIdAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(facilities);

        var query = new GetFacilitiesQuery { AcademyId = academyId };

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value![0].FacilityName.Should().Be("Basketball Court");
        result.Value[0].FacilityType.Should().Be("Court");
        result.Value[1].FacilityName.Should().Be("Swimming Pool");
    }

    [Fact]
    public async Task Handle_NoFacilities_ReturnsEmptyList()
    {
        var academyId = Guid.NewGuid();

        _facilityRepositoryMock
            .Setup(r => r.GetByAcademyIdAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AcademyFacility>());

        var query = new GetFacilitiesQuery { AcademyId = academyId };

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }
}
