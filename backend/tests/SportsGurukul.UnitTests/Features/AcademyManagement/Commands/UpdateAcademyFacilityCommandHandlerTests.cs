using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.AcademyManagement.Commands.UpdateFacility;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.UnitTests.Features.AcademyManagement.Commands;

public class UpdateAcademyFacilityCommandHandlerTests
{
    private readonly Mock<IAcademyFacilityRepository> _facilityRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<UpdateFacilityCommandHandler>> _loggerMock;
    private readonly UpdateFacilityCommandHandler _handler;

    public UpdateAcademyFacilityCommandHandlerTests()
    {
        _facilityRepositoryMock = new Mock<IAcademyFacilityRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<UpdateFacilityCommandHandler>>();
        _handler = new UpdateFacilityCommandHandler(
            _facilityRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsSuccess()
    {
        var academyId = Guid.NewGuid();
        var facilityId = Guid.NewGuid();
        var facility = CreateFacility(facilityId, academyId, "Old Name");

        _facilityRepositoryMock
            .Setup(r => r.GetByIdAsync(facilityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(facility);
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new UpdateFacilityCommand
        {
            FacilityId = facilityId,
            AcademyId = academyId,
            FacilityName = "Updated Facility",
            Capacity = 100
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.FacilityName.Should().Be("Updated Facility");
        result.Value.Capacity.Should().Be(100);
        _facilityRepositoryMock.Verify(r => r.Update(It.IsAny<AcademyFacility>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_FacilityNotFound_ReturnsFailure()
    {
        var facilityId = Guid.NewGuid();
        var academyId = Guid.NewGuid();

        _facilityRepositoryMock
            .Setup(r => r.GetByIdAsync(facilityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((AcademyFacility?)null);

        var command = new UpdateFacilityCommand
        {
            FacilityId = facilityId,
            AcademyId = academyId,
            FacilityName = "Updated Facility"
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not found");
    }

    [Fact]
    public async Task Handle_DeletedFacility_ReturnsFailure()
    {
        var academyId = Guid.NewGuid();
        var facilityId = Guid.NewGuid();
        var facility = CreateFacility(facilityId, academyId, "Facility", isDeleted: true);

        _facilityRepositoryMock
            .Setup(r => r.GetByIdAsync(facilityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(facility);

        var command = new UpdateFacilityCommand
        {
            FacilityId = facilityId,
            AcademyId = academyId,
            FacilityName = "Updated Facility"
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("deleted");
    }

    [Fact]
    public async Task Handle_BelongsToDifferentAcademy_ReturnsFailure()
    {
        var academyId = Guid.NewGuid();
        var otherAcademyId = Guid.NewGuid();
        var facilityId = Guid.NewGuid();
        var facility = CreateFacility(facilityId, otherAcademyId, "Facility");

        _facilityRepositoryMock
            .Setup(r => r.GetByIdAsync(facilityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(facility);

        var command = new UpdateFacilityCommand
        {
            FacilityId = facilityId,
            AcademyId = academyId,
            FacilityName = "Updated Facility"
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("does not belong");
    }

    private static AcademyFacility CreateFacility(Guid id, Guid academyId, string name, bool isDeleted = false) => new()
    {
        Id = id,
        AcademyId = academyId,
        FacilityName = name,
        FacilityType = AcademyFacilityType.Court,
        Available = true,
        IsDeleted = isDeleted,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };
}
