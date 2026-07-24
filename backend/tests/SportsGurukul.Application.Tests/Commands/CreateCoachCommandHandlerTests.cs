using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.CoachManagement.Commands.CreateCoach;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Tests.Commands;

public class CreateCoachCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock = TestMocks.CreateUserRepository();
    private readonly Mock<ICoachRepository> _coachRepositoryMock = TestMocks.CreateCoachRepository();
    private readonly Mock<ICoachAvailabilityRepository> _coachAvailabilityRepositoryMock = TestMocks.CreateCoachAvailabilityRepository();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = TestMocks.CreateUnitOfWork();
    private readonly Mock<ILogger<CreateCoachCommandHandler>> _loggerMock = TestMocks.CreateLogger<CreateCoachCommandHandler>();
    private readonly CreateCoachCommandHandler _handler;

    public CreateCoachCommandHandlerTests()
    {
        _handler = new CreateCoachCommandHandler(
            _userRepositoryMock.Object,
            _coachRepositoryMock.Object,
            _coachAvailabilityRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_UserNotFound_ReturnsFailure()
    {
        var userId = Guid.NewGuid();
        _userRepositoryMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var result = await _handler.Handle(new CreateCoachCommand { UserId = userId }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("User not found.");
    }

    [Fact]
    public async Task Handle_CoachAlreadyExistsForUser_ReturnsFailure()
    {
        var userId = Guid.NewGuid();
        var user = TestDataBuilder.CreateUser(userId);
        var existingCoach = TestDataBuilder.CreateCoach(userId: userId);

        _userRepositoryMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _coachRepositoryMock.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCoach);

        var result = await _handler.Handle(new CreateCoachCommand { UserId = userId }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("A coach profile already exists for this user.");
    }

    [Fact]
    public async Task Handle_NewCoach_CreatesCoachAndReturnsSuccess()
    {
        var userId = Guid.NewGuid();
        var user = TestDataBuilder.CreateUser(userId);

        _userRepositoryMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _coachRepositoryMock.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Coach?)null);
        _coachRepositoryMock.Setup(r => r.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Coach, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _coachRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Coach>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Coach());
        _coachAvailabilityRepositoryMock.Setup(r => r.AddAsync(It.IsAny<CoachAvailability>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CoachAvailability());
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(new CreateCoachCommand
        {
            UserId = userId,
            Biography = "New coach bio",
            YearsOfExperience = 3,
            CurrentOrganization = "New Academy",
            HighestQualification = "BPEd",
            PreferredLanguage = "Hindi",
            CoachingLevel = CoachingLevel.Junior
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.UserId.Should().Be(userId);
        result.Value.Biography.Should().Be("New coach bio");
        result.Value.YearsOfExperience.Should().Be(3);
        result.Value.CurrentOrganization.Should().Be("New Academy");
        result.Value.HighestQualification.Should().Be("BPEd");
        result.Value.PreferredLanguage.Should().Be("Hindi");
        result.Value.CoachingLevel.Should().Be("Junior");
        result.Value.Status.Should().Be("Active");
        result.Value.VerificationStatus.Should().Be("Pending");
        _coachRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Coach>(), It.IsAny<CancellationToken>()), Times.Once);
        _coachAvailabilityRepositoryMock.Verify(r => r.AddAsync(It.IsAny<CoachAvailability>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NewCoach_GeneratesUniqueCoachCode()
    {
        var userId = Guid.NewGuid();
        var user = TestDataBuilder.CreateUser(userId);
        string? capturedCode = null;

        _userRepositoryMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _coachRepositoryMock.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Coach?)null);
        _coachRepositoryMock.Setup(r => r.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Coach, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _coachRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Coach>(), It.IsAny<CancellationToken>()))
            .Callback<Coach, CancellationToken>((c, _) => capturedCode = c.CoachCode)
            .ReturnsAsync(new Coach());
        _coachAvailabilityRepositoryMock.Setup(r => r.AddAsync(It.IsAny<CoachAvailability>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CoachAvailability());
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _handler.Handle(new CreateCoachCommand { UserId = userId }, CancellationToken.None);

        capturedCode.Should().NotBeNullOrEmpty();
        capturedCode.Should().StartWith("COACH-");
    }

    [Fact]
    public async Task Handle_NewCoach_SetsRegistrationDateAndActiveStatus()
    {
        var userId = Guid.NewGuid();
        var user = TestDataBuilder.CreateUser(userId);
        Coach? capturedCoach = null;

        _userRepositoryMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _coachRepositoryMock.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Coach?)null);
        _coachRepositoryMock.Setup(r => r.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Coach, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _coachRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Coach>(), It.IsAny<CancellationToken>()))
            .Callback<Coach, CancellationToken>((c, _) => capturedCoach = c)
            .ReturnsAsync(new Coach());
        _coachAvailabilityRepositoryMock.Setup(r => r.AddAsync(It.IsAny<CoachAvailability>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CoachAvailability());
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _handler.Handle(new CreateCoachCommand { UserId = userId }, CancellationToken.None);

        capturedCoach.Should().NotBeNull();
        capturedCoach!.Status.Should().Be(CoachStatus.Active);
        capturedCoach.VerificationStatus.Should().Be(VerificationStatus.Pending);
        capturedCoach.RegistrationDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Handle_MapToDto_MapsAllFields()
    {
        var coach = TestDataBuilder.CreateCoachWithDetails();
        var user = coach.User;

        var dto = CreateCoachCommandHandler.MapToDto(coach, user);

        dto.Id.Should().Be(coach.Id);
        dto.UserId.Should().Be(coach.UserId);
        dto.CoachCode.Should().Be(coach.CoachCode);
        dto.FullName.Should().Be(user.FullName);
        dto.Email.Should().Be(user.Email);
        dto.Biography.Should().Be(coach.Biography);
        dto.YearsOfExperience.Should().Be(coach.YearsOfExperience);
        dto.CurrentOrganization.Should().Be(coach.CurrentOrganization);
        dto.HighestQualification.Should().Be(coach.HighestQualification);
        dto.PreferredLanguage.Should().Be(coach.PreferredLanguage);
        dto.CoachingLevel.Should().Be(coach.CoachingLevel.ToString());
        dto.Status.Should().Be(coach.Status.ToString());
        dto.VerificationStatus.Should().Be(coach.VerificationStatus.ToString());
        dto.Availability.Should().NotBeNull();
        dto.Location.Should().NotBeNull();
        dto.Sports.Should().HaveCount(1);
        dto.Certifications.Should().HaveCount(1);
        dto.Experiences.Should().HaveCount(1);
        dto.Education.Should().HaveCount(1);
    }
}
