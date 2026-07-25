using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.CoachManagement.Commands.VerifyCertification;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Tests.Commands;

public class VerifyCertificationCommandHandlerTests
{
    private readonly Mock<ICoachCertificationRepository> _coachCertificationRepositoryMock = TestMocks.CreateCoachCertificationRepository();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = TestMocks.CreateUnitOfWork();
    private readonly Mock<ILogger<VerifyCertificationCommandHandler>> _loggerMock = TestMocks.CreateLogger<VerifyCertificationCommandHandler>();
    private readonly VerifyCertificationCommandHandler _handler;

    public VerifyCertificationCommandHandlerTests()
    {
        _handler = new VerifyCertificationCommandHandler(
            _coachCertificationRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_CertificationNotFound_ReturnsFailure()
    {
        _coachCertificationRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CoachCertification?)null);

        var result = await _handler.Handle(new VerifyCertificationCommand
        {
            CertificationId = Guid.NewGuid(),
            Status = VerificationStatus.Verified
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

        var result = await _handler.Handle(new VerifyCertificationCommand
        {
            CertificationId = certificationId,
            Status = VerificationStatus.Verified
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Certification not found.");
    }

    [Fact]
    public async Task Handle_ValidVerification_SetsStatusAndReturnsSuccess()
    {
        var certificationId = Guid.NewGuid();
        var certification = TestDataBuilder.CreateCoachCertification();
        certification.Id = certificationId;
        certification.VerificationStatus = VerificationStatus.Pending;

        _coachCertificationRepositoryMock.Setup(r => r.GetByIdAsync(certificationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(certification);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(new VerifyCertificationCommand
        {
            CertificationId = certificationId,
            Status = VerificationStatus.Verified
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.VerificationStatus.Should().Be("Verified");
        certification.VerificationStatus.Should().Be(VerificationStatus.Verified);
        _coachCertificationRepositoryMock.Verify(r => r.Update(certification), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
