using System.Linq.Expressions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace TournamentTestShared;

public static class MockRepositoryBuilder
{
    public static Mock<IRepository<T>> Create<T>() where T : BaseEntity
    {
        var mock = new Mock<IRepository<T>>();
        mock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<T>());
        mock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<T, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<T>());
        mock.Setup(r => r.CountAsync(It.IsAny<Expression<Func<T, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
        mock.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<T, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        return mock;
    }

    public static Mock<ITournamentRepository> CreateTournamentRepository()
    {
        var mock = new Mock<ITournamentRepository>();
        mock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid id, CancellationToken _) => (Tournament?)null);
        mock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Tournament>());
        mock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Tournament, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Tournament>());
        mock.Setup(r => r.CountAsync(It.IsAny<Expression<Func<Tournament, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
        mock.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<Tournament, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        mock.Setup(r => r.SearchAsync(
                It.IsAny<Guid?>(), It.IsAny<TournamentStatus?>(),
                It.IsAny<TournamentType?>(), It.IsAny<string?>(),
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Tournament>());
        mock.Setup(r => r.CountSearchAsync(
                It.IsAny<Guid?>(), It.IsAny<TournamentStatus?>(),
                It.IsAny<TournamentType?>(), It.IsAny<string?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
        return mock;
    }

    public static Mock<IMatchRepository> CreateMatchRepository()
    {
        var mock = new Mock<IMatchRepository>();
        mock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid id, CancellationToken _) => (TournamentMatch?)null);
        mock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TournamentMatch>());
        mock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<TournamentMatch, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TournamentMatch>());
        mock.Setup(r => r.CountAsync(It.IsAny<Expression<Func<TournamentMatch, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
        mock.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<TournamentMatch, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        mock.Setup(r => r.GetByTournamentIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TournamentMatch>());
        mock.Setup(r => r.GetByStatusAsync(It.IsAny<Guid>(), It.IsAny<MatchStatus>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TournamentMatch>());
        mock.Setup(r => r.SearchAsync(
                It.IsAny<Guid?>(), It.IsAny<MatchStatus?>(),
                It.IsAny<DateTime?>(), It.IsAny<DateTime?>(),
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TournamentMatch>());
        mock.Setup(r => r.CountSearchAsync(
                It.IsAny<Guid?>(), It.IsAny<MatchStatus?>(),
                It.IsAny<DateTime?>(), It.IsAny<DateTime?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
        return mock;
    }

    public static Mock<IRegistrationRepository> CreateRegistrationRepository()
    {
        var mock = new Mock<IRegistrationRepository>();
        mock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid id, CancellationToken _) => (TournamentRegistration?)null);
        mock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TournamentRegistration>());
        mock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<TournamentRegistration, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TournamentRegistration>());
        mock.Setup(r => r.CountAsync(It.IsAny<Expression<Func<TournamentRegistration, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
        mock.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<TournamentRegistration, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        mock.Setup(r => r.GetByTournamentIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TournamentRegistration>());
        mock.Setup(r => r.GetByStatusAsync(It.IsAny<Guid>(), It.IsAny<TournamentRegistrationStatus>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TournamentRegistration>());
        mock.Setup(r => r.GetRegistrationCountAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
        mock.Setup(r => r.IsAlreadyRegisteredAsync(
                It.IsAny<Guid>(), It.IsAny<Guid?>(), It.IsAny<Guid?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        mock.Setup(r => r.SearchAsync(
                It.IsAny<Guid?>(), It.IsAny<TournamentRegistrationStatus?>(),
                It.IsAny<string?>(), It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TournamentRegistration>());
        mock.Setup(r => r.CountSearchAsync(
                It.IsAny<Guid?>(), It.IsAny<TournamentRegistrationStatus?>(),
                It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
        return mock;
    }

    public static Mock<IRankingRepository> CreateRankingRepository()
    {
        var mock = new Mock<IRankingRepository>();
        mock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid id, CancellationToken _) => (TournamentRanking?)null);
        mock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TournamentRanking>());
        mock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<TournamentRanking, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TournamentRanking>());
        mock.Setup(r => r.CountAsync(It.IsAny<Expression<Func<TournamentRanking, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
        mock.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<TournamentRanking, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        mock.Setup(r => r.GetByTournamentIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TournamentRanking>());
        mock.Setup(r => r.GetByCategoryIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TournamentRanking>());
        mock.Setup(r => r.GetTopRankingsAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TournamentRanking>());
        mock.Setup(r => r.GetTopRankingsByCategoryAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TournamentRanking>());
        return mock;
    }
}
