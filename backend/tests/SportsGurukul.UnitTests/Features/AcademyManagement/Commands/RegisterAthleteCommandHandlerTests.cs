using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.AcademyManagement.Commands.RegisterAthlete;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.UnitTests.Features.AcademyManagement.Commands;

public class RegisterAthleteCommandHandlerTests
{
    private readonly Mock<IAcademyRepository> _academyRepositoryMock;
    private readonly Mock<IAthleteRepository> _athleteRepositoryMock;
    private readonly Mock<IAthleteAcademyRepository> _athleteAcademyRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<RegisterAthleteCommandHandler>> _loggerMock;
    private readonly RegisterAthleteCommandHandler _handler;

    public RegisterAthleteCommandHandlerTests()
    {
        _academyRepositoryMock = new Mock<IAcademyRepository>();
        _athleteRepositoryMock = new Mock<IAthleteRepository>();
        _athleteAcademyRepositoryMock = new Mock<IAthleteAcademyRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<RegisterAthleteCommandHandler>>();
        _handler = new RegisterAthleteCommandHandler(
            _academyRepositoryMock.Object,
            _athleteRepositoryMock.Object,
            _athleteAcademyRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_RegistersAthlete()
    {
        var academyId = Guid.NewGuid();
        var athleteId = Guid.NewGuid();
        var academy = CreateTestAcademy(academyId);
        academy.VerificationStatus = VerificationStatus.Verified;
        var athlete = CreateTestAthlete(athleteId);

        _academyRepositoryMock
            .Setup(r => r.GetByIdAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(academy);
        _athleteRepositoryMock
            .Setup(r => r.GetByIdAsync(athleteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(athlete);
        _athleteAcademyRepositoryMock
            .Setup(r => r.AnyAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<AthleteAcademy, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _athleteAcademyRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<AthleteAcademy>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((AthleteAcademy aa, CancellationToken _) => aa);
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new RegisterAthleteCommand
        {
            AcademyId = academyId,
            AthleteId = athleteId
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.AthleteId.Should().Be(athleteId);
        _athleteAcademyRepositoryMock.Verify(r => r.AddAsync(It.IsAny<AthleteAcademy>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_AcademyNotFound_ReturnsFailure()
    {
        var academyId = Guid.NewGuid();
        var athleteId = Guid.NewGuid();

        _academyRepositoryMock
            .Setup(r => r.GetByIdAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Academy?)null);

        var command = new RegisterAthleteCommand
        {
            AcademyId = academyId,
            AthleteId = athleteId
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Academy not found.");
    }

    [Fact]
    public async Task Handle_AcademyNotVerified_ReturnsFailure()
    {
        var academyId = Guid.NewGuid();
        var athleteId = Guid.NewGuid();
        var academy = CreateTestAcademy(academyId);
        academy.VerificationStatus = VerificationStatus.Pending;

        _academyRepositoryMock
            .Setup(r => r.GetByIdAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(academy);

        var command = new RegisterAthleteCommand
        {
            AcademyId = academyId,
            AthleteId = athleteId
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("verified");
    }

    [Fact]
    public async Task Handle_AthleteNotFound_ReturnsFailure()
    {
        var academyId = Guid.NewGuid();
        var athleteId = Guid.NewGuid();
        var academy = CreateTestAcademy(academyId);
        academy.VerificationStatus = VerificationStatus.Verified;

        _academyRepositoryMock
            .Setup(r => r.GetByIdAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(academy);
        _athleteRepositoryMock
            .Setup(r => r.GetByIdAsync(athleteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Athlete?)null);

        var command = new RegisterAthleteCommand
        {
            AcademyId = academyId,
            AthleteId = athleteId
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Athlete not found.");
    }

    [Fact]
    public async Task Handle_AthleteAlreadyRegistered_ReturnsFailure()
    {
        var academyId = Guid.NewGuid();
        var athleteId = Guid.NewGuid();
        var academy = CreateTestAcademy(academyId);
        academy.VerificationStatus = VerificationStatus.Verified;
        var athlete = CreateTestAthlete(athleteId);

        _academyRepositoryMock
            .Setup(r => r.GetByIdAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(academy);
        _athleteRepositoryMock
            .Setup(r => r.GetByIdAsync(athleteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(athlete);
        _athleteAcademyRepositoryMock
            .Setup(r => r.AnyAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<AthleteAcademy, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var command = new RegisterAthleteCommand
        {
            AcademyId = academyId,
            AthleteId = athleteId
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("already registered");
    }

    private static Academy CreateTestAcademy(Guid? id = null) => new()
    {
        Id = id ?? Guid.NewGuid(),
        AcademyCode = "ACAD-20260725-TEST",
        Name = "Test Academy",
        Email = "test@test.com",
        Phone = "1234567890",
        Status = AcademyStatus.Active,
        VerificationStatus = VerificationStatus.Pending,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        Branches = new List<AcademyBranch>(),
        AcademySports = new List<AcademySport>(),
        Facilities = new List<AcademyFacility>(),
        Memberships = new List<AcademyMembership>(),
        Documents = new List<AcademyDocument>(),
        GalleryImages = new List<AcademyGallery>()
    };

    private static Athlete CreateTestAthlete(Guid? id = null) => new()
    {
        Id = id ?? Guid.NewGuid(),
        UserId = Guid.NewGuid(),
        AthleteCode = "ATH-20260725-TEST",
        RegistrationDate = DateTime.UtcNow,
        CurrentLevel = AthleteLevel.Intermediate,
        Status = AthleteStatus.Active,
        ExperienceYears = 3,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        User = new User
        {
            Id = Guid.NewGuid(),
            FullName = "Test Athlete",
            Email = "athlete@test.com",
            PhoneNumber = "1234567890",
            ProfileImageUrl = null
        }
    };
}
