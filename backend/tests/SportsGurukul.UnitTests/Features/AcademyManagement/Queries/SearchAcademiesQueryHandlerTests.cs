using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.AcademyManagement.Queries.SearchAcademies;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.UnitTests.Features.AcademyManagement.Queries;

public class SearchAcademiesQueryHandlerTests
{
    private readonly Mock<IAcademyRepository> _academyRepositoryMock;
    private readonly Mock<ILogger<SearchAcademiesQueryHandler>> _loggerMock;
    private readonly SearchAcademiesQueryHandler _handler;

    public SearchAcademiesQueryHandlerTests()
    {
        _academyRepositoryMock = new Mock<IAcademyRepository>();
        _loggerMock = new Mock<ILogger<SearchAcademiesQueryHandler>>();
        _handler = new SearchAcademiesQueryHandler(
            _academyRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_NoFilters_ReturnsAllAcademies()
    {
        var academies = new List<Academy>
        {
            CreateAcademy("Academy 1", "academy1@test.com"),
            CreateAcademy("Academy 2", "academy2@test.com")
        };

        _academyRepositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(academies);

        var query = new SearchAcademiesQuery { PageSize = 20 };

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().HaveCount(2);
        result.Value.TotalRecords.Should().Be(2);
    }

    [Fact]
    public async Task Handle_WithSearchTerm_FiltersByName()
    {
        var academies = new List<Academy>
        {
            CreateAcademy("Cricket Academy", "cricket@test.com"),
            CreateAcademy("Football Academy", "football@test.com"),
            CreateAcademy("Tennis Academy", "tennis@test.com")
        };

        _academyRepositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(academies);

        var query = new SearchAcademiesQuery { SearchTerm = "cricket", PageSize = 20 };

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().HaveCount(1);
        result.Value.Items[0].Name.Should().Be("Cricket Academy");
    }

    [Fact]
    public async Task Handle_WithCityFilter_FiltersByCity()
    {
        var academies = new List<Academy>
        {
            CreateAcademyWithContact("Academy 1", "a1@test.com", "Mumbai", "Maharashtra"),
            CreateAcademyWithContact("Academy 2", "a2@test.com", "Delhi", "Delhi"),
            CreateAcademyWithContact("Academy 3", "a3@test.com", "Mumbai", "Maharashtra")
        };

        _academyRepositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(academies);

        var query = new SearchAcademiesQuery { City = "Mumbai", PageSize = 20 };

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().HaveCount(2);
        result.Value.Items.Should().OnlyContain(a => a.City == "Mumbai");
    }

    [Fact]
    public async Task Handle_WithStateFilter_FiltersByState()
    {
        var academies = new List<Academy>
        {
            CreateAcademyWithContact("Academy 1", "a1@test.com", "Mumbai", "Maharashtra"),
            CreateAcademyWithContact("Academy 2", "a2@test.com", "Pune", "Maharashtra"),
            CreateAcademyWithContact("Academy 3", "a3@test.com", "Delhi", "Delhi")
        };

        _academyRepositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(academies);

        var query = new SearchAcademiesQuery { State = "Maharashtra", PageSize = 20 };

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().HaveCount(2);
        result.Value.Items.Should().OnlyContain(a => a.State == "Maharashtra");
    }

    [Fact]
    public async Task Handle_Pagination_ReturnsCorrectPage()
    {
        var academies = Enumerable.Range(1, 25)
            .Select(i => CreateAcademy($"Academy {i:D2}", $"academy{i:D2}@test.com"))
            .ToList();

        _academyRepositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(academies);

        var query = new SearchAcademiesQuery { Page = 2, PageSize = 10 };

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().HaveCount(10);
        result.Value.TotalRecords.Should().Be(25);
        result.Value.TotalPages.Should().Be(3);
        result.Value.CurrentPage.Should().Be(2);
    }

    [Fact]
    public async Task Handle_EmptyResult_ReturnsEmptyList()
    {
        _academyRepositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Academy>());

        var query = new SearchAcademiesQuery { SearchTerm = "nonexistent", PageSize = 20 };

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().BeEmpty();
        result.Value.TotalRecords.Should().Be(0);
    }

    private static Academy CreateAcademy(string name, string email) => new()
    {
        Id = Guid.NewGuid(),
        AcademyCode = "ACAD-TEST",
        Name = name,
        Email = email,
        Phone = "1234567890",
        Status = AcademyStatus.Active,
        VerificationStatus = VerificationStatus.Pending,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        Branches = new List<AcademyBranch>(),
        AcademySports = new List<AcademySport>(),
        Facilities = new List<AcademyFacility>(),
        Memberships = new List<AcademyMembership>()
    };

    private static Academy CreateAcademyWithContact(string name, string email, string city, string state) => new()
    {
        Id = Guid.NewGuid(),
        AcademyCode = "ACAD-TEST",
        Name = name,
        Email = email,
        Phone = "1234567890",
        Status = AcademyStatus.Active,
        VerificationStatus = VerificationStatus.Pending,
        Contact = new AcademyContact
        {
            Id = Guid.NewGuid(),
            City = city,
            State = state,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        },
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        Branches = new List<AcademyBranch>(),
        AcademySports = new List<AcademySport>(),
        Facilities = new List<AcademyFacility>(),
        Memberships = new List<AcademyMembership>()
    };
}
