using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.AcademyManagement.Commands.RegisterAthlete;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.UnitTests.Features.AcademyManagement.BusinessRules;

public class AthleteRegistrationTests
{
    private readonly Mock<IAcademyRepository> _academyRepoMock;
    private readonly Mock<IAthleteRepository> _athleteRepoMock;
    private readonly Mock<IAthleteAcademyRepository> _athleteAcademyRepoMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<RegisterAthleteCommandHandler>> _loggerMock;
    private readonly RegisterAthleteCommandHandler _handler;

    public AthleteRegistrationTests()
    {
        _academyRepoMock = new Mock<IAcademyRepository>();
        _athleteRepoMock = new Mock<IAthleteRepository>();
        _athleteAcademyRepoMock = new Mock<IAthleteAcademyRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<RegisterAthleteCommandHandler>>();
        _handler = new RegisterAthleteCommandHandler(
            _academyRepoMock.Object,
            _athleteRepoMock.Object,
            _athleteAcademyRepoMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task RegisterAthlete_Success()
    {
        var academyId = Guid.NewGuid();
        var athleteId = Guid.NewGuid();
        var academy = CreateTestAcademy(academyId);
        academy.VerificationStatus = VerificationStatus.Verified;
        var athlete = CreateTestAthlete(athleteId);

        _academyRepoMock
            .Setup(r => r.GetByIdAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(academy);
        _athleteRepoMock
            .Setup(r => r.GetByIdAsync(athleteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(athlete);
        _athleteAcademyRepoMock
            .Setup(r => r.AnyAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<AthleteAcademy, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _athleteAcademyRepoMock
            .Setup(r => r.AddAsync(It.IsAny<AthleteAcademy>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((AthleteAcademy aa, CancellationToken _) => aa);
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(new RegisterAthleteCommand
        {
            AcademyId = academyId,
            AthleteId = athleteId
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.AthleteId.Should().Be(athleteId);
        _athleteAcademyRepoMock.Verify(r => r.AddAsync(It.IsAny<AthleteAcademy>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RegisterAthlete_NonVerifiedAcademy_ReturnsFailure()
    {
        var academyId = Guid.NewGuid();
        var athleteId = Guid.NewGuid();
        var academy = CreateTestAcademy(academyId);
        academy.VerificationStatus = VerificationStatus.Pending;

        _academyRepoMock
            .Setup(r => r.GetByIdAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(academy);

        var result = await _handler.Handle(new RegisterAthleteCommand
        {
            AcademyId = academyId,
            AthleteId = athleteId
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("verified");
        _athleteAcademyRepoMock.Verify(r => r.AddAsync(It.IsAny<AthleteAcademy>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RegisterAthlete_InactiveAcademy_StillSucceedsIfVerified()
    {
        var academyId = Guid.NewGuid();
        var athleteId = Guid.NewGuid();
        var academy = CreateTestAcademy(academyId);
        academy.VerificationStatus = VerificationStatus.Verified;
        academy.Status = AcademyStatus.Inactive;
        var athlete = CreateTestAthlete(athleteId);

        _academyRepoMock
            .Setup(r => r.GetByIdAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(academy);
        _athleteRepoMock
            .Setup(r => r.GetByIdAsync(athleteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(athlete);
        _athleteAcademyRepoMock
            .Setup(r => r.AnyAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<AthleteAcademy, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _athleteAcademyRepoMock
            .Setup(r => r.AddAsync(It.IsAny<AthleteAcademy>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((AthleteAcademy aa, CancellationToken _) => aa);
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(new RegisterAthleteCommand
        {
            AcademyId = academyId,
            AthleteId = athleteId
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task RegisterAthlete_AlreadyRegistered_ReturnsFailure()
    {
        var academyId = Guid.NewGuid();
        var athleteId = Guid.NewGuid();
        var academy = CreateTestAcademy(academyId);
        academy.VerificationStatus = VerificationStatus.Verified;
        var athlete = CreateTestAthlete(athleteId);

        _academyRepoMock
            .Setup(r => r.GetByIdAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(academy);
        _athleteRepoMock
            .Setup(r => r.GetByIdAsync(athleteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(athlete);
        _athleteAcademyRepoMock
            .Setup(r => r.AnyAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<AthleteAcademy, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _handler.Handle(new RegisterAthleteCommand
        {
            AcademyId = academyId,
            AthleteId = athleteId
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("already registered");
        _athleteAcademyRepoMock.Verify(r => r.AddAsync(It.IsAny<AthleteAcademy>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RegisterAthlete_AcademyNotFound_ReturnsFailure()
    {
        var academyId = Guid.NewGuid();
        var athleteId = Guid.NewGuid();

        _academyRepoMock
            .Setup(r => r.GetByIdAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Academy?)null);

        var result = await _handler.Handle(new RegisterAthleteCommand
        {
            AcademyId = academyId,
            AthleteId = athleteId
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Academy not found.");
        _athleteAcademyRepoMock.Verify(r => r.AddAsync(It.IsAny<AthleteAcademy>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RegisterAthlete_AthleteNotFound_ReturnsFailure()
    {
        var academyId = Guid.NewGuid();
        var athleteId = Guid.NewGuid();
        var academy = CreateTestAcademy(academyId);
        academy.VerificationStatus = VerificationStatus.Verified;

        _academyRepoMock
            .Setup(r => r.GetByIdAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(academy);
        _athleteRepoMock
            .Setup(r => r.GetByIdAsync(athleteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Athlete?)null);

        var result = await _handler.Handle(new RegisterAthleteCommand
        {
            AcademyId = academyId,
            AthleteId = athleteId
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Athlete not found.");
        _athleteAcademyRepoMock.Verify(r => r.AddAsync(It.IsAny<AthleteAcademy>(), It.IsAny<CancellationToken>()), Times.Never);
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
