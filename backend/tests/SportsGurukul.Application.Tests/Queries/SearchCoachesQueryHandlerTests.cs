using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.CoachManagement.Queries.SearchCoaches;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Tests.Queries;

public class SearchCoachesQueryHandlerTests
{
    private readonly Mock<ICoachSearchRepository> _searchRepositoryMock = TestMocks.CreateCoachSearchRepository();
    private readonly Mock<ILogger<SearchCoachesQueryHandler>> _loggerMock = TestMocks.CreateLogger<SearchCoachesQueryHandler>();
    private readonly SearchCoachesQueryHandler _handler;

    public SearchCoachesQueryHandlerTests()
    {
        _handler = new SearchCoachesQueryHandler(
            _searchRepositoryMock.Object,
            _loggerMock.Object);
    }

    private static Coach CreateSearchableCoach(
        string name = "Test Coach",
        string email = "test@example.com",
        string coachCode = "COACH-001",
        int experience = 5,
        string? city = "Mumbai",
        string? state = "Maharashtra",
        string? country = "India",
        string? language = "English",
        CoachingLevel level = CoachingLevel.Senior,
        CoachStatus status = CoachStatus.Active,
        VerificationStatus verification = VerificationStatus.Verified,
        bool onlineAvailable = true,
        bool offlineAvailable = true,
        DateTime? createdAt = null)
    {
        var coach = TestDataBuilder.CreateCoach();
        coach.User.FullName = name;
        coach.User.Email = email;
        coach.CoachCode = coachCode;
        coach.YearsOfExperience = experience;
        coach.PreferredLanguage = language;
        coach.CoachingLevel = level;
        coach.Status = status;
        coach.VerificationStatus = verification;
        coach.CreatedAt = createdAt ?? DateTime.UtcNow;
        coach.Availability = new CoachAvailability
        {
            Id = Guid.NewGuid(),
            OnlineAvailable = onlineAvailable,
            OfflineAvailable = offlineAvailable
        };
        coach.Location = new CoachLocation
        {
            Id = Guid.NewGuid(),
            City = city,
            State = state,
            Country = country
        };
        return coach;
    }

    [Fact]
    public async Task Handle_NoFilters_ReturnsAllCoaches()
    {
        var coaches = new List<Coach>
        {
            CreateSearchableCoach(name: "Coach A"),
            CreateSearchableCoach(name: "Coach B")
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
            .ReturnsAsync((coaches, coaches.Count));

        var result = await _handler.Handle(new SearchCoachesQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().HaveCount(2);
        result.Value.TotalRecords.Should().Be(2);
    }

    [Fact]
    public async Task Handle_SearchTerm_FiltersByName()
    {
        var allCoaches = new List<Coach>
        {
            CreateSearchableCoach(name: "John Smith"),
            CreateSearchableCoach(name: "Jane Doe"),
            CreateSearchableCoach(name: "John Adams")
        };
        var filteredCoaches = allCoaches.Where(c => c.User.FullName.Contains("John")).ToList();

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
            .ReturnsAsync((filteredCoaches, filteredCoaches.Count));

        var result = await _handler.Handle(new SearchCoachesQuery
        {
            SearchTerm = "John"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_SearchTerm_FiltersByEmail()
    {
        var allCoaches = new List<Coach>
        {
            CreateSearchableCoach(email: "alice@sports.com"),
            CreateSearchableCoach(email: "bob@other.com")
        };
        var filteredCoaches = allCoaches.Where(c => c.User.Email!.Contains("sports")).ToList();

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
            .ReturnsAsync((filteredCoaches, filteredCoaches.Count));

        var result = await _handler.Handle(new SearchCoachesQuery
        {
            SearchTerm = "sports"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_MinExperience_FiltersCorrectly()
    {
        var filteredCoaches = new List<Coach>
        {
            CreateSearchableCoach(experience: 8),
            CreateSearchableCoach(experience: 15)
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
            .ReturnsAsync((filteredCoaches, filteredCoaches.Count));

        var result = await _handler.Handle(new SearchCoachesQuery
        {
            MinExperience = 5
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_MaxExperience_FiltersCorrectly()
    {
        var filteredCoaches = new List<Coach>
        {
            CreateSearchableCoach(experience: 2),
            CreateSearchableCoach(experience: 8)
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
            .ReturnsAsync((filteredCoaches, filteredCoaches.Count));

        var result = await _handler.Handle(new SearchCoachesQuery
        {
            MaxExperience = 10
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_City_FiltersCorrectly()
    {
        var filteredCoaches = new List<Coach>
        {
            CreateSearchableCoach(city: "Mumbai"),
            CreateSearchableCoach(city: "Mumbai")
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
            .ReturnsAsync((filteredCoaches, filteredCoaches.Count));

        var result = await _handler.Handle(new SearchCoachesQuery
        {
            City = "Mumbai"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_Status_FiltersCorrectly()
    {
        var filteredCoaches = new List<Coach>
        {
            CreateSearchableCoach(status: CoachStatus.Active),
            CreateSearchableCoach(status: CoachStatus.Active)
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
            .ReturnsAsync((filteredCoaches, filteredCoaches.Count));

        var result = await _handler.Handle(new SearchCoachesQuery
        {
            Status = CoachStatus.Active
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_OnlineAvailable_FiltersCorrectly()
    {
        var filteredCoaches = new List<Coach>
        {
            CreateSearchableCoach(onlineAvailable: true)
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
            .ReturnsAsync((filteredCoaches, filteredCoaches.Count));

        var result = await _handler.Handle(new SearchCoachesQuery
        {
            OnlineAvailable = true
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_Pagination_ReturnsCorrectPage()
    {
        var pageCoaches = Enumerable.Range(1, 10)
            .Select(i => CreateSearchableCoach(name: $"Coach {i + 10:D2}"))
            .ToList();

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
            .ReturnsAsync((pageCoaches, 25));

        var result = await _handler.Handle(new SearchCoachesQuery
        {
            Page = 2,
            PageSize = 10
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().HaveCount(10);
        result.Value.TotalRecords.Should().Be(25);
        result.Value.TotalPages.Should().Be(3);
        result.Value.CurrentPage.Should().Be(2);
    }

    [Fact]
    public async Task Handle_SortByNameAscending_ReturnsSorted()
    {
        var sortedCoaches = new List<Coach>
        {
            CreateSearchableCoach(name: "Alice"),
            CreateSearchableCoach(name: "Bob"),
            CreateSearchableCoach(name: "Charlie")
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

        var result = await _handler.Handle(new SearchCoachesQuery
        {
            SortBy = "name",
            SortDescending = false
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Items[0].FullName.Should().Be("Alice");
        result.Value.Items[1].FullName.Should().Be("Bob");
        result.Value.Items[2].FullName.Should().Be("Charlie");
    }
}
