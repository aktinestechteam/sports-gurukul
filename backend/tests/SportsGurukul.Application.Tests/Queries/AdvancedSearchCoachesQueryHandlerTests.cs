using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.CoachManagement.Queries.AdvancedSearchCoaches;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Tests.Queries;

public class AdvancedSearchCoachesQueryHandlerTests
{
    private readonly Mock<ICoachSearchRepository> _searchRepoMock = TestMocks.CreateCoachSearchRepository();
    private readonly Mock<ILogger<AdvancedSearchCoachesQueryHandler>> _loggerMock = TestMocks.CreateLogger<AdvancedSearchCoachesQueryHandler>();
    private readonly AdvancedSearchCoachesQueryHandler _handler;

    public AdvancedSearchCoachesQueryHandlerTests()
    {
        _handler = new AdvancedSearchCoachesQueryHandler(
            _searchRepoMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsCoachesWithPagination()
    {
        var coaches = new List<Coach>
        {
            CreateTestCoach("Coach 1", "COACH-001"),
            CreateTestCoach("Coach 2", "COACH-002")
        };
        _searchRepoMock.Setup(r => r.SearchCoachesAsync(
            It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(),
            It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<Guid[]>(), It.IsAny<string?>(),
            It.IsAny<CoachingLevel?>(), It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<string?>(),
            It.IsAny<VerificationStatus?>(), It.IsAny<string?>(), It.IsAny<string?>(),
            It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(),
            It.IsAny<decimal?>(), It.IsAny<decimal?>(), It.IsAny<double?>(),
            It.IsAny<bool?>(), It.IsAny<bool?>(), It.IsAny<bool?>(),
            It.IsAny<bool?>(), It.IsAny<bool?>(), It.IsAny<string?>(),
            It.IsAny<string?>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<int>(),
            It.IsAny<string?>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((coaches, 20));

        var result = await _handler.Handle(new AdvancedSearchCoachesQuery
        {
            Page = 1,
            PageSize = 10
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().HaveCount(2);
        result.Value.TotalRecords.Should().Be(20);
        result.Value.TotalPages.Should().Be(2);
        result.Value.CurrentPage.Should().Be(1);
        result.Value.PageSize.Should().Be(10);
    }

    [Fact]
    public async Task Handle_NoResults_ReturnsEmptyResponse()
    {
        _searchRepoMock.Setup(r => r.SearchCoachesAsync(
            It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(),
            It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<Guid[]>(), It.IsAny<string?>(),
            It.IsAny<CoachingLevel?>(), It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<string?>(),
            It.IsAny<VerificationStatus?>(), It.IsAny<string?>(), It.IsAny<string?>(),
            It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(),
            It.IsAny<decimal?>(), It.IsAny<decimal?>(), It.IsAny<double?>(),
            It.IsAny<bool?>(), It.IsAny<bool?>(), It.IsAny<bool?>(),
            It.IsAny<bool?>(), It.IsAny<bool?>(), It.IsAny<string?>(),
            It.IsAny<string?>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<int>(),
            It.IsAny<string?>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<Coach>(), 0));

        var result = await _handler.Handle(new AdvancedSearchCoachesQuery
        {
            SearchTerm = "nonexistent",
            Page = 1,
            PageSize = 20
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().BeEmpty();
        result.Value.TotalRecords.Should().Be(0);
        result.Value.TotalPages.Should().Be(0);
    }

    [Fact]
    public async Task Handle_SetsNextCursorWhenMoreResultsExist()
    {
        var coaches = Enumerable.Range(0, 20)
            .Select(i => CreateTestCoach($"Coach {i}", $"COACH-{i:D3}"))
            .ToList();
        _searchRepoMock.Setup(r => r.SearchCoachesAsync(
            It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(),
            It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<Guid[]>(), It.IsAny<string?>(),
            It.IsAny<CoachingLevel?>(), It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<string?>(),
            It.IsAny<VerificationStatus?>(), It.IsAny<string?>(), It.IsAny<string?>(),
            It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(),
            It.IsAny<decimal?>(), It.IsAny<decimal?>(), It.IsAny<double?>(),
            It.IsAny<bool?>(), It.IsAny<bool?>(), It.IsAny<bool?>(),
            It.IsAny<bool?>(), It.IsAny<bool?>(), It.IsAny<string?>(),
            It.IsAny<string?>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<int>(),
            It.IsAny<string?>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((coaches, 50));

        var result = await _handler.Handle(new AdvancedSearchCoachesQuery
        {
            UseCursorPagination = true,
            PageSize = 20
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.NextCursor.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_MapsCoachFieldsCorrectly()
    {
        var coaches = new List<Coach> { CreateTestCoach("Rahul Sharma", "COACH-001") };
        _searchRepoMock.Setup(r => r.SearchCoachesAsync(
            It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(),
            It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<Guid[]>(), It.IsAny<string?>(),
            It.IsAny<CoachingLevel?>(), It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<string?>(),
            It.IsAny<VerificationStatus?>(), It.IsAny<string?>(), It.IsAny<string?>(),
            It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(),
            It.IsAny<decimal?>(), It.IsAny<decimal?>(), It.IsAny<double?>(),
            It.IsAny<bool?>(), It.IsAny<bool?>(), It.IsAny<bool?>(),
            It.IsAny<bool?>(), It.IsAny<bool?>(), It.IsAny<string?>(),
            It.IsAny<string?>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<int>(),
            It.IsAny<string?>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((coaches, 1));

        var result = await _handler.Handle(new AdvancedSearchCoachesQuery { Page = 1, PageSize = 20 }, CancellationToken.None);

        var item = result.Value!.Items[0];
        item.FullName.Should().Be("Rahul Sharma");
        item.CoachCode.Should().Be("COACH-001");
        item.CoachingLevel.Should().Be("Senior");
        item.Status.Should().Be("Active");
        item.VerificationStatus.Should().Be("Verified");
        item.IsVerified.Should().BeTrue();
        item.PrimarySport.Should().Be("Cricket");
        item.SportCategory.Should().Be("Team Sports");
        item.City.Should().Be("Mumbai");
        item.State.Should().Be("Maharashtra");
        item.Country.Should().Be("India");
    }

    private static Coach CreateTestCoach(string name, string code)
    {
        return new Coach
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            CoachCode = code,
            CoachingLevel = CoachingLevel.Senior,
            Status = CoachStatus.Active,
            VerificationStatus = VerificationStatus.Verified,
            YearsOfExperience = 8,
            CreatedAt = DateTime.UtcNow.AddDays(-90),
            User = new User { FullName = name, Email = $"{name.ToLower().Replace(" ", "")}@example.com" },
            CoachSports = new List<CoachSport>
            {
                new()
                {
                    SportId = Guid.NewGuid(),
                    IsPrimarySport = true,
                    Sport = new Sport
                    {
                        Name = "Cricket",
                        SportCategory = new SportCategory { Name = "Team Sports" }
                    }
                }
            },
            Certifications = new List<CoachCertification>
            {
                new() { Id = Guid.NewGuid(), CertificationName = "BCCI Level A", VerificationStatus = VerificationStatus.Verified }
            },
            Availability = new CoachAvailability
            {
                OnlineAvailable = true,
                OfflineAvailable = true
            },
            Location = new CoachLocation
            {
                City = "Mumbai",
                State = "Maharashtra",
                Country = "India"
            }
        };
    }
}
