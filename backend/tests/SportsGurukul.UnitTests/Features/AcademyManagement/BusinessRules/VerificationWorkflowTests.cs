using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.AcademyManagement.Commands.AssignCoach;
using SportsGurukul.Application.Features.AcademyManagement.Commands.RegisterAthlete;
using SportsGurukul.Application.Features.AcademyManagement.Commands.RejectAcademyVerification;
using SportsGurukul.Application.Features.AcademyManagement.Commands.VerifyAcademy;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.UnitTests.Features.AcademyManagement.BusinessRules;

public class VerificationWorkflowTests
{
    private readonly Mock<IAcademyRepository> _academyRepositoryMock;
    private readonly Mock<ICoachRepository> _coachRepositoryMock;
    private readonly Mock<IAthleteRepository> _athleteRepositoryMock;
    private readonly Mock<ICoachAcademyRepository> _coachAcademyRepoMock;
    private readonly Mock<IAthleteAcademyRepository> _athleteAcademyRepoMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ICurrentUser> _currentUserMock;
    private readonly Mock<ILogger<VerifyAcademyCommandHandler>> _verifyLoggerMock;
    private readonly Mock<ILogger<RejectAcademyVerificationCommandHandler>> _rejectLoggerMock;
    private readonly Mock<ILogger<AssignCoachCommandHandler>> _assignCoachLoggerMock;
    private readonly Mock<ILogger<RegisterAthleteCommandHandler>> _registerAthleteLoggerMock;
    private readonly VerifyAcademyCommandHandler _verifyHandler;
    private readonly RejectAcademyVerificationCommandHandler _rejectHandler;
    private readonly AssignCoachCommandHandler _assignCoachHandler;
    private readonly RegisterAthleteCommandHandler _registerAthleteHandler;

    public VerificationWorkflowTests()
    {
        _academyRepositoryMock = new Mock<IAcademyRepository>();
        _coachRepositoryMock = new Mock<ICoachRepository>();
        _athleteRepositoryMock = new Mock<IAthleteRepository>();
        _coachAcademyRepoMock = new Mock<ICoachAcademyRepository>();
        _athleteAcademyRepoMock = new Mock<IAthleteAcademyRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _currentUserMock = new Mock<ICurrentUser>();
        _verifyLoggerMock = new Mock<ILogger<VerifyAcademyCommandHandler>>();
        _rejectLoggerMock = new Mock<ILogger<RejectAcademyVerificationCommandHandler>>();
        _assignCoachLoggerMock = new Mock<ILogger<AssignCoachCommandHandler>>();
        _registerAthleteLoggerMock = new Mock<ILogger<RegisterAthleteCommandHandler>>();

        _currentUserMock.Setup(c => c.UserId).Returns(Guid.NewGuid());

        _verifyHandler = new VerifyAcademyCommandHandler(
            _academyRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _currentUserMock.Object,
            _verifyLoggerMock.Object);

        _rejectHandler = new RejectAcademyVerificationCommandHandler(
            _academyRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _currentUserMock.Object,
            _rejectLoggerMock.Object);

        _assignCoachHandler = new AssignCoachCommandHandler(
            _academyRepositoryMock.Object,
            _coachRepositoryMock.Object,
            _coachAcademyRepoMock.Object,
            _unitOfWorkMock.Object,
            _assignCoachLoggerMock.Object);

        _registerAthleteHandler = new RegisterAthleteCommandHandler(
            _academyRepositoryMock.Object,
            _athleteRepositoryMock.Object,
            _athleteAcademyRepoMock.Object,
            _unitOfWorkMock.Object,
            _registerAthleteLoggerMock.Object);
    }

    [Fact]
    public void NewAcademy_HasPendingVerificationStatus()
    {
        var academy = new Academy();

        academy.VerificationStatus.Should().Be(VerificationStatus.Pending);
        academy.Status.Should().Be(AcademyStatus.Pending);
    }

