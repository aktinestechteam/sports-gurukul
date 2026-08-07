using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.UserManagement.Queries.GetCurrentUser;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.UnitTests.UserManagement;

public class GetCurrentUserQueryHandlerTests
{
    private readonly Mock<ICurrentUser> _currentUserMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IUserProfileRepository> _userProfileRepositoryMock;
    private readonly Mock<ILogger<GetCurrentUserQueryHandler>> _loggerMock;
    private readonly GetCurrentUserQueryHandler _handler;

    private readonly Guid _userId = Guid.NewGuid();

    public GetCurrentUserQueryHandlerTests()
    {
        _currentUserMock = new Mock<ICurrentUser>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _userProfileRepositoryMock = new Mock<IUserProfileRepository>();
        _loggerMock = new Mock<ILogger<GetCurrentUserQueryHandler>>();
        _handler = new GetCurrentUserQueryHandler(
            _currentUserMock.Object,
            _userRepositoryMock.Object,
            _userProfileRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_NotAuthenticated()
    {
        _currentUserMock.Setup(c => c.UserId).Returns((Guid?)null);

        var result = await _handler.Handle(
            new GetCurrentUserQuery(),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("User not authenticated.");
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_UserNotFound()
    {
        _currentUserMock.Setup(c => c.UserId).Returns(_userId);
        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var result = await _handler.Handle(
            new GetCurrentUserQuery(),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("User not found.");
    }

    [Fact]
    public async Task Handle_Should_ReturnIdentityOnly_When_ProfileNotFound()
    {
        _currentUserMock.Setup(c => c.UserId).Returns(_userId);
        var user = CreateTestUser();
        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _userProfileRepositoryMock
            .Setup(r => r.GetFullProfileAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserProfile?)null);

        var result = await _handler.Handle(
            new GetCurrentUserQuery(),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.HasProfile.Should().BeFalse();
        result.Value.UserId.Should().Be(_userId);
        result.Value.FullName.Should().Be("Test User");
        result.Value.Email.Should().Be("test@example.com");
        result.Value.Roles.Should().Contain("Athlete");
    }

    [Fact]
    public async Task Handle_Should_ReturnIdentityOnly_When_ProfileIsDeleted()
    {
        _currentUserMock.Setup(c => c.UserId).Returns(_userId);
        var user = CreateTestUser();
        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _userProfileRepositoryMock
            .Setup(r => r.GetFullProfileAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UserProfile { IsDeleted = true, Addresses = new List<Address>() });

        var result = await _handler.Handle(
            new GetCurrentUserQuery(),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.HasProfile.Should().BeFalse();
        result.Value.UserId.Should().Be(_userId);
    }

    [Fact]
    public async Task Handle_Should_ReturnProfile_When_ValidRequest()
    {
        _currentUserMock.Setup(c => c.UserId).Returns(_userId);
        var user = CreateTestUser();
        var profile = CreateTestProfile();

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _userProfileRepositoryMock
            .Setup(r => r.GetFullProfileAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(profile);

        var result = await _handler.Handle(
            new GetCurrentUserQuery(),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.HasProfile.Should().BeTrue();
        result.Value.UserId.Should().Be(_userId);
        result.Value.FullName.Should().Be("Test User");
        result.Value.Email.Should().Be("test@example.com");
    }

    [Fact]
    public async Task Handle_Should_MapBio_When_ProfileHasBio()
    {
        _currentUserMock.Setup(c => c.UserId).Returns(_userId);
        var user = CreateTestUser();
        var profile = CreateTestProfile();
        profile.Bio = "I am an athlete";

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _userProfileRepositoryMock
            .Setup(r => r.GetFullProfileAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(profile);

        var result = await _handler.Handle(
            new GetCurrentUserQuery(),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Bio.Should().Be("I am an athlete");
    }

    private User CreateTestUser() => new()
    {
        Id = _userId,
        Email = "test@example.com",
        FullName = "Test User",
        Status = UserStatus.Active,
        UserRoles = new List<UserRole>
        {
            new() { Role = new Role { Name = "Athlete" } }
        }
    };

    private UserProfile CreateTestProfile() => new()
    {
        Id = Guid.NewGuid(),
        UserId = _userId,
        IsDeleted = false,
        Gender = Gender.Male,
        Bio = "Test bio",
        Addresses = new List<Address>(),
        ContactInformation = null,
        UserPreference = null,
        User = CreateTestUser()
    };
}
