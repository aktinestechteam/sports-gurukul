using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.CoachManagement.Queries.GetCoachByUserId;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Tests.Queries;

public class GetCoachByUserIdQueryHandlerTests
{
    private readonly Mock<ICoachRepository> _coachRepositoryMock = TestMocks.CreateCoachRepository();
    private readonly Mock<ILogger<GetCoachByUserIdQueryHandler>> _loggerMock = TestMocks.CreateLogger<GetCoachByUserIdQueryHandler>();
    private readonly GetCoachByUserIdQueryHandler _handler;

    public GetCoachByUserIdQueryHandlerTests()
    {
        _handler = new GetCoachByUserIdQueryHandler(
            _coachRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_CoachNotFound_ReturnsFailure()
    {
        _coachRepositoryMock.Setup(r => r.GetByUserIdWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Coach?)null);

        var result = await _handler.Handle(new GetCoachByUserIdQuery { UserId = Guid.NewGuid() }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Coach not found for the given user.");
    }

    [Fact]
    public async Task Handle_CoachFound_ReturnsSuccessWithCorrectDto()
    {
        var coach = TestDataBuilder.CreateCoachWithDetails();

        _coachRepositoryMock.Setup(r => r.GetByUserIdWithDetailsAsync(coach.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(coach);

        var result = await _handler.Handle(new GetCoachByUserIdQuery { UserId = coach.UserId }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Id.Should().Be(coach.Id);
        result.Value.UserId.Should().Be(coach.UserId);
        result.Value.FullName.Should().Be(coach.User.FullName);
        result.Value.Email.Should().Be(coach.User.Email);
    }
}
