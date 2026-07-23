using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.UserManagement.Queries.GetUserById;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.UnitTests.UserManagement;

public class GetUserByIdQueryHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IUserProfileRepository> _userProfileRepositoryMock;
    private readonly Mock<ILogger<GetUserByIdQueryHandler>> _loggerMock;
    private readonly GetUserByIdQueryHandler _handler;

    private readonly Guid _userId = Guid.NewGuid();

    public GetUserByIdQueryHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _userProfileRepositoryMock = new Mock<IUserProfileRepository>();
        _loggerMock = new Mock<ILogger<GetUserByIdQueryHandler>>();
        _handler = new GetUserByIdQueryHandler(
            _userRepositoryMock.Object,
            _userProfileRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_UserNotFound()
    {
        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var result = await _handler.Handle(
            new GetUserByIdQuery { UserId = _userId },
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("User not found.");
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_ProfileNotFound()
    {
        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateTestUser());
        _userProfileRepositoryMock
            .Setup(r => r.GetFullProfileAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserProfile?)null);

        var result = await _handler.Handle(
            new GetUserByIdQuery { UserId = _userId },
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Profile not found.");
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_ProfileIsDeleted()
    {
        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateTestUser());
        _userProfileRepositoryMock
            .Setup(r => r.GetFullProfileAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UserProfile { IsDeleted = true, Addresses = new List<Address>() });

        var result = await _handler.Handle(
            new GetUserByIdQuery { UserId = _userId },
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Profile not found.");
    }

    [Fact]
    public async Task Handle_Should_ReturnProfile_When_ValidRequest()
    {
        var user = CreateTestUser();
        var profile = CreateTestProfile();

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _userProfileRepositoryMock
            .Setup(r => r.GetFullProfileAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(profile);

        var result = await _handler.Handle(
            new GetUserByIdQuery { UserId = _userId },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.UserId.Should().Be(_userId);
        result.Value.FullName.Should().Be("Test User");
    }

    [Fact]
    public async Task Handle_Should_MapRoles_When_UserHasRoles()
    {
        var user = CreateTestUser();
        user.UserRoles = new List<UserRole>
        {
            new() { Role = new Role { Name = "Coach" } }
        };
        var profile = CreateTestProfile();

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _userProfileRepositoryMock
            .Setup(r => r.GetFullProfileAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(profile);

        var result = await _handler.Handle(
            new GetUserByIdQuery { UserId = _userId },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Roles.Should().Contain("Coach");
    }

    private User CreateTestUser() => new()
    {
        Id = _userId,
        Email = "test@example.com",
        FullName = "Test User",
        Status = UserStatus.Active,
        UserRoles = new List<UserRole>()
    };

    private UserProfile CreateTestProfile() => new()
    {
        Id = Guid.NewGuid(),
        UserId = _userId,
        IsDeleted = false,
        Gender = Gender.Male,
        Addresses = new List<Address>(),
        ContactInformation = null,
        UserPreference = null,
        User = CreateTestUser()
    };
}
