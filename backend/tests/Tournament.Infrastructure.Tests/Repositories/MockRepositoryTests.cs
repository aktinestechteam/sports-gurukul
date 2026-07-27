using System.Linq.Expressions;
using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Entities;

namespace Tournament.Infrastructure.Tests.Repositories;

public class MockRepositoryTests
{
    private readonly Mock<IRepository<TournamentEntity>> _repositoryMock;

    public MockRepositoryTests()
    {
        _repositoryMock = new Mock<IRepository<TournamentEntity>>();
    }

    [Fact]
    public async Task MockRepository_GetByIdAsync_ReturnsEntity()
    {
        var id = Guid.NewGuid();
        var tournament = new TournamentEntity
        {
            Id = id,
            TournamentName = "Test Tournament",
            TournamentCode = "TRN-001",
            CreatedAt = DateTime.UtcNow
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tournament);

        var result = await _repositoryMock.Object.GetByIdAsync(id);

        result.Should().NotBeNull();
        result!.Id.Should().Be(id);
        result.TournamentName.Should().Be("Test Tournament");
    }

    [Fact]
    public async Task MockRepository_GetByIdAsync_ReturnsNull_WhenNotFound()
    {
        var id = Guid.NewGuid();

        _repositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TournamentEntity?)null);

        var result = await _repositoryMock.Object.GetByIdAsync(id);

        result.Should().BeNull();
    }

    [Fact]
    public async Task MockRepository_GetAllAsync_ReturnsListOfEntities()
    {
        var tournaments = new List<TournamentEntity>
        {
            new() { Id = Guid.NewGuid(), TournamentName = "Tournament 1", CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), TournamentName = "Tournament 2", CreatedAt = DateTime.UtcNow }
        };

        _repositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(tournaments);

        var result = await _repositoryMock.Object.GetAllAsync();

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task MockRepository_AddAsync_ReturnsAddedEntity()
    {
        var tournament = new TournamentEntity
        {
            Id = Guid.NewGuid(),
            TournamentName = "New Tournament",
            TournamentCode = "TRN-002",
            CreatedAt = DateTime.UtcNow
        };

        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<TournamentEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TournamentEntity t, CancellationToken _) =>
            {
                t.CreatedAt = DateTime.UtcNow;
                return t;
            });

        var result = await _repositoryMock.Object.AddAsync(tournament);

        result.Should().NotBeNull();
        result.TournamentName.Should().Be("New Tournament");
        _repositoryMock.Verify(r => r.AddAsync(tournament, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public void MockRepository_Update_CallsCorrectly()
    {
        var tournament = new TournamentEntity
        {
            Id = Guid.NewGuid(),
            TournamentName = "Old Name",
            TournamentCode = "TRN-003",
            CreatedAt = DateTime.UtcNow
        };

        _repositoryMock
            .Setup(r => r.Update(It.IsAny<TournamentEntity>()));

        tournament.TournamentName = "Updated Name";
        _repositoryMock.Object.Update(tournament);

        _repositoryMock.Verify(r => r.Update(tournament), Times.Once);
    }

    [Fact]
    public void MockRepository_Remove_CallsCorrectly()
    {
        var tournament = new TournamentEntity
        {
            Id = Guid.NewGuid(),
            TournamentName = "Tournament to Delete",
            TournamentCode = "TRN-004",
            CreatedAt = DateTime.UtcNow
        };

        _repositoryMock
            .Setup(r => r.Remove(It.IsAny<TournamentEntity>()));

        _repositoryMock.Object.Remove(tournament);

        _repositoryMock.Verify(r => r.Remove(tournament), Times.Once);
    }

    [Fact]
    public async Task MockRepository_FindAsync_ReturnsMatchingEntities()
    {
        var tournaments = new List<TournamentEntity>
        {
            new() { Id = Guid.NewGuid(), TournamentName = "Match 1", CreatedAt = DateTime.UtcNow }
        };

        _repositoryMock
            .Setup(r => r.FindAsync(It.IsAny<Expression<Func<TournamentEntity, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tournaments);

        var result = await _repositoryMock.Object.FindAsync(t => t.TournamentName == "Match 1");

        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task MockRepository_CountAsync_ReturnsCount()
    {
        _repositoryMock
            .Setup(r => r.CountAsync(It.IsAny<Expression<Func<TournamentEntity, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(5);

        var result = await _repositoryMock.Object.CountAsync();

        result.Should().Be(5);
    }

    [Fact]
    public async Task MockRepository_AnyAsync_ReturnsTrue_WhenExists()
    {
        _repositoryMock
            .Setup(r => r.AnyAsync(It.IsAny<Expression<Func<TournamentEntity, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _repositoryMock.Object.AnyAsync(t => t.TournamentName == "Test");

        result.Should().BeTrue();
    }

    [Fact]
    public async Task MockRepository_AnyAsync_ReturnsFalse_WhenNotExists()
    {
        _repositoryMock
            .Setup(r => r.AnyAsync(It.IsAny<Expression<Func<TournamentEntity, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _repositoryMock.Object.AnyAsync(t => t.TournamentName == "NonExistent");

        result.Should().BeFalse();
    }

    [Fact]
    public async Task MockRepository_Verify_AddCalledExactlyOnce()
    {
        var tournament = new TournamentEntity
        {
            Id = Guid.NewGuid(),
            TournamentName = "Verify Test",
            CreatedAt = DateTime.UtcNow
        };

        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<TournamentEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tournament);

        _ = await _repositoryMock.Object.AddAsync(tournament);

        _repositoryMock.Verify(
            r => r.AddAsync(It.IsAny<TournamentEntity>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }


    [Fact]
    public void MockRepository_Verify_UpdateCalledExactlyOnce()
    {
        var tournament = new TournamentEntity
        {
            Id = Guid.NewGuid(),
            TournamentName = "Update Verify",
            CreatedAt = DateTime.UtcNow
        };

        _repositoryMock.Object.Update(tournament);

        _repositoryMock.Verify(r => r.Update(It.IsAny<TournamentEntity>()), Times.Once);
    }

    [Fact]
    public void MockRepository_Verify_RemoveCalledExactlyOnce()
    {
        var tournament = new TournamentEntity
        {
            Id = Guid.NewGuid(),
            TournamentName = "Remove Verify",
            CreatedAt = DateTime.UtcNow
        };

        _repositoryMock.Object.Remove(tournament);

        _repositoryMock.Verify(r => r.Remove(It.IsAny<TournamentEntity>()), Times.Once);
    }

    [Fact]
    public void MockRepository_RepositoryType_ShouldBeIRepositoryOfTournamentEntity()
    {
        _repositoryMock.Object.Should().BeAssignableTo<IRepository<TournamentEntity>>();
    }

}
