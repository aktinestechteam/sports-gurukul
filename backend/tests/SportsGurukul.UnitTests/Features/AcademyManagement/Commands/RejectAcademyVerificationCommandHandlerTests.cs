using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.AcademyManagement.Commands.RejectAcademyVerification;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.UnitTests.Features.AcademyManagement.Commands;

public class RejectAcademyVerificationCommandHandlerTests
{
    private readonly Mock<IAcademyRepository> _academyRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ICurrentUser> _currentUserMock;
    private readonly Mock<ILogger<RejectAcademyVerificationCommandHandler>> _loggerMock;
    private readonly RejectAcademyVerificationCommandHandler _handler;

    public RejectAcademyVerificationCommandHandlerTests()
    {
        _academyRepositoryMock = new Mock<IAcademyRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _currentUserMock = new Mock<ICurrentUser>();
        _loggerMock = new Mock<ILogger<RejectAcademyVerificationCommandHandler>>();
        _handler = new RejectAcademyVerificationCommandHandler(
            _academyRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _currentUserMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_RejectsAcademy()
    {
        var academyId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var academy = CreateTestAcademyWithDetails(academyId);
        academy.Verification!.VerificationStatus = VerificationStatus.Pending;

        _currentUserMock
            .Setup(c => c.UserId)
            .Returns(userId);
        _academyRepositoryMock
            .Setup(r => r.GetByIdWithDetailsAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(academy);
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new RejectAcademyVerificationCommand
        {
            AcademyId = academyId,
            Remarks = "Documents are not authentic"
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        academy.Verification.VerificationStatus.Should().Be(VerificationStatus.Rejected);
        academy.Verification.VerifiedBy.Should().Be(userId);
        academy.Verification.Remarks.Should().Be("Documents are not authentic");
        academy.VerificationStatus.Should().Be(VerificationStatus.Rejected);
        _academyRepositoryMock.Verify(r => r.Update(It.IsAny<Academy>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_AcademyNotFound_ReturnsFailure()
    {
        var academyId = Guid.NewGuid();

        _academyRepositoryMock
            .Setup(r => r.GetByIdWithDetailsAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Academy?)null);

        var command = new RejectAcademyVerificationCommand
        {
            AcademyId = academyId,
            Remarks = "Invalid"
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Academy not found.");
    }

    [Fact]
    public async Task Handle_VerificationRecordNotFound_ReturnsFailure()
    {
        var academyId = Guid.NewGuid();
        var academy = CreateTestAcademyWithDetails(academyId);
        academy.Verification = null;

        _academyRepositoryMock
            .Setup(r => r.GetByIdWithDetailsAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(academy);

        var command = new RejectAcademyVerificationCommand
        {
            AcademyId = academyId,
            Remarks = "Invalid"
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Academy verification record not found.");
    }

    [Fact]
    public async Task Handle_AlreadyRejected_ReturnsFailure()
    {
        var academyId = Guid.NewGuid();
        var academy = CreateTestAcademyWithDetails(academyId);
        academy.Verification!.VerificationStatus = VerificationStatus.Rejected;

        _academyRepositoryMock
            .Setup(r => r.GetByIdWithDetailsAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(academy);

        var command = new RejectAcademyVerificationCommand
        {
            AcademyId = academyId,
            Remarks = "Already rejected"
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Academy verification is already rejected.");
    }

    private static Academy CreateTestAcademyWithDetails(Guid? id = null) => new()
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
        Verification = new AcademyVerification
        {
            Id = Guid.NewGuid(),
            VerificationStatus = VerificationStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        },
        Branches = new List<AcademyBranch>(),
        AcademySports = new List<AcademySport>(),
        Facilities = new List<AcademyFacility>(),
        Memberships = new List<AcademyMembership>(),
        Documents = new List<AcademyDocument>(),
        GalleryImages = new List<AcademyGallery>()
    };
}
