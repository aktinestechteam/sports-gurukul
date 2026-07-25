using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.CoachManagement.Queries.GetPagedCoaches;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Tests.Queries;

public class GetPagedCoachesQueryHandlerTests
{
    private readonly Mock<ICoachSearchRepository> _searchRepositoryMock = TestMocks.CreateCoachSearchRepository();
    private readonly Mock<ILogger<GetPagedCoachesQueryHandler>> _loggerMock = TestMocks.CreateLogger<GetPagedCoachesQueryHandler>();
    private readonly GetPagedCoachesQueryHandler _handler;

    public GetPagedCoachesQueryHandlerTests()
    {
        _handler = new GetPagedCoachesQueryHandler(
            _searchRepositoryMock.Object,
            _loggerMock.Object);
    }

    private static Coach CreateCoachWithUser(string fullName, int experience, DateTime createdAt)
    {
        var coach = TestDataBuilder.CreateCoach();
        coach.User.FullName = fullName;
        coach.YearsOfExperience = experience;
        coach.CreatedAt = createdAt;
        return coach;
    }

    [Fact]
    public async Task Handle_ReturnsPagedResults()
    {
        var pagedCoaches = new List<Coach>
        {
            CreateCoachWithUser("Alice", 5, DateTime.UtcNow.AddDays(-10)),
            CreateCoachWithUser("Bob", 10, DateTime.UtcNow.AddDays(-5))
        };

        _searchRepositoryMock.Setup(r => r.SearchCoachesAsync(
                It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(),
                It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<Guid[]>(), It.IsAny<string?>(),
                It.IsAny<Domain.Enums.CoachingLevel?>(), It.IsAny<int?>(), It.IsAny<int?>(),
                It.IsAny<string?>(), It.IsAny<Domain.Enums.VerificationStatus?>(), It.IsAny<string?>(),
                It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(),
                It.IsAny<string?>(), It.IsAny<decimal?>(), It.IsAny<decimal?>(), It.IsAny<double?>(),
                It.IsAny<bool?>(), It.IsAny<bool?>(), It.IsAny<bool?>(), It.IsAny<bool?>(),
                It.IsAny<bool?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<bool>(),
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string?>(), It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((pagedCoaches, 3));

        var result = await _handler.Handle(new GetPagedCoachesQuery
        {
            Page = 1,
            PageSize = 2
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().HaveCount(2);
        result.Value.TotalRecords.Should().Be(3);
        result.Value.TotalPages.Should().Be(2);
        result.Value.CurrentPage.Should().Be(1);
        result.Value.PageSize.Should().Be(2);
    }

    [Fact]
    public async Task Handle_SortByName_ReturnsSortedResults()
    {
        var sortedCoaches = new List<Coach>
        {
            CreateCoachWithUser("Alice", 10, DateTime.UtcNow),
            CreateCoachWithUser("Bob", 15, DateTime.UtcNow),
            CreateCoachWithUser("Charlie", 5, DateTime.UtcNow)
        };

        _searchRepositoryMock.Setup(r => r.SearchCoachesAsync(
                It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(),
                It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<Guid[]>(), It.IsAny<string?>(),
                It.IsAny<Domain.Enums.CoachingLevel?>(), It.IsAny<int?>(), It.IsAny<int?>(),
                It.IsAny<string?>(), It.IsAny<Domain.Enums.VerificationStatus?>(), It.IsAny<string?>(),
                It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(),
                It.IsAny<string?>(), It.IsAny<decimal?>(), It.IsAny<decimal?>(), It.IsAny<double?>(),
                It.IsAny<bool?>(), It.IsAny<bool?>(), It.IsAny<bool?>(), It.IsAny<bool?>(),
                It.IsAny<bool?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<bool>(),
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string?>(), It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((sortedCoaches, sortedCoaches.Count));

        var result = await _handler.Handle(new GetPagedCoachesQuery
        {
            Page = 1,
            PageSize = 10,
            SortBy = "name",
            SortDescending = false
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Items[0].FullName.Should().Be("Alice");
        result.Value.Items[1].FullName.Should().Be("Bob");
        result.Value.Items[2].FullName.Should().Be("Charlie");
    }

    [Fact]
    public async Task Handle_SortByNameDescending_ReturnsSortedResults()
    {
        var sortedCoaches = new List<Coach>
        {
            CreateCoachWithUser("Charlie", 10, DateTime.UtcNow),
            CreateCoachWithUser("Bob", 15, DateTime.UtcNow),
            CreateCoachWithUser("Alice", 5, DateTime.UtcNow)
        };

        _searchRepositoryMock.Setup(r => r.SearchCoachesAsync(
                It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(),
                It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<Guid[]>(), It.IsAny<string?>(),
                It.IsAny<Domain.Enums.CoachingLevel?>(), It.IsAny<int?>(), It.IsAny<int?>(),
                It.IsAny<string?>(), It.IsAny<Domain.Enums.VerificationStatus?>(), It.IsAny<string?>(),
                It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(),
                It.IsAny<string?>(), It.IsAny<decimal?>(), It.IsAny<decimal?>(), It.IsAny<double?>(),
                It.IsAny<bool?>(), It.IsAny<bool?>(), It.IsAny<bool?>(), It.IsAny<bool?>(),
                It.IsAny<bool?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<bool>(),
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string?>(), It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((sortedCoaches, sortedCoaches.Count));

        var result = await _handler.Handle(new GetPagedCoachesQuery
        {
            Page = 1,
            PageSize = 10,
            SortBy = "name",
            SortDescending = true
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Items[0].FullName.Should().Be("Charlie");
        result.Value.Items[1].FullName.Should().Be("Bob");
        result.Value.Items[2].FullName.Should().Be("Alice");
    }

    [Fact]
    public async Task Handle_SortByExperience_ReturnsSortedResults()
    {
        var sortedCoaches = new List<Coach>
        {
            CreateCoachWithUser("Bob", 5, DateTime.UtcNow),
            CreateCoachWithUser("Charlie", 10, DateTime.UtcNow),
            CreateCoachWithUser("Alice", 15, DateTime.UtcNow)
        };

        _searchRepositoryMock.Setup(r => r.SearchCoachesAsync(
                It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(),
                It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<Guid[]>(), It.IsAny<string?>(),
                It.IsAny<Domain.Enums.CoachingLevel?>(), It.IsAny<int?>(), It.IsAny<int?>(),
                It.IsAny<string?>(), It.IsAny<Domain.Enums.VerificationStatus?>(), It.IsAny<string?>(),
                It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(),
                It.IsAny<string?>(), It.IsAny<decimal?>(), It.IsAny<decimal?>(), It.IsAny<double?>(),
                It.IsAny<bool?>(), It.IsAny<bool?>(), It.IsAny<bool?>(), It.IsAny<bool?>(),
                It.IsAny<bool?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<bool>(),
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string?>(), It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((sortedCoaches, sortedCoaches.Count));

        var result = await _handler.Handle(new GetPagedCoachesQuery
        {
            Page = 1,
            PageSize = 10,
            SortBy = "experience"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Items[0].YearsOfExperience.Should().Be(5);
        result.Value.Items[1].YearsOfExperience.Should().Be(10);
        result.Value.Items[2].YearsOfExperience.Should().Be(15);
    }

    [Fact]
    public async Task Handle_EmptyCoaches_ReturnsEmptyResults()
    {
        _searchRepositoryMock.Setup(r => r.SearchCoachesAsync(
                It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(),
                It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<Guid[]>(), It.IsAny<string?>(),
                It.IsAny<Domain.Enums.CoachingLevel?>(), It.IsAny<int?>(), It.IsAny<int?>(),
                It.IsAny<string?>(), It.IsAny<Domain.Enums.VerificationStatus?>(), It.IsAny<string?>(),
                It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(),
                It.IsAny<string?>(), It.IsAny<decimal?>(), It.IsAny<decimal?>(), It.IsAny<double?>(),
                It.IsAny<bool?>(), It.IsAny<bool?>(), It.IsAny<bool?>(), It.IsAny<bool?>(),
                It.IsAny<bool?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<bool>(),
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string?>(), It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<Coach>(), 0));

        var result = await _handler.Handle(new GetPagedCoachesQuery
        {
            Page = 1,
            PageSize = 20
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().BeEmpty();
        result.Value.TotalRecords.Should().Be(0);
        result.Value.TotalPages.Should().Be(0);
    }
}
