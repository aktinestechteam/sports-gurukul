using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.AcademyManagement.Commands.RestoreFacility;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.UnitTests.Features.AcademyManagement.Commands;

public class RestoreAcademyFacilityCommandHandlerTests
{
    private readonly Mock<IAcademyFacilityRepository> _facilityRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<RestoreFacilityCommandHandler>> _loggerMock;
    private readonly RestoreFacilityCommandHandler _handler;

    public RestoreAcademyFacilityCommandHandlerTests()
    {
        _facilityRepositoryMock = new Mock<IAcademyFacilityRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<RestoreFacilityCommandHandler>>();
        _handler = new RestoreFacilityCommandHandler(
            _facilityRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_RestoresFacility()
    {
        var facilityId = Guid.NewGuid();
        var facility = CreateFacility(facilityId, isDeleted: true);

        _facilityRepositoryMock
            .Setup(r => r.GetByIdAsync(facilityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(facility);
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new RestoreFacilityCommand { FacilityId = facilityId };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Id.Should().Be(facilityId);
        facility.IsDeleted.Should().BeFalse();
        _facilityRepositoryMock.Verify(r => r.Update(facility), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_FacilityNotFound_ReturnsFailure()
    {
        var facilityId = Guid.NewGuid();

        _facilityRepositoryMock
            .Setup(r => r.GetByIdAsync(facilityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((AcademyFacility?)null);

        var command = new RestoreFacilityCommand { FacilityId = facilityId };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not found");
    }

    [Fact]
    public async Task Handle_NotDeleted_ReturnsFailure()
    {
        var facilityId = Guid.NewGuid();
        var facility = CreateFacility(facilityId, isDeleted: false);

        _facilityRepositoryMock
            .Setup(r => r.GetByIdAsync(facilityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(facility);

        var command = new RestoreFacilityCommand { FacilityId = facilityId };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not deleted");
    }

    private static AcademyFacility CreateFacility(Guid id, bool isDeleted) => new()
    {
        Id = id,
        AcademyId = Guid.NewGuid(),
        FacilityName = "Test Facility",
        FacilityType = AcademyFacilityType.Court,
        IsDeleted = isDeleted,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };
}
