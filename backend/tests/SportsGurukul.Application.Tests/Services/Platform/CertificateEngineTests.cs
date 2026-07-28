using FluentAssertions;
using Moq;
using SportsGurukul.Application.Features.RegistrationAttendancePlatform.Engines;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Tests.Services.Platform;

public class CertificateEngineTests
{
    private readonly Mock<ILogger<CertificateEngine>> _loggerMock = new();
    private readonly CertificateEngine _engine;

    public CertificateEngineTests()
    {
        _engine = new CertificateEngine(_loggerMock.Object);
    }

    [Fact]
    public async Task GenerateCertificateNumberAsync_EventType_ReturnsEVT_Prefix()
    {
        var result = await _engine.GenerateCertificateNumberAsync(ProgramType.Event, CancellationToken.None);

        result.Should().StartWith("EVT-CERT-");
    }

    [Fact]
    public async Task GenerateCertificateNumberAsync_TrainingType_ReturnsTRN_Prefix()
    {
        var result = await _engine.GenerateCertificateNumberAsync(ProgramType.Training, CancellationToken.None);

        result.Should().StartWith("TRN-CERT-");
    }

    [Fact]
    public async Task IsEligibleForCertificateAsync_CompletedWithHighAttendance_ReturnsTrue()
    {
        var result = await _engine.IsEligibleForCertificateAsync(85.0, true, 75.0);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsEligibleForCertificateAsync_NotCompleted_ReturnsFalse()
    {
        var result = await _engine.IsEligibleForCertificateAsync(90.0, false, 75.0);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsEligibleForCertificateAsync_LowAttendance_ReturnsFalse()
    {
        var result = await _engine.IsEligibleForCertificateAsync(50.0, true, 75.0);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task DetermineCertificateTypeAsync_HighScore_ReturnsExcellence()
    {
        var result = await _engine.DetermineCertificateTypeAsync(95.0, 92.0, CancellationToken.None);

        result.Should().Be(CertificateType.Excellence);
    }

    [Fact]
    public async Task DetermineCertificateTypeAsync_MediumScore_ReturnsMerit()
    {
        var result = await _engine.DetermineCertificateTypeAsync(85.0, 78.0, CancellationToken.None);

        result.Should().Be(CertificateType.Merit);
    }

    [Fact]
    public async Task DetermineCertificateTypeAsync_HighAttendanceNoScore_ReturnsCompletion()
    {
        var result = await _engine.DetermineCertificateTypeAsync(95.0, null, CancellationToken.None);

        result.Should().Be(CertificateType.Completion);
    }

    [Fact]
    public async Task DetermineCertificateTypeAsync_LowAttendanceNoScore_ReturnsParticipation()
    {
        var result = await _engine.DetermineCertificateTypeAsync(76.0, null, CancellationToken.None);

        result.Should().Be(CertificateType.Participation);
    }

    [Fact]
    public async Task SelectTemplateAsync_EventParticipation_ReturnsExpectedTemplate()
    {
        var result = await _engine.SelectTemplateAsync(ProgramType.Event, CertificateType.Participation, CancellationToken.None);

        result.Should().Be("template-event-participation");
    }
}
