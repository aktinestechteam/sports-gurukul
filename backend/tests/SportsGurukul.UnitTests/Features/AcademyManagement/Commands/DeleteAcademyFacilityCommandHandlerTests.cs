using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.AcademyManagement.Commands.DeleteFacility;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.UnitTests.Features.AcademyManagement.Commands;

public class DeleteAcademyFacilityCommandHandlerTests
{
    private readonly Mock<IAcademyFacilityRepository> _facilityRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<DeleteFacilityCommandHandler>> _loggerMock;
    private readonly DeleteFacilityCommandHandler _handler;

    public DeleteAcademyFacilityCommandHandlerTests()
    {
        _facilityRepositoryMock = new Mock<IAcademyFacilityRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<DeleteFacilityCommandHandler>>();
        _handler = new DeleteFacilityCommandHandler(
            _facilityRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_SoftDeletesFacility()
    {
        var facilityId = Guid.NewGuid();
        var facility = CreateFacility(facilityId, isDeleted: false);

        _facilityRepositoryMock
            .Setup(r => r.GetByIdAsync(facilityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(facility);
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new DeleteFacilityCommand { FacilityId = facilityId };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        facility.IsDeleted.Should().BeTrue();
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

        var command = new DeleteFacilityCommand { FacilityId = facilityId };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not found");
    }

    [Fact]
    public async Task Handle_AlreadyDeleted_ReturnsFailure()
    {
        var facilityId = Guid.NewGuid();
        var facility = CreateFacility(facilityId, isDeleted: true);

        _facilityRepositoryMock
            .Setup(r => r.GetByIdAsync(facilityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(facility);

        var command = new DeleteFacilityCommand { FacilityId = facilityId };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("already deleted");
        _facilityRepositoryMock.Verify(r => r.Update(It.IsAny<AcademyFacility>()), Times.Never);
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
