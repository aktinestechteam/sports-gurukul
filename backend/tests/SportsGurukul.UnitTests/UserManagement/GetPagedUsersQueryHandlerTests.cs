using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.UserManagement.DTOs;
using SportsGurukul.Application.Features.UserManagement.Queries.GetPagedUsers;

namespace SportsGurukul.UnitTests.UserManagement;

public class GetPagedUsersQueryHandlerTests
{
    private readonly Mock<IUserProfileRepository> _userProfileRepositoryMock;
    private readonly Mock<ILogger<GetPagedUsersQueryHandler>> _loggerMock;
    private readonly GetPagedUsersQueryHandler _handler;

    public GetPagedUsersQueryHandlerTests()
    {
        _userProfileRepositoryMock = new Mock<IUserProfileRepository>();
        _loggerMock = new Mock<ILogger<GetPagedUsersQueryHandler>>();
        _handler = new GetPagedUsersQueryHandler(
            _userProfileRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnEmptyResults_When_NoUsers()
    {
        _userProfileRepositoryMock
            .Setup(r => r.SearchProfilesAsync(It.IsAny<UserSearchRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<UserListDto>(), 0));

        var result = await _handler.Handle(
            new GetPagedUsersQuery(),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().BeEmpty();
        result.Value.TotalRecords.Should().Be(0);
    }

    [Fact]
    public async Task Handle_Should_ReturnPagedUsers_When_UsersExist()
    {
        var users = new List<UserListDto>
        {
            new() { UserId = Guid.NewGuid(), FullName = "User 1" },
            new() { UserId = Guid.NewGuid(), FullName = "User 2" }
        };
        _userProfileRepositoryMock
            .Setup(r => r.SearchProfilesAsync(It.IsAny<UserSearchRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((users, 2));

        var result = await _handler.Handle(
            new GetPagedUsersQuery { Page = 1, PageSize = 10 },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_Should_UseDefaultSort_When_NoSortByProvided()
    {
        _userProfileRepositoryMock
            .Setup(r => r.SearchProfilesAsync(It.IsAny<UserSearchRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<UserListDto>(), 0));

        await _handler.Handle(
            new GetPagedUsersQuery { SortBy = null },
            CancellationToken.None);

        _userProfileRepositoryMock.Verify(r => r.SearchProfilesAsync(
            It.Is<UserSearchRequest>(req => req.SortBy == "CreatedDate"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_PassSortBy_When_Provided()
    {
        _userProfileRepositoryMock
            .Setup(r => r.SearchProfilesAsync(It.IsAny<UserSearchRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<UserListDto>(), 0));

        await _handler.Handle(
            new GetPagedUsersQuery { SortBy = "name", SortDescending = true },
            CancellationToken.None);

        _userProfileRepositoryMock.Verify(r => r.SearchProfilesAsync(
            It.Is<UserSearchRequest>(req =>
                req.SortBy == "name" &&
                req.SortDescending == true),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_CalculateTotalPages_Correctly()
    {
        _userProfileRepositoryMock
            .Setup(r => r.SearchProfilesAsync(It.IsAny<UserSearchRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<UserListDto>(), 45));

        var result = await _handler.Handle(
            new GetPagedUsersQuery { Page = 1, PageSize = 20 },
            CancellationToken.None);

        result.Value!.TotalPages.Should().Be(3);
        result.Value.TotalRecords.Should().Be(45);
    }

    [Fact]
    public async Task Handle_Should_PassPageAndPageSize_ToRepository()
    {
        _userProfileRepositoryMock
            .Setup(r => r.SearchProfilesAsync(It.IsAny<UserSearchRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<UserListDto>(), 0));

        await _handler.Handle(
            new GetPagedUsersQuery { Page = 3, PageSize = 25 },
            CancellationToken.None);

        _userProfileRepositoryMock.Verify(r => r.SearchProfilesAsync(
            It.Is<UserSearchRequest>(req =>
                req.Page == 3 &&
                req.PageSize == 25),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
