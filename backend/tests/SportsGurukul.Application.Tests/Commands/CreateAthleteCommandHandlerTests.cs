using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.AthleteManagement.Commands.CreateAthlete;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Tests.Commands;

public class CreateAthleteCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock = TestMocks.CreateUserRepository();
    private readonly Mock<IAthleteRepository> _athleteRepositoryMock = TestMocks.CreateAthleteRepository();
    private readonly Mock<IRepository<MedicalProfile>> _medicalProfileRepositoryMock = TestMocks.CreateMedicalProfileRepository();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = TestMocks.CreateUnitOfWork();
    private readonly Mock<ILogger<CreateAthleteCommandHandler>> _loggerMock = TestMocks.CreateLogger<CreateAthleteCommandHandler>();
    private readonly CreateAthleteCommandHandler _handler;

    public CreateAthleteCommandHandlerTests()
    {
        _handler = new CreateAthleteCommandHandler(
            _userRepositoryMock.Object,
            _athleteRepositoryMock.Object,
            _medicalProfileRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_UserNotFound_ReturnsFailure()
    {
        var userId = Guid.NewGuid();
        _userRepositoryMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var result = await _handler.Handle(new CreateAthleteCommand { UserId = userId }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("User not found.");
    }

    [Fact]
    public async Task Handle_AthleteAlreadyExists_ReturnsFailure()
    {
        var userId = Guid.NewGuid();
        var user = TestDataBuilder.CreateUser(userId);
        var existingAthlete = TestDataBuilder.CreateAthlete(userId: userId);

        _userRepositoryMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _athleteRepositoryMock.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingAthlete);

        var result = await _handler.Handle(new CreateAthleteCommand { UserId = userId }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("An athlete profile already exists for this user.");
    }

    [Fact]
    public async Task Handle_DeletedAthleteExists_RestoresAndReturnsSuccess()
    {
        var userId = Guid.NewGuid();
        var user = TestDataBuilder.CreateUser(userId);
        var deletedAthlete = TestDataBuilder.CreateAthlete(userId: userId, isDeleted: true);

        _userRepositoryMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _athleteRepositoryMock.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(deletedAthlete);
        _athleteRepositoryMock.Setup(r => r.GetByAthleteCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Athlete?)null);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(new CreateAthleteCommand
        {
            UserId = userId,
            CurrentLevel = AthleteLevel.Advanced,
            ExperienceYears = 10
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        deletedAthlete.IsDeleted.Should().BeFalse();
        deletedAthlete.Status.Should().Be(AthleteStatus.Active);
    }

    [Fact]
    public async Task Handle_NewAthlete_CreatesAthleteAndReturnsSuccess()
    {
        var userId = Guid.NewGuid();
        var user = TestDataBuilder.CreateUser(userId);

        _userRepositoryMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _athleteRepositoryMock.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Athlete?)null);
        _athleteRepositoryMock.Setup(r => r.GetByAthleteCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Athlete?)null);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(new CreateAthleteCommand
        {
            UserId = userId,
            CurrentLevel = AthleteLevel.Beginner,
            ExperienceYears = 2,
            Height = "170cm",
            Weight = "65kg",
            BloodGroup = BloodGroup.BPositive,
            DominantHand = DominantHand.Right,
            DominantFoot = DominantFoot.Right,
            Biography = "New athlete"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.UserId.Should().Be(userId);
        result.Value.CurrentLevel.Should().Be("Beginner");
        result.Value.ExperienceYears.Should().Be(2);
        result.Value.Height.Should().Be("170cm");
        result.Value.Weight.Should().Be("65kg");
        result.Value.BloodGroup.Should().Be("BPositive");
        result.Value.DominantHand.Should().Be("Right");
        result.Value.DominantFoot.Should().Be("Right");
        result.Value.Biography.Should().Be("New athlete");
        _athleteRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Athlete>(), It.IsAny<CancellationToken>()), Times.Once);
        _medicalProfileRepositoryMock.Verify(r => r.AddAsync(It.IsAny<MedicalProfile>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NewAthlete_GeneratesUniqueAthleteCode()
    {
        var userId = Guid.NewGuid();
        var user = TestDataBuilder.CreateUser(userId);
        string? capturedCode = null;

        _userRepositoryMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _athleteRepositoryMock.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Athlete?)null);
        _athleteRepositoryMock.Setup(r => r.GetByAthleteCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Athlete?)null);
        _athleteRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Athlete>(), It.IsAny<CancellationToken>()))
            .Callback<Athlete, CancellationToken>((a, _) => capturedCode = a.AthleteCode)
            .ReturnsAsync(new Athlete());
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _handler.Handle(new CreateAthleteCommand { UserId = userId }, CancellationToken.None);

        capturedCode.Should().NotBeNullOrEmpty();
        capturedCode.Should().StartWith("ATH-");
    }

    [Fact]
    public async Task Handle_NewAthlete_SetsRegistrationDateAndActiveStatus()
    {
        var userId = Guid.NewGuid();
        var user = TestDataBuilder.CreateUser(userId);
        Athlete? capturedAthlete = null;

        _userRepositoryMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _athleteRepositoryMock.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Athlete?)null);
        _athleteRepositoryMock.Setup(r => r.GetByAthleteCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Athlete?)null);
        _athleteRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Athlete>(), It.IsAny<CancellationToken>()))
            .Callback<Athlete, CancellationToken>((a, _) => capturedAthlete = a)
            .ReturnsAsync(new Athlete());
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _handler.Handle(new CreateAthleteCommand { UserId = userId }, CancellationToken.None);

        capturedAthlete.Should().NotBeNull();
        capturedAthlete!.Status.Should().Be(AthleteStatus.Active);
        capturedAthlete.RegistrationDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Handle_MapToDto_MapsAllFields()
    {
        var athlete = TestDataBuilder.CreateAthleteWithDetails();
        var user = athlete.User;

        var dto = CreateAthleteCommandHandler.MapToDto(athlete, user);

        dto.Id.Should().Be(athlete.Id);
        dto.UserId.Should().Be(athlete.UserId);
        dto.AthleteCode.Should().Be(athlete.AthleteCode);
        dto.FullName.Should().Be(user.FullName);
        dto.Email.Should().Be(user.Email);
        dto.CurrentLevel.Should().Be(athlete.CurrentLevel.ToString());
        dto.ExperienceYears.Should().Be(athlete.ExperienceYears);
        dto.Height.Should().Be(athlete.Height);
        dto.Weight.Should().Be(athlete.Weight);
        dto.Status.Should().Be(athlete.Status.ToString());
        dto.MedicalProfile.Should().NotBeNull();
        dto.EmergencyContact.Should().NotBeNull();
        dto.Ranking.Should().NotBeNull();
        dto.Sports.Should().HaveCount(1);
        dto.Achievements.Should().HaveCount(1);
    }
}
