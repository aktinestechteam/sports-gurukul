using FluentAssertions;
using MediatR;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.CoachManagement.Commands.DeleteCertification;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Tests.Commands;

public class DeleteCertificationCommandHandlerTests
{
    private readonly Mock<ICoachCertificationRepository> _coachCertificationRepositoryMock = TestMocks.CreateCoachCertificationRepository();
    private readonly Mock<ICoachRepository> _coachRepositoryMock = TestMocks.CreateCoachRepository();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = TestMocks.CreateUnitOfWork();
    private readonly Mock<ILogger<DeleteCertificationCommandHandler>> _loggerMock = TestMocks.CreateLogger<DeleteCertificationCommandHandler>();
    private readonly Mock<ICurrentUser> _currentUserMock = new();
    private readonly DeleteCertificationCommandHandler _handler;

    public DeleteCertificationCommandHandlerTests()
    {
        _currentUserMock.Setup(u => u.Roles).Returns(new List<string> { "Coach" });
        _handler = new DeleteCertificationCommandHandler(
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

        var result = await _handler.Handle(new DeleteCertificationCommand
        {
            CertificationId = Guid.NewGuid()
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

        var result = await _handler.Handle(new DeleteCertificationCommand
        {
            CertificationId = certificationId
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Certification not found.");
    }

    [Fact]
    public async Task Handle_ValidDelete_RemovesAndReturnsSuccess()
    {
        var certificationId = Guid.NewGuid();
        var certification = TestDataBuilder.CreateCoachCertification();
        certification.Id = certificationId;

        _coachCertificationRepositoryMock.Setup(r => r.GetByIdAsync(certificationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(certification);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(new DeleteCertificationCommand
        {
            CertificationId = certificationId
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(Unit.Value);
        _coachCertificationRepositoryMock.Verify(r => r.Remove(certification), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
