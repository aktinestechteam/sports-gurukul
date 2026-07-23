using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.UserManagement.Commands.UpdateUserProfile;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.UnitTests.UserManagement;

public class UpdateUserProfileCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IUserProfileRepository> _userProfileRepositoryMock;
    private readonly Mock<IRepository<ContactInformation>> _contactRepositoryMock;
    private readonly Mock<IRepository<Address>> _addressRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<UpdateUserProfileCommandHandler>> _loggerMock;
    private readonly UpdateUserProfileCommandHandler _handler;

    private readonly Guid _userId = Guid.NewGuid();

    public UpdateUserProfileCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _userProfileRepositoryMock = new Mock<IUserProfileRepository>();
        _contactRepositoryMock = new Mock<IRepository<ContactInformation>>();
        _addressRepositoryMock = new Mock<IRepository<Address>>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<UpdateUserProfileCommandHandler>>();
        _handler = new UpdateUserProfileCommandHandler(
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
            new UpdateUserProfileCommand { UserId = _userId },
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("User not found.");
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_ProfileNotFound()
    {
        var user = CreateTestUser();

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _userProfileRepositoryMock
            .Setup(r => r.GetFullProfileAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserProfile?)null);

        var result = await _handler.Handle(
            new UpdateUserProfileCommand { UserId = _userId },
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Profile not found. Please create a profile first.");
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_ProfileIsDeleted()
    {
        var user = CreateTestUser();
        var profile = CreateTestProfile();
        profile.IsDeleted = true;

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _userProfileRepositoryMock
            .Setup(r => r.GetFullProfileAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(profile);

        var result = await _handler.Handle(
            new UpdateUserProfileCommand { UserId = _userId },
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Profile has been deleted. Please restore it first.");
    }

    [Fact]
    public async Task Handle_Should_UpdateProfile_When_ValidRequestWithBioAndGender()
    {
        var user = CreateTestUser();
        var profile = CreateTestProfile();

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _userProfileRepositoryMock
            .Setup(r => r.GetFullProfileAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(profile);
        _userProfileRepositoryMock
            .Setup(r => r.GetFullProfileAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(profile);
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(
            new UpdateUserProfileCommand
            {
                UserId = _userId,
                Bio = "Updated bio",
                Gender = Gender.Female,
                Height = "165cm",
                Weight = "55kg"
            },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        profile.Bio.Should().Be("Updated bio");
        profile.Gender.Should().Be(Gender.Female);
        profile.Height.Should().Be("165cm");
        profile.Weight.Should().Be("55kg");
        profile.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_Should_CreateContact_When_PhoneProvidedAndNoExistingContact()
    {
        var user = CreateTestUser();
        var profile = CreateTestProfile();
        profile.ContactInformation = null;

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _userProfileRepositoryMock
            .Setup(r => r.GetFullProfileAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(profile);
        _userProfileRepositoryMock
            .Setup(r => r.GetFullProfileAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(profile);
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(
            new UpdateUserProfileCommand
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
    public async Task Handle_Should_UpdateContact_When_PhoneNumberChanged()
    {
        var user = CreateTestUser();
        var profile = CreateTestProfile();
        profile.ContactInformation = new ContactInformation
        {
            Id = Guid.NewGuid(),
            UserProfileId = profile.Id,
            PrimaryPhoneNumber = "0987654321",
            PrimaryPhoneCountryCode = "+91"
        };

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _userProfileRepositoryMock
            .Setup(r => r.GetFullProfileAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(profile);
        _userProfileRepositoryMock
            .Setup(r => r.GetFullProfileAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(profile);
        _userRepositoryMock
            .Setup(r => r.GetByPhoneNumberAsync("1234567890", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(
            new UpdateUserProfileCommand
            {
                UserId = _userId,
                PrimaryPhoneNumber = "1234567890"
            },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        profile.ContactInformation.PrimaryPhoneNumber.Should().Be("1234567890");
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_DuplicatePhoneNumberFromAnotherUser()
    {
        var user = CreateTestUser();
        var profile = CreateTestProfile();
        profile.ContactInformation = new ContactInformation
        {
            Id = Guid.NewGuid(),
            UserProfileId = profile.Id,
            PrimaryPhoneNumber = "0987654321"
        };
        var otherUser = new User { Id = Guid.NewGuid() };

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _userProfileRepositoryMock
            .Setup(r => r.GetFullProfileAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(profile);
        _userProfileRepositoryMock
            .Setup(r => r.GetFullProfileAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(profile);
        _userRepositoryMock
            .Setup(r => r.GetByPhoneNumberAsync("1234567890", It.IsAny<CancellationToken>()))
            .ReturnsAsync(otherUser);

        var result = await _handler.Handle(
            new UpdateUserProfileCommand
            {
                UserId = _userId,
                PrimaryPhoneNumber = "1234567890"
            },
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("This phone number is already associated with another account.");
    }

    [Fact]
    public async Task Handle_Should_CreateAddress_When_AddressProvidedAndNoPrimaryExists()
    {
        var user = CreateTestUser();
        var profile = CreateTestProfile();
        profile.Addresses = new List<Address>();

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _userProfileRepositoryMock
            .Setup(r => r.GetFullProfileAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(profile);
        _userProfileRepositoryMock
            .Setup(r => r.GetFullProfileAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(profile);
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(
            new UpdateUserProfileCommand
            {
                UserId = _userId,
                AddressLine1 = "456 Oak Ave",
                City = "Delhi",
                State = "Delhi",
                Country = "India",
                PostalCode = "110001"
            },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _addressRepositoryMock.Verify(r => r.AddAsync(
            It.Is<Address>(a =>
                a.Line1 == "456 Oak Ave" &&
                a.City == "Delhi" &&
                a.IsPrimary == true),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_UpdateExistingAddress_When_PrimaryAddressExists()
    {
        var user = CreateTestUser();
        var profile = CreateTestProfile();
        var existingAddress = new Address
        {
            Id = Guid.NewGuid(),
            UserProfileId = profile.Id,
            Line1 = "Old Address",
            City = "Old City",
            IsPrimary = true,
            IsDeleted = false
        };
        profile.Addresses = new List<Address> { existingAddress };

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _userProfileRepositoryMock
            .Setup(r => r.GetFullProfileAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(profile);
        _userProfileRepositoryMock
            .Setup(r => r.GetFullProfileAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(profile);
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(
            new UpdateUserProfileCommand
            {
                UserId = _userId,
                AddressLine1 = "New Address",
                City = "New City"
            },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        existingAddress.Line1.Should().Be("New Address");
        existingAddress.City.Should().Be("New City");
    }

    [Fact]
    public async Task Handle_Should_NotUpdatePhone_When_SamePhoneNumberProvided()
    {
        var user = CreateTestUser();
        var profile = CreateTestProfile();
        profile.ContactInformation = new ContactInformation
        {
            Id = Guid.NewGuid(),
            UserProfileId = profile.Id,
            PrimaryPhoneNumber = "1234567890"
        };

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _userProfileRepositoryMock
            .Setup(r => r.GetFullProfileAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(profile);
        _userProfileRepositoryMock
            .Setup(r => r.GetFullProfileAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(profile);
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(
            new UpdateUserProfileCommand
            {
                UserId = _userId,
                PrimaryPhoneNumber = "1234567890"
            },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _contactRepositoryMock.Verify(r => r.Update(
            It.IsAny<ContactInformation>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_PreserveExistingValues_When_NullFieldsProvided()
    {
        var user = CreateTestUser();
        var profile = CreateTestProfile();
        profile.Bio = "Original bio";
        profile.Height = "180cm";

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _userProfileRepositoryMock
            .Setup(r => r.GetFullProfileAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(profile);
        _userProfileRepositoryMock
            .Setup(r => r.GetFullProfileAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(profile);
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(
            new UpdateUserProfileCommand
            {
                UserId = _userId,
                Bio = null,
                Height = null
            },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        profile.Bio.Should().Be("Original bio");
        profile.Height.Should().Be("180cm");
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
        Gender = Gender.PreferNotToSay,
        Addresses = new List<Address>(),
        ContactInformation = null,
        UserPreference = null
    };
}
