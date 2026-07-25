using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.CoachManagement.Commands.UpdateCertification;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Tests.Commands;

public class UpdateCertificationCommandHandlerTests
{
    private readonly Mock<ICoachCertificationRepository> _coachCertificationRepositoryMock = TestMocks.CreateCoachCertificationRepository();
    private readonly Mock<ICoachRepository> _coachRepositoryMock = TestMocks.CreateCoachRepository();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = TestMocks.CreateUnitOfWork();
    private readonly Mock<ILogger<UpdateCertificationCommandHandler>> _loggerMock = TestMocks.CreateLogger<UpdateCertificationCommandHandler>();
    private readonly Mock<ICurrentUser> _currentUserMock = new();
    private readonly UpdateCertificationCommandHandler _handler;

    public UpdateCertificationCommandHandlerTests()
    {
        _currentUserMock.Setup(u => u.Roles).Returns(new List<string> { "Coach" });
        _handler = new UpdateCertificationCommandHandler(
            _coachCertificationRepositoryMock.Object,
            _coachRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object,
            _currentUserMock.Object);
    }

    [Fact]
    public async Task Handle_CertificationNotFound_ReturnsFailure()
    {
        _coachCertificationRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CoachCertification?)null);

        var result = await _handler.Handle(new UpdateCertificationCommand
        {
            CertificationId = Guid.NewGuid(),
            CertificationName = "Updated Cert"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Certification not found.");
    }

    [Fact]
    public async Task Handle_CertificationIsDeleted_ReturnsFailure()
    {
        var certificationId = Guid.NewGuid();
        var certification = TestDataBuilder.CreateCoachCertification();
        certification.Id = certificationId;
        certification.IsDeleted = true;

        _coachCertificationRepositoryMock.Setup(r => r.GetByIdAsync(certificationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(certification);

        var result = await _handler.Handle(new UpdateCertificationCommand
        {
            CertificationId = certificationId,
            CertificationName = "Updated Cert"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Certification not found.");
    }

    [Fact]
    public async Task Handle_ValidUpdateWithAllFields_UpdatesAndReturnsSuccess()
    {
        var certificationId = Guid.NewGuid();
        var certification = TestDataBuilder.CreateCoachCertification();
        certification.Id = certificationId;
        certification.CertificationName = "Old Cert";
        certification.IssuingAuthority = "Old Authority";

        _coachCertificationRepositoryMock.Setup(r => r.GetByIdAsync(certificationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(certification);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var newIssueDate = DateTime.UtcNow.AddYears(-2);
        var newExpiryDate = DateTime.UtcNow.AddYears(3);
        var result = await _handler.Handle(new UpdateCertificationCommand
        {
            CertificationId = certificationId,
            CertificationName = "New Cert",
            IssuingAuthority = "New Authority",
            CertificateNumber = "CERT-002",
            IssueDate = newIssueDate,
            ExpiryDate = newExpiryDate,
            CertificateUrl = "https://example.com/new-cert.pdf"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.CertificationName.Should().Be("New Cert");
        result.Value.IssuingAuthority.Should().Be("New Authority");
        result.Value.CertificateNumber.Should().Be("CERT-002");
        result.Value.IssueDate.Should().Be(newIssueDate);
        result.Value.ExpiryDate.Should().Be(newExpiryDate);
        result.Value.CertificateUrl.Should().Be("https://example.com/new-cert.pdf");
        _coachCertificationRepositoryMock.Verify(r => r.Update(certification), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NullFieldsDoNotOverwriteExistingValues_ReturnsSuccess()
    {
        var certificationId = Guid.NewGuid();
        var certification = TestDataBuilder.CreateCoachCertification();
        certification.Id = certificationId;
        certification.CertificationName = "Original Cert";
        certification.IssuingAuthority = "Original Authority";
        certification.CertificateNumber = "CERT-001";

        _coachCertificationRepositoryMock.Setup(r => r.GetByIdAsync(certificationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(certification);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(new UpdateCertificationCommand
        {
            CertificationId = certificationId,
            CertificationName = null,
            IssuingAuthority = null,
            CertificateNumber = null,
            IssueDate = null,
            ExpiryDate = null,
            CertificateUrl = null
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.CertificationName.Should().Be("Original Cert");
        result.Value.IssuingAuthority.Should().Be("Original Authority");
        result.Value.CertificateNumber.Should().Be("CERT-001");
    }
}
