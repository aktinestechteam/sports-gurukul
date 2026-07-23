using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.AthleteManagement.Commands.UpdateRanking;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Tests.Commands;

public class UpdateRankingCommandHandlerTests
{
    private readonly Mock<IAthleteRepository> _athleteRepositoryMock = TestMocks.CreateAthleteRepository();
    private readonly Mock<IRepository<Ranking>> _rankingRepositoryMock = TestMocks.CreateRankingRepository();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = TestMocks.CreateUnitOfWork();
    private readonly Mock<ILogger<UpdateRankingCommandHandler>> _loggerMock = TestMocks.CreateLogger<UpdateRankingCommandHandler>();
    private readonly UpdateRankingCommandHandler _handler;

    public UpdateRankingCommandHandlerTests()
    {
        _handler = new UpdateRankingCommandHandler(
            _athleteRepositoryMock.Object,
            _rankingRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_AthleteNotFound_ReturnsFailure()
    {
        _athleteRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Athlete?)null);

        var result = await _handler.Handle(new UpdateRankingCommand
        {
            AthleteId = Guid.NewGuid()
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Athlete not found.");
    }

    [Fact]
    public async Task Handle_NoExistingRanking_CreatesNewRanking()
    {
        var athleteId = Guid.NewGuid();
        var athlete = TestDataBuilder.CreateAthleteWithDetails(id: athleteId);
        athlete.Ranking = null;

        _athleteRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(athleteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(athlete);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(new UpdateRankingCommand
        {
            AthleteId = athleteId,
            CurrentRank = "5",
            StateRank = "3",
            NationalRank = "25",
            InternationalRank = "250",
            RankingAuthority = "BCCI"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.CurrentRank.Should().Be("5");
        result.Value.StateRank.Should().Be("3");
        result.Value.NationalRank.Should().Be("25");
        result.Value.InternationalRank.Should().Be("250");
        result.Value.RankingAuthority.Should().Be("BCCI");
        _rankingRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Ranking>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ExistingRanking_UpdatesRanking()
    {
        var athleteId = Guid.NewGuid();
        var athlete = TestDataBuilder.CreateAthleteWithDetails(id: athleteId);
        var existingRanking = athlete.Ranking!;

        _athleteRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(athleteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(athlete);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(new UpdateRankingCommand
        {
            AthleteId = athleteId,
            CurrentRank = "1",
            StateRank = "1"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        existingRanking.CurrentRank.Should().Be("1");
        existingRanking.StateRank.Should().Be("1");
        _rankingRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Ranking>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
