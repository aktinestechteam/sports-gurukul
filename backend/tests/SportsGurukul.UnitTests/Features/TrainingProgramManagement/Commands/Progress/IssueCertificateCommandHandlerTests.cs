using System.Text.RegularExpressions;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Progress.IssueCertificate;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.UnitTests.Features.TrainingProgramManagement.Commands.Progress;

public class IssueCertificateCommandHandlerTests
{
    private readonly Mock<ITrainingBatchRepository> _batchRepositoryMock;
    private readonly Mock<ILogger<IssueCertificateCommandHandler>> _loggerMock;
    private readonly IssueCertificateCommandHandler _handler;

    public IssueCertificateCommandHandlerTests()
    {
        _batchRepositoryMock = new Mock<ITrainingBatchRepository>();
        _loggerMock = new Mock<ILogger<IssueCertificateCommandHandler>>();
        _handler = new IssueCertificateCommandHandler(
            _batchRepositoryMock.Object,
            _loggerMock.Object);
    }

    private static IssueCertificateCommand CreateValidCommand(
        Guid? enrollmentId = null,
        string certificateType = "Completion") => new()
    {
        EnrollmentId = enrollmentId ?? Guid.NewGuid(),
        CertificateType = certificateType,
        FileUrl = "https://example.com/cert.pdf"
    };

    private void SetupBatchWithEnrollment(Guid batchId, Guid enrollmentId, EnrollmentStatus status)
    {
        var enrollment = TestHelpers.CreateTestEnrollment(id: enrollmentId, batchId: batchId, status: status);
        enrollment.Certificates = new List<TrainingCertificate>();
        var batch = TestHelpers.CreateTestBatch(id: batchId);
        batch.Enrollments = new List<TrainingEnrollment> { enrollment };

        _batchRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TrainingBatch> { batch });
        _batchRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(batchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(batch);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_ValidCertificateIssuance()
    {
        var enrollmentId = Guid.NewGuid();
        var batchId = Guid.NewGuid();
        var command = CreateValidCommand(enrollmentId);

        SetupBatchWithEnrollment(batchId, enrollmentId, EnrollmentStatus.Completed);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.EnrollmentId.Should().Be(enrollmentId);
        result.Value.CertificateType.Should().Be("Completion");
        result.Value.FileUrl.Should().Be("https://example.com/cert.pdf");
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_InvalidCertificateType()
    {
        var enrollmentId = Guid.NewGuid();
        var command = CreateValidCommand(enrollmentId, certificateType: "Invalid");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Invalid certificate type");
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_EnrollmentNotFound()
    {
        var enrollmentId = Guid.NewGuid();
        var command = CreateValidCommand(enrollmentId);

        _batchRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TrainingBatch>());

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Enrollment not found");
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_EnrollmentNotCompleted()
    {
        var enrollmentId = Guid.NewGuid();
        var batchId = Guid.NewGuid();
        var command = CreateValidCommand(enrollmentId);

        SetupBatchWithEnrollment(batchId, enrollmentId, EnrollmentStatus.Active);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Certificate can only be issued for completed enrollments");
    }

    [Fact]
    public async Task Handle_Should_GenerateCertificateNumber_When_Issuing()
    {
        var enrollmentId = Guid.NewGuid();
        var batchId = Guid.NewGuid();
        var command = CreateValidCommand(enrollmentId);

        SetupBatchWithEnrollment(batchId, enrollmentId, EnrollmentStatus.Completed);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        var certNumber = result.Value.CertificateNumber;
        var pattern = @"^CERT-\d{8}-\d{6}$";
        Regex.IsMatch(certNumber, pattern).Should().BeTrue($"Certificate number '{certNumber}' should match pattern CERT-{{yyyyMMdd}}-{{6digits}}");
    }
}
