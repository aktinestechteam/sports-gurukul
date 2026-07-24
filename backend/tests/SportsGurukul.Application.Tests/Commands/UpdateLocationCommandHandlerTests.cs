using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.CoachManagement.Commands.UpdateLocation;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Tests.Commands;

public class UpdateLocationCommandHandlerTests
{
    private readonly Mock<ICoachRepository> _coachRepositoryMock = TestMocks.CreateCoachRepository();
    private readonly Mock<IRepository<CoachLocation>> _locationRepositoryMock = TestMocks.CreateCoachLocationRepository();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = TestMocks.CreateUnitOfWork();
    private readonly Mock<ILogger<UpdateLocationCommandHandler>> _loggerMock = TestMocks.CreateLogger<UpdateLocationCommandHandler>();
    private readonly UpdateLocationCommandHandler _handler;

    public UpdateLocationCommandHandlerTests()
    {
        _handler = new UpdateLocationCommandHandler(
            _coachRepositoryMock.Object,
            _locationRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_CoachNotFound_ReturnsFailure()
    {
        _coachRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Coach?)null);

        var result = await _handler.Handle(new UpdateLocationCommand { CoachId = Guid.NewGuid() }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Coach not found.");
    }

    [Fact]
    public async Task Handle_InvalidLatitude_ReturnsFailure()
    {
        var coachId = Guid.NewGuid();
        var coach = TestDataBuilder.CreateCoach(id: coachId);

        _coachRepositoryMock.Setup(r => r.GetByIdAsync(coachId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(coach);

        var result = await _handler.Handle(new UpdateLocationCommand
        {
            CoachId = coachId,
            Country = "India",
            Latitude = 95m
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Latitude must be between -90 and 90.");
    }

    [Fact]
    public async Task Handle_InvalidLongitude_ReturnsFailure()
    {
        var coachId = Guid.NewGuid();
        var coach = TestDataBuilder.CreateCoach(id: coachId);

        _coachRepositoryMock.Setup(r => r.GetByIdAsync(coachId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(coach);

        var result = await _handler.Handle(new UpdateLocationCommand
        {
            CoachId = coachId,
            Country = "India",
            Longitude = -200m
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Longitude must be between -180 and 180.");
    }

    [Fact]
    public async Task Handle_ValidLocation_CreatesAndReturnsSuccess()
    {
        var coachId = Guid.NewGuid();
        var coach = TestDataBuilder.CreateCoach(id: coachId);

        _coachRepositoryMock.Setup(r => r.GetByIdAsync(coachId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(coach);
        _locationRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<CoachLocation, bool>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CoachLocation>());
        _locationRepositoryMock.Setup(r => r.AddAsync(It.IsAny<CoachLocation>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CoachLocation());
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(new UpdateLocationCommand
        {
            CoachId = coachId,
            Country = "India",
            State = "Maharashtra",
            City = "Mumbai",
            District = "Mumbai City",
            Latitude = 19.076m,
            Longitude = 72.877m
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Country.Should().Be("India");
        result.Value.State.Should().Be("Maharashtra");
        result.Value.City.Should().Be("Mumbai");
        result.Value.District.Should().Be("Mumbai City");
        result.Value.Latitude.Should().Be(19.076m);
        result.Value.Longitude.Should().Be(72.877m);
        _locationRepositoryMock.Verify(r => r.AddAsync(It.IsAny<CoachLocation>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
