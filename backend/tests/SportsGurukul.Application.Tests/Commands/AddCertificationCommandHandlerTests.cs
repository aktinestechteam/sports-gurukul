using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.CoachManagement.Commands.AddCertification;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Tests.Commands;

public class AddCertificationCommandHandlerTests
{
    private readonly Mock<ICoachRepository> _coachRepositoryMock = TestMocks.CreateCoachRepository();
    private readonly Mock<ICoachCertificationRepository> _coachCertificationRepositoryMock = TestMocks.CreateCoachCertificationRepository();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = TestMocks.CreateUnitOfWork();
    private readonly Mock<ILogger<AddCertificationCommandHandler>> _loggerMock = TestMocks.CreateLogger<AddCertificationCommandHandler>();
    private readonly Mock<ICurrentUser> _currentUserMock = new();
    private readonly Guid _testUserId = Guid.NewGuid();
    private readonly AddCertificationCommandHandler _handler;

    public AddCertificationCommandHandlerTests()
    {
        _currentUserMock.Setup(u => u.Roles).Returns(new List<string> { "Coach" });
        _currentUserMock.Setup(u => u.UserId).Returns(_testUserId);
        _handler = new AddCertificationCommandHandler(
            _coachRepositoryMock.Object,
            _coachCertificationRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object,
            _currentUserMock.Object);
    }

    [Fact]
    public async Task Handle_CoachNotFound_ReturnsFailure()
    {
        _coachRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Coach?)null);

        var result = await _handler.Handle(new AddCertificationCommand
        {
            CoachId = Guid.NewGuid(),
            CertificationName = "Test Cert"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Coach not found.");
    }

    [Fact]
    public async Task Handle_DuplicateCertification_ReturnsFailure()
    {
        var coachId = Guid.NewGuid();
        var coach = TestDataBuilder.CreateCoach(id: coachId, userId: _testUserId);
        var existingCerts = new List<CoachCertification>
        {
            TestDataBuilder.CreateCoachCertification(coachId)
        };
        existingCerts[0].CertificationName = "BCCI Level A";

        _coachRepositoryMock.Setup(r => r.GetByIdAsync(coachId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(coach);
        _coachCertificationRepositoryMock.Setup(r => r.GetByCoachIdAsync(coachId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCerts);

        var result = await _handler.Handle(new AddCertificationCommand
        {
            CoachId = coachId,
            CertificationName = "BCCI Level A"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("A certification with this name already exists for this coach.");
    }

    [Fact]
    public async Task Handle_NewCertification_AddsAndReturnsSuccess()
    {
        var coachId = Guid.NewGuid();
        var coach = TestDataBuilder.CreateCoach(id: coachId, userId: _testUserId);

        _coachRepositoryMock.Setup(r => r.GetByIdAsync(coachId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(coach);
        _coachCertificationRepositoryMock.Setup(r => r.GetByCoachIdAsync(coachId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CoachCertification>());
        _coachCertificationRepositoryMock.Setup(r => r.AddAsync(It.IsAny<CoachCertification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CoachCertification());
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(new AddCertificationCommand
        {
            CoachId = coachId,
            CertificationName = "NIS Diploma",
            IssuingAuthority = "NIS",
            CertificateNumber = "NIS-001",
            IssueDate = DateTime.UtcNow.AddYears(-1),
            ExpiryDate = DateTime.UtcNow.AddYears(4),
            CertificateUrl = "https://example.com/cert.pdf"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.CertificationName.Should().Be("NIS Diploma");
        result.Value.IssuingAuthority.Should().Be("NIS");
        result.Value.CertificateNumber.Should().Be("NIS-001");
        result.Value.VerificationStatus.Should().Be("Pending");
        _coachCertificationRepositoryMock.Verify(r => r.AddAsync(It.IsAny<CoachCertification>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
