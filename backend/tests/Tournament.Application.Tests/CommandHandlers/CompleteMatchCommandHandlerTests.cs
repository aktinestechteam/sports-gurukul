using MediatR;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.TournamentManagement.Commands.CompleteMatch;
using SportsGurukul.Application.Features.TournamentManagement.Services;
using SportsGurukul.Domain.Enums;

namespace Tournament.Application.Tests.CommandHandlers;

public class CompleteMatchCommandHandlerTests
{
    private readonly Mock<IMatchRepository> _matchRepositoryMock = MockRepositoryBuilder.CreateMatchRepository();
    private readonly Mock<IScoringService> _scoringServiceMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = MockUnitOfWorkBuilder.Create();
    private readonly Mock<ILogger<CompleteMatchCommandHandler>> _loggerMock = MockLoggerBuilder.Create<CompleteMatchCommandHandler>();
    private readonly CompleteMatchCommandHandler _handler;

    public CompleteMatchCommandHandlerTests()
    {
        _handler = new CompleteMatchCommandHandler(
            _matchRepositoryMock.Object,
            _scoringServiceMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsSuccess()
    {
        var match = TestDataBuilder.CreateMatch(status: MatchStatus.InProgress);
        _matchRepositoryMock.Setup(r => r.GetByIdAsync(match.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(match);
        _scoringServiceMock.Setup(s => s.CompleteMatchAsync(match, It.IsAny<CancellationToken>()))
            .ReturnsAsync(match);

        var result = await _handler.Handle(new CompleteMatchCommand
        {
            MatchId = match.Id
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _scoringServiceMock.Verify(s => s.CompleteMatchAsync(match, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_MatchNotFound_ReturnsFailure()
    {
        var result = await _handler.Handle(new CompleteMatchCommand
        {
            MatchId = Guid.NewGuid()
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Match not found.");
    }

    [Fact]
    public async Task Handle_MatchNotInProgress_ReturnsFailure()
    {
        var match = TestDataBuilder.CreateMatch(status: MatchStatus.Scheduled);
        _matchRepositoryMock.Setup(r => r.GetByIdAsync(match.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(match);

        var result = await _handler.Handle(new CompleteMatchCommand
        {
            MatchId = match.Id
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Only in-progress matches can be completed.");
    }
}
