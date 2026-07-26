using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.AcademyManagement.Commands.AssignCoach;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.UnitTests.Features.AcademyManagement.Commands;

public class AssignCoachCommandHandlerTests
{
    private readonly Mock<IAcademyRepository> _academyRepositoryMock;
    private readonly Mock<ICoachRepository> _coachRepositoryMock;
    private readonly Mock<ICoachAcademyRepository> _coachAcademyRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<AssignCoachCommandHandler>> _loggerMock;
    private readonly AssignCoachCommandHandler _handler;

    public AssignCoachCommandHandlerTests()
    {
        _academyRepositoryMock = new Mock<IAcademyRepository>();
        _coachRepositoryMock = new Mock<ICoachRepository>();
        _coachAcademyRepositoryMock = new Mock<ICoachAcademyRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<AssignCoachCommandHandler>>();
        _handler = new AssignCoachCommandHandler(
            _academyRepositoryMock.Object,
            _coachRepositoryMock.Object,
            _coachAcademyRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_AssignsCoach()
    {
        var academyId = Guid.NewGuid();
        var coachId = Guid.NewGuid();
        var academy = CreateTestAcademy(academyId);
        academy.VerificationStatus = VerificationStatus.Verified;
        var coach = CreateTestCoach(coachId);

        _academyRepositoryMock
            .Setup(r => r.GetByIdAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(academy);
        _coachRepositoryMock
            .Setup(r => r.GetByIdAsync(coachId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(coach);
        _coachAcademyRepositoryMock
            .Setup(r => r.AnyAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<CoachAcademy, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _coachAcademyRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<CoachAcademy>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CoachAcademy ca, CancellationToken _) => ca);
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new AssignCoachCommand
        {
            AcademyId = academyId,
            CoachId = coachId
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.CoachId.Should().Be(coachId);
        _coachAcademyRepositoryMock.Verify(r => r.AddAsync(It.IsAny<CoachAcademy>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_AcademyNotFound_ReturnsFailure()
    {
        var academyId = Guid.NewGuid();
        var coachId = Guid.NewGuid();

        _academyRepositoryMock
            .Setup(r => r.GetByIdAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Academy?)null);

        var command = new AssignCoachCommand
        {
            AcademyId = academyId,
            CoachId = coachId
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Academy not found.");
    }

    [Fact]
    public async Task Handle_AcademyNotVerified_ReturnsFailure()
    {
        var academyId = Guid.NewGuid();
        var coachId = Guid.NewGuid();
        var academy = CreateTestAcademy(academyId);
        academy.VerificationStatus = VerificationStatus.Pending;

        _academyRepositoryMock
            .Setup(r => r.GetByIdAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(academy);

        var command = new AssignCoachCommand
        {
            AcademyId = academyId,
            CoachId = coachId
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("verified");
    }

    [Fact]
    public async Task Handle_CoachNotFound_ReturnsFailure()
    {
        var academyId = Guid.NewGuid();
        var coachId = Guid.NewGuid();
        var academy = CreateTestAcademy(academyId);
        academy.VerificationStatus = VerificationStatus.Verified;

        _academyRepositoryMock
            .Setup(r => r.GetByIdAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(academy);
        _coachRepositoryMock
            .Setup(r => r.GetByIdAsync(coachId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Coach?)null);

        var command = new AssignCoachCommand
        {
            AcademyId = academyId,
            CoachId = coachId
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Coach not found.");
    }

    [Fact]
    public async Task Handle_CoachAlreadyAssigned_ReturnsFailure()
    {
        var academyId = Guid.NewGuid();
        var coachId = Guid.NewGuid();
        var academy = CreateTestAcademy(academyId);
        academy.VerificationStatus = VerificationStatus.Verified;
        var coach = CreateTestCoach(coachId);

        _academyRepositoryMock
            .Setup(r => r.GetByIdAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(academy);
        _coachRepositoryMock
            .Setup(r => r.GetByIdAsync(coachId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(coach);
        _coachAcademyRepositoryMock
            .Setup(r => r.AnyAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<CoachAcademy, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var command = new AssignCoachCommand
        {
            AcademyId = academyId,
            CoachId = coachId
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("already assigned");
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
