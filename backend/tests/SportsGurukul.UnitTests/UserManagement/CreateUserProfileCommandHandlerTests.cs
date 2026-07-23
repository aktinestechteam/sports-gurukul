using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.UserManagement.Commands.CreateUserProfile;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.UnitTests.UserManagement;

public class CreateUserProfileCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IUserProfileRepository> _userProfileRepositoryMock;
    private readonly Mock<IRepository<ContactInformation>> _contactRepositoryMock;
    private readonly Mock<IRepository<Address>> _addressRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<CreateUserProfileCommandHandler>> _loggerMock;
    private readonly CreateUserProfileCommandHandler _handler;

    private readonly Guid _userId = Guid.NewGuid();

    public CreateUserProfileCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _userProfileRepositoryMock = new Mock<IUserProfileRepository>();
        _contactRepositoryMock = new Mock<IRepository<ContactInformation>>();
        _addressRepositoryMock = new Mock<IRepository<Address>>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<CreateUserProfileCommandHandler>>();
        _handler = new CreateUserProfileCommandHandler(
            _userRepositoryMock.Object,
            _userProfileRepositoryMock.Object,
            _contactRepositoryMock.Object,
            _addressRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_UserNotFound()
    {
        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var result = await _handler.Handle(
            new CreateUserProfileCommand { UserId = _userId },
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("User not found.");
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_ProfileAlreadyExistsAndNotDeleted()
    {
        var user = CreateTestUser();
        var existingProfile = CreateTestProfile(isDeleted: false);

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _userProfileRepositoryMock
            .Setup(r => r.GetByUserIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingProfile);

        var result = await _handler.Handle(
            new CreateUserProfileCommand { UserId = _userId },
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("A profile already exists for this user.");
    }

    [Fact]
    public async Task Handle_Should_CreateProfile_When_ValidRequestWithNoPhoneOrAddress()
    {
        var user = CreateTestUser();

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _userProfileRepositoryMock
            .Setup(r => r.GetByUserIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserProfile?)null);
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(
            new CreateUserProfileCommand
            {
                UserId = _userId,
                Gender = Gender.Male,
                Bio = "Test bio",
                Height = "180cm",
                Weight = "75kg"
            },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.UserId.Should().Be(_userId);
        result.Value.Bio.Should().Be("Test bio");
        result.Value.Height.Should().Be("180cm");
        result.Value.Weight.Should().Be("75kg");
        result.Value.Gender.Should().Be(Gender.Male);
    }

    [Fact]
    public async Task Handle_Should_AddContact_When_PhoneNumberProvided()
    {
        var user = CreateTestUser();

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _userProfileRepositoryMock
            .Setup(r => r.GetByUserIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserProfile?)null);
        _userRepositoryMock
            .Setup(r => r.GetByPhoneNumberAsync("1234567890", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(
            new CreateUserProfileCommand
            {
                UserId = _userId,
                PrimaryPhoneNumber = "1234567890",
                PrimaryPhoneCountryCode = "+91"
            },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _contactRepositoryMock.Verify(r => r.AddAsync(
            It.Is<ContactInformation>(c =>
                c.PrimaryPhoneNumber == "1234567890" &&
                c.PrimaryPhoneCountryCode == "+91"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_AddAddress_When_AddressProvided()
    {
        var user = CreateTestUser();

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _userProfileRepositoryMock
            .Setup(r => r.GetByUserIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserProfile?)null);
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(
            new CreateUserProfileCommand
            {
                UserId = _userId,
                AddressLine1 = "123 Main St",
                City = "Mumbai",
                State = "Maharashtra",
                Country = "India",
                PostalCode = "400001",
                AddressType = AddressType.Home
            },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _addressRepositoryMock.Verify(r => r.AddAsync(
            It.Is<Address>(a =>
                a.Line1 == "123 Main St" &&
                a.City == "Mumbai" &&
                a.State == "Maharashtra" &&
                a.Country == "India" &&
                a.PostalCode == "400001" &&
                a.AddressType == AddressType.Home &&
                a.IsPrimary == true),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_DuplicatePhoneNumber()
    {
        var user = CreateTestUser();
        var existingUserWithPhone = new User
        {
            Id = Guid.NewGuid(),
            PhoneNumber = "1234567890"
        };

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _userProfileRepositoryMock
            .Setup(r => r.GetByUserIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserProfile?)null);
        _userRepositoryMock
            .Setup(r => r.GetByPhoneNumberAsync("1234567890", It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUserWithPhone);

        var result = await _handler.Handle(
            new CreateUserProfileCommand
            {
                UserId = _userId,
                PrimaryPhoneNumber = "1234567890"
            },
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("This phone number is already associated with another account.");
        _contactRepositoryMock.Verify(r => r.AddAsync(
            It.IsAny<ContactInformation>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_RestoreDeletedProfile_When_ProfileWasSoftDeleted()
    {
        var user = CreateTestUser();
        var deletedProfile = CreateTestProfile(isDeleted: true);

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _userProfileRepositoryMock
            .Setup(r => r.GetByUserIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(deletedProfile);
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(
            new CreateUserProfileCommand
            {
                UserId = _userId,
                Bio = "Restored bio"
            },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        deletedProfile.IsDeleted.Should().BeFalse();
        deletedProfile.Bio.Should().Be("Restored bio");
        deletedProfile.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_Should_CallSaveChanges_When_ProfileCreated()
    {
        var user = CreateTestUser();

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _userProfileRepositoryMock
            .Setup(r => r.GetByUserIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserProfile?)null);
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _handler.Handle(
            new CreateUserProfileCommand { UserId = _userId },
            CancellationToken.None);

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_MapRoles_When_UserHasRoles()
    {
        var user = CreateTestUser();
        user.UserRoles = new List<UserRole>
        {
            new() { Role = new Role { Name = "Athlete" } }
        };

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _userProfileRepositoryMock
            .Setup(r => r.GetByUserIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserProfile?)null);
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(
            new CreateUserProfileCommand { UserId = _userId },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Roles.Should().Contain("Athlete");
    }

    [Fact]
    public async Task Handle_Should_SetDefaultCountryCode_When_PhoneProvidedWithoutCountryCode()
    {
        var user = CreateTestUser();

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _userProfileRepositoryMock
            .Setup(r => r.GetByUserIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserProfile?)null);
        _userRepositoryMock
            .Setup(r => r.GetByPhoneNumberAsync("1234567890", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(
            new CreateUserProfileCommand
            {
                UserId = _userId,
                PrimaryPhoneNumber = "1234567890"
            },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _contactRepositoryMock.Verify(r => r.AddAsync(
            It.Is<ContactInformation>(c => c.PrimaryPhoneCountryCode == "+91"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_NotAddAddress_When_OnlyAddressLine1ProvidedWithoutCity()
    {
        var user = CreateTestUser();

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _userProfileRepositoryMock
            .Setup(r => r.GetByUserIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserProfile?)null);
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(
            new CreateUserProfileCommand
            {
                UserId = _userId,
                AddressLine1 = "123 Main St"
            },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _addressRepositoryMock.Verify(r => r.AddAsync(
            It.IsAny<Address>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_AllowSamePhoneNumber_When_OwnedBySameUser()
    {
        var user = CreateTestUser();

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _userProfileRepositoryMock
            .Setup(r => r.GetByUserIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserProfile?)null);
        _userRepositoryMock
            .Setup(r => r.GetByPhoneNumberAsync("1234567890", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(
            new CreateUserProfileCommand
            {
                UserId = _userId,
                PrimaryPhoneNumber = "1234567890"
            },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    private User CreateTestUser() => new()
    {
        Id = _userId,
        Email = "test@example.com",
        FullName = "Test User",
        PhoneNumber = "9876543210",
        Status = UserStatus.Active,
        UserRoles = new List<UserRole>()
    };

    private UserProfile CreateTestProfile(bool isDeleted) => new()
    {
        Id = Guid.NewGuid(),
        UserId = _userId,
        IsDeleted = isDeleted,
        Gender = Gender.PreferNotToSay,
        Addresses = new List<Address>(),
        ContactInformation = null,
        UserPreference = null
    };
}
