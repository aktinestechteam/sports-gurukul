using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.AthleteManagement.Queries.GetAthleteByUserId;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Tests.Queries;

public class GetAthleteByUserIdQueryHandlerTests
{
    private readonly Mock<IAthleteRepository> _athleteRepositoryMock = TestMocks.CreateAthleteRepository();
    private readonly Mock<ILogger<GetAthleteByUserIdQueryHandler>> _loggerMock = TestMocks.CreateLogger<GetAthleteByUserIdQueryHandler>();
    private readonly GetAthleteByUserIdQueryHandler _handler;

    public GetAthleteByUserIdQueryHandlerTests()
    {
        _handler = new GetAthleteByUserIdQueryHandler(_athleteRepositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_AthleteNotFound_ReturnsFailure()
    {
        _athleteRepositoryMock.Setup(r => r.GetByUserIdWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Athlete?)null);

        var result = await _handler.Handle(new GetAthleteByUserIdQuery { UserId = Guid.NewGuid() }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Athlete profile not found for this user.");
    }

    [Fact]
    public async Task Handle_AthleteExists_ReturnsSuccess()
    {
        var athlete = TestDataBuilder.CreateAthleteWithDetails();
        _athleteRepositoryMock.Setup(r => r.GetByUserIdWithDetailsAsync(athlete.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(athlete);

        var result = await _handler.Handle(new GetAthleteByUserIdQuery { UserId = athlete.UserId }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.UserId.Should().Be(athlete.UserId);
    }
}
