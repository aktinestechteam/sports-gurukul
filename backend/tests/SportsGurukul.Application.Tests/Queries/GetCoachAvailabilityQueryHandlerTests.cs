using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.CoachManagement.Queries.GetCoachAvailability;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Tests.Queries;

public class GetCoachAvailabilityQueryHandlerTests
{
    private readonly Mock<ICoachAvailabilityRepository> _availabilityRepositoryMock = TestMocks.CreateCoachAvailabilityRepository();
    private readonly Mock<ILogger<GetCoachAvailabilityQueryHandler>> _loggerMock = TestMocks.CreateLogger<GetCoachAvailabilityQueryHandler>();
    private readonly GetCoachAvailabilityQueryHandler _handler;

    public GetCoachAvailabilityQueryHandlerTests()
    {
        _handler = new GetCoachAvailabilityQueryHandler(
            _availabilityRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_AvailabilityNotFound_ReturnsFailure()
    {
        _availabilityRepositoryMock.Setup(r => r.GetByCoachIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CoachAvailability?)null);

        var result = await _handler.Handle(new GetCoachAvailabilityQuery { CoachId = Guid.NewGuid() }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Availability not found for the given coach.");
    }

    [Fact]
    public async Task Handle_AvailabilityFound_ReturnsSuccess()
    {
        var coachId = Guid.NewGuid();
        var availability = TestDataBuilder.CreateCoachAvailability(coachId);

        _availabilityRepositoryMock.Setup(r => r.GetByCoachIdAsync(coachId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(availability);

        var result = await _handler.Handle(new GetCoachAvailabilityQuery { CoachId = coachId }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Id.Should().Be(availability.Id);
        result.Value.OnlineAvailable.Should().Be(availability.OnlineAvailable);
        result.Value.OfflineAvailable.Should().Be(availability.OfflineAvailable);
        result.Value.TravelDistance.Should().Be(availability.TravelDistance);
    }
}
