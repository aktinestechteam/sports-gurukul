using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.AcademyManagement.Commands.AssignCoach;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.UnitTests.Features.AcademyManagement.BusinessRules;

public class CoachAssignmentTests
{
    private readonly Mock<IAcademyRepository> _academyRepoMock;
    private readonly Mock<ICoachRepository> _coachRepoMock;
    private readonly Mock<ICoachAcademyRepository> _coachAcademyRepoMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<AssignCoachCommandHandler>> _loggerMock;
    private readonly AssignCoachCommandHandler _handler;

    public CoachAssignmentTests()
    {
        _academyRepoMock = new Mock<IAcademyRepository>();
        _coachRepoMock = new Mock<ICoachRepository>();
        _coachAcademyRepoMock = new Mock<ICoachAcademyRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<AssignCoachCommandHandler>>();
        _handler = new AssignCoachCommandHandler(
            _academyRepoMock.Object,
            _coachRepoMock.Object,
            _coachAcademyRepoMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task AssignCoach_Success()
    {
        var academyId = Guid.NewGuid();
        var coachId = Guid.NewGuid();
        var academy = CreateTestAcademy(academyId);
        academy.VerificationStatus = VerificationStatus.Verified;
        var coach = CreateTestCoach(coachId);

        _academyRepoMock
            .Setup(r => r.GetByIdAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(academy);
        _coachRepoMock
            .Setup(r => r.GetByIdAsync(coachId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(coach);
        _coachAcademyRepoMock
            .Setup(r => r.AnyAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<CoachAcademy, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _coachAcademyRepoMock
            .Setup(r => r.AddAsync(It.IsAny<CoachAcademy>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CoachAcademy ca, CancellationToken _) => ca);
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(new AssignCoachCommand
        {
            AcademyId = academyId,
            CoachId = coachId
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.CoachId.Should().Be(coachId);
        _coachAcademyRepoMock.Verify(r => r.AddAsync(It.IsAny<CoachAcademy>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AssignCoach_NonVerifiedAcademy_ReturnsFailure()
    {
        var academyId = Guid.NewGuid();
        var coachId = Guid.NewGuid();
        var academy = CreateTestAcademy(academyId);
        academy.VerificationStatus = VerificationStatus.Pending;

        _academyRepoMock
            .Setup(r => r.GetByIdAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(academy);

        var result = await _handler.Handle(new AssignCoachCommand
        {
            AcademyId = academyId,
            CoachId = coachId
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("verified");
        _coachAcademyRepoMock.Verify(r => r.AddAsync(It.IsAny<CoachAcademy>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task AssignCoach_InactiveAcademy_StillSucceedsIfVerified()
    {
        var academyId = Guid.NewGuid();
        var coachId = Guid.NewGuid();
        var academy = CreateTestAcademy(academyId);
        academy.VerificationStatus = VerificationStatus.Verified;
        academy.Status = AcademyStatus.Inactive;
        var coach = CreateTestCoach(coachId);

        _academyRepoMock
            .Setup(r => r.GetByIdAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(academy);
        _coachRepoMock
            .Setup(r => r.GetByIdAsync(coachId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(coach);
        _coachAcademyRepoMock
            .Setup(r => r.AnyAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<CoachAcademy, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _coachAcademyRepoMock
            .Setup(r => r.AddAsync(It.IsAny<CoachAcademy>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CoachAcademy ca, CancellationToken _) => ca);
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(new AssignCoachCommand
        {
            AcademyId = academyId,
            CoachId = coachId
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task AssignCoach_AlreadyAssigned_ReturnsFailure()
    {
        var academyId = Guid.NewGuid();
        var coachId = Guid.NewGuid();
        var academy = CreateTestAcademy(academyId);
        academy.VerificationStatus = VerificationStatus.Verified;
        var coach = CreateTestCoach(coachId);

        _academyRepoMock
            .Setup(r => r.GetByIdAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(academy);
        _coachRepoMock
            .Setup(r => r.GetByIdAsync(coachId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(coach);
        _coachAcademyRepoMock
            .Setup(r => r.AnyAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<CoachAcademy, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _handler.Handle(new AssignCoachCommand
        {
            AcademyId = academyId,
            CoachId = coachId
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("already assigned");
        _coachAcademyRepoMock.Verify(r => r.AddAsync(It.IsAny<CoachAcademy>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task AssignCoach_AcademyNotFound_ReturnsFailure()
    {
        var academyId = Guid.NewGuid();
        var coachId = Guid.NewGuid();

        _academyRepoMock
            .Setup(r => r.GetByIdAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Academy?)null);

        var result = await _handler.Handle(new AssignCoachCommand
        {
            AcademyId = academyId,
            CoachId = coachId
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Academy not found.");
        _coachAcademyRepoMock.Verify(r => r.AddAsync(It.IsAny<CoachAcademy>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task AssignCoach_CoachNotFound_ReturnsFailure()
    {
        var academyId = Guid.NewGuid();
        var coachId = Guid.NewGuid();
        var academy = CreateTestAcademy(academyId);
        academy.VerificationStatus = VerificationStatus.Verified;

        _academyRepoMock
            .Setup(r => r.GetByIdAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(academy);
        _coachRepoMock
            .Setup(r => r.GetByIdAsync(coachId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Coach?)null);

        var result = await _handler.Handle(new AssignCoachCommand
        {
            AcademyId = academyId,
            CoachId = coachId
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Coach not found.");
        _coachAcademyRepoMock.Verify(r => r.AddAsync(It.IsAny<CoachAcademy>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task AssignCoach_InactiveCoach_ReturnsFailure()
    {
        var academyId = Guid.NewGuid();
        var coachId = Guid.NewGuid();
        var academy = CreateTestAcademy(academyId);
        academy.VerificationStatus = VerificationStatus.Verified;
        var coach = CreateTestCoach(coachId);
        coach.Status = CoachStatus.Inactive;

        _academyRepoMock
            .Setup(r => r.GetByIdAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(academy);
        _coachRepoMock
            .Setup(r => r.GetByIdAsync(coachId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(coach);

        var result = await _handler.Handle(new AssignCoachCommand
        {
            AcademyId = academyId,
            CoachId = coachId
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("active");
        _coachAcademyRepoMock.Verify(r => r.AddAsync(It.IsAny<CoachAcademy>(), It.IsAny<CancellationToken>()), Times.Never);
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

    private static Coach CreateTestCoach(Guid? id = null) => new()
    {
        Id = id ?? Guid.NewGuid(),
        UserId = Guid.NewGuid(),
        CoachCode = "COACH-20260725-TEST",
        RegistrationDate = DateTime.UtcNow,
        CoachingLevel = CoachingLevel.Senior,
        Status = CoachStatus.Active,
        VerificationStatus = VerificationStatus.Verified,
        YearsOfExperience = 5,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        User = new User
        {
            Id = Guid.NewGuid(),
            FullName = "Test Coach",
            Email = "coach@test.com",
            PhoneNumber = "9876543210",
            ProfileImageUrl = null
        }
    };
}
