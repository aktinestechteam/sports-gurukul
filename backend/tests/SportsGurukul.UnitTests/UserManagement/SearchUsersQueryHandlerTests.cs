using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.UserManagement.DTOs;
using SportsGurukul.Application.Features.UserManagement.Queries.SearchUsers;

namespace SportsGurukul.UnitTests.UserManagement;

public class SearchUsersQueryHandlerTests
{
    private readonly Mock<IUserProfileRepository> _userProfileRepositoryMock;
    private readonly Mock<ILogger<SearchUsersQueryHandler>> _loggerMock;
    private readonly SearchUsersQueryHandler _handler;

    public SearchUsersQueryHandlerTests()
    {
        _userProfileRepositoryMock = new Mock<IUserProfileRepository>();
        _loggerMock = new Mock<ILogger<SearchUsersQueryHandler>>();
        _handler = new SearchUsersQueryHandler(
            _userProfileRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnEmptyResults_When_NoUsersMatch()
    {
        _userProfileRepositoryMock
            .Setup(r => r.SearchProfilesAsync(It.IsAny<UserSearchRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<UserListDto>(), 0));

        var result = await _handler.Handle(
            new SearchUsersQuery { SearchTerm = "nonexistent" },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().BeEmpty();
        result.Value.TotalRecords.Should().Be(0);
        result.Value.TotalPages.Should().Be(0);
    }

    [Fact]
    public async Task Handle_Should_ReturnUsers_When_MatchFound()
    {
        var users = new List<UserListDto>
        {
            new() { UserId = Guid.NewGuid(), FullName = "John Doe", Email = "john@example.com" }
        };
        _userProfileRepositoryMock
            .Setup(r => r.SearchProfilesAsync(It.IsAny<UserSearchRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((users, 1));

        var result = await _handler.Handle(
            new SearchUsersQuery { SearchTerm = "John" },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().HaveCount(1);
        result.Value.Items.First().FullName.Should().Be("John Doe");
    }

    [Fact]
    public async Task Handle_Should_CalculateTotalPages_When_ResultsExist()
    {
        var users = new List<UserListDto>
        {
            new() { UserId = Guid.NewGuid() }
        };
        _userProfileRepositoryMock
            .Setup(r => r.SearchProfilesAsync(It.IsAny<UserSearchRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((users, 50));

        var result = await _handler.Handle(
            new SearchUsersQuery { Page = 1, PageSize = 20 },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.TotalPages.Should().Be(3);
        result.Value.TotalRecords.Should().Be(50);
    }

    [Fact]
    public async Task Handle_Should_PassFiltersToRepository()
    {
        _userProfileRepositoryMock
            .Setup(r => r.SearchProfilesAsync(It.IsAny<UserSearchRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<UserListDto>(), 0));

        await _handler.Handle(
            new SearchUsersQuery
            {
                SearchTerm = "test",
                Name = "John",
                Email = "john@example.com",
                Mobile = "123456",
                City = "Mumbai",
                State = "Maharashtra",
                Country = "India",
                SortBy = "name",
                SortDescending = true,
                Page = 2,
                PageSize = 10
            },
            CancellationToken.None);

        _userProfileRepositoryMock.Verify(r => r.SearchProfilesAsync(
            It.Is<UserSearchRequest>(req =>
                req.SearchTerm == "test" &&
                req.Name == "John" &&
                req.Email == "john@example.com" &&
                req.Mobile == "123456" &&
                req.City == "Mumbai" &&
                req.State == "Maharashtra" &&
                req.Country == "India" &&
                req.SortBy == "name" &&
                req.SortDescending == true &&
                req.Page == 2 &&
                req.PageSize == 10),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_MapPaginationFields_When_ReturningResults()
    {
        var users = new List<UserListDto>
        {
            new() { UserId = Guid.NewGuid() }
        };
        _userProfileRepositoryMock
            .Setup(r => r.SearchProfilesAsync(It.IsAny<UserSearchRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((users, 25));

        var result = await _handler.Handle(
            new SearchUsersQuery { Page = 2, PageSize = 10 },
            CancellationToken.None);

        result.Value!.CurrentPage.Should().Be(2);
        result.Value.PageSize.Should().Be(10);
        result.Value.TotalRecords.Should().Be(25);
        result.Value.TotalPages.Should().Be(3);
    }

    [Fact]
    public async Task Handle_Should_PassRoleFilter_When_Provided()
    {
        _userProfileRepositoryMock
            .Setup(r => r.SearchProfilesAsync(It.IsAny<UserSearchRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<UserListDto>(), 0));

        await _handler.Handle(
            new SearchUsersQuery
            {
                Role = Domain.Enums.RoleType.Coach
            },
            CancellationToken.None);

        _userProfileRepositoryMock.Verify(r => r.SearchProfilesAsync(
            It.Is<UserSearchRequest>(req => req.Role == Domain.Enums.RoleType.Coach),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_PassDateRangeFilters_When_Provided()
    {
        var from = DateTime.UtcNow.AddDays(-30);
        var to = DateTime.UtcNow;

        _userProfileRepositoryMock
            .Setup(r => r.SearchProfilesAsync(It.IsAny<UserSearchRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<UserListDto>(), 0));

        await _handler.Handle(
            new SearchUsersQuery
            {
                CreatedFrom = from,
                CreatedTo = to,
                UpdatedFrom = from,
                UpdatedTo = to
            },
            CancellationToken.None);

        _userProfileRepositoryMock.Verify(r => r.SearchProfilesAsync(
            It.Is<UserSearchRequest>(req =>
                req.CreatedFrom == from &&
                req.CreatedTo == to &&
                req.UpdatedFrom == from &&
                req.UpdatedTo == to),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