    [Fact]
    public async Task VerifyAcademy_SetsStatusToVerified()
    {
        var academyId = Guid.NewGuid();
        var academy = CreateTestAcademy(academyId);

        _academyRepositoryMock
            .Setup(r => r.GetByIdWithDetailsAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(academy);
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _verifyHandler.Handle(new VerifyAcademyCommand { AcademyId = academyId }, CancellationToken.None);

        academy.VerificationStatus.Should().Be(VerificationStatus.Verified);
    }

    [Fact]
    public async Task VerifyAcademy_SetsVerifiedByUserId()
    {
        var academyId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var academy = CreateTestAcademy(academyId);
        _currentUserMock.Setup(c => c.UserId).Returns(userId);

        _academyRepositoryMock
            .Setup(r => r.GetByIdWithDetailsAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(academy);
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _verifyHandler.Handle(new VerifyAcademyCommand { AcademyId = academyId }, CancellationToken.None);

        academy.Verification!.VerifiedBy.Should().Be(userId);
    }

    [Fact]
    public async Task VerifyAcademy_SetsVerifiedAtTimestamp()
    {
        var academyId = Guid.NewGuid();
        var beforeVerify = DateTime.UtcNow;
        var academy = CreateTestAcademy(academyId);

        _academyRepositoryMock
            .Setup(r => r.GetByIdWithDetailsAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(academy);
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _verifyHandler.Handle(new VerifyAcademyCommand { AcademyId = academyId }, CancellationToken.None);

        var afterVerify = DateTime.UtcNow;
        academy.Verification!.VerifiedOn.Should().NotBeNull();
        academy.Verification.VerifiedOn!.Value.Should().BeOnOrAfter(beforeVerify).And.BeOnOrBefore(afterVerify);
    }

    [Fact]
    public async Task RejectAcademy_SetsStatusToRejected()
    {
        var academyId = Guid.NewGuid();
        var academy = CreateTestAcademy(academyId);

        _academyRepositoryMock
            .Setup(r => r.GetByIdWithDetailsAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(academy);
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _rejectHandler.Handle(new RejectAcademyVerificationCommand
        {
            AcademyId = academyId,
            Remarks = "Insufficient documentation"
        }, CancellationToken.None);

        academy.VerificationStatus.Should().Be(VerificationStatus.Rejected);
    }

    [Fact]
    public async Task RejectAcademy_SetsRemarks()
    {
        var academyId = Guid.NewGuid();
        var academy = CreateTestAcademy(academyId);
        var remarks = "Missing insurance documents";

        _academyRepositoryMock
            .Setup(r => r.GetByIdWithDetailsAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(academy);
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _rejectHandler.Handle(new RejectAcademyVerificationCommand
        {
            AcademyId = academyId,
            Remarks = remarks
        }, CancellationToken.None);

        academy.Verification!.Remarks.Should().Be(remarks);
    }

    [Fact]
    public async Task RejectAcademy_SetsRejectedByUserId()
    {
        var academyId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var academy = CreateTestAcademy(academyId);
        _currentUserMock.Setup(c => c.UserId).Returns(userId);

        _academyRepositoryMock
            .Setup(r => r.GetByIdWithDetailsAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(academy);
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _rejectHandler.Handle(new RejectAcademyVerificationCommand
        {
            AcademyId = academyId,
            Remarks = "Incomplete profile"
        }, CancellationToken.None);

        academy.Verification!.VerifiedBy.Should().Be(userId);
    }

    [Fact]
    public async Task Verify_AfterReject_UpdatesStatusToVerified()
    {
        var academyId = Guid.NewGuid();
        var academy = CreateTestAcademy(academyId);
        academy.VerificationStatus = VerificationStatus.Rejected;
        academy.Verification!.VerificationStatus = VerificationStatus.Rejected;
        academy.Verification.Remarks = "Previous rejection";

        _academyRepositoryMock
            .Setup(r => r.GetByIdWithDetailsAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(academy);
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _verifyHandler.Handle(new VerifyAcademyCommand { AcademyId = academyId }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        academy.VerificationStatus.Should().Be(VerificationStatus.Verified);
        academy.Verification.VerificationStatus.Should().Be(VerificationStatus.Verified);
    }

    [Fact]
    public async Task AssignCoach_RequiresVerifiedAcademy()
    {
        var academyId = Guid.NewGuid();
        var coachId = Guid.NewGuid();
        var academy = CreateTestAcademy(academyId);
        academy.VerificationStatus = VerificationStatus.Pending;

        _academyRepositoryMock
            .Setup(r => r.GetByIdAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(academy);

        var result = await _assignCoachHandler.Handle(new AssignCoachCommand
        {
            AcademyId = academyId,
            CoachId = coachId
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("verified");
    }

    [Fact]
    public async Task RegisterAthlete_RequiresVerifiedAcademy()
    {
        var academyId = Guid.NewGuid();
        var athleteId = Guid.NewGuid();
        var academy = CreateTestAcademy(academyId);
        academy.VerificationStatus = VerificationStatus.Pending;

        _academyRepositoryMock
            .Setup(r => r.GetByIdAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(academy);

        var result = await _registerAthleteHandler.Handle(new RegisterAthleteCommand
        {
            AcademyId = academyId,
            AthleteId = athleteId
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("verified");
    }

    private static Academy CreateTestAcademy(Guid? id = null) => new()
    {
        Id = id ?? Guid.NewGuid(),
        AcademyCode = "ACAD-20260725-TEST",
        Name = "Test Academy",
        Email = "test@test.com",
        Phone = "1234567890",
        Status = AcademyStatus.Pending,
        VerificationStatus = VerificationStatus.Pending,
        Verification = new AcademyVerification
        {
            Id = Guid.NewGuid(),
            VerificationStatus = VerificationStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        },
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        Branches = new List<AcademyBranch>(),
        AcademySports = new List<AcademySport>(),
        Facilities = new List<AcademyFacility>(),
        Memberships = new List<AcademyMembership>(),
        Documents = new List<AcademyDocument>(),
        GalleryImages = new List<AcademyGallery>()
    };
}
