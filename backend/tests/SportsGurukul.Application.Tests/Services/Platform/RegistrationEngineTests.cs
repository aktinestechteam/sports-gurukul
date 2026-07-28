using FluentAssertions;
using Moq;
using SportsGurukul.Application.Features.RegistrationAttendancePlatform.Engines;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Tests.Services.Platform;

public class RegistrationEngineTests
{
    private readonly Mock<ILogger<RegistrationEngine>> _loggerMock = new();
    private readonly RegistrationEngine _engine;

    public RegistrationEngineTests()
    {
        _engine = new RegistrationEngine(_loggerMock.Object);
    }

    [Fact]
    public async Task GenerateRegistrationNumberAsync_EventType_ReturnsEVT_Prefix()
    {
        var result = await _engine.GenerateRegistrationNumberAsync(ProgramType.Event, CancellationToken.None);

        result.Should().StartWith("EVT-REG-");
        result.Should().Contain(DateTime.UtcNow.ToString("yyyyMMdd"));
    }

    [Fact]
    public async Task GenerateRegistrationNumberAsync_TrainingType_ReturnsTRN_Prefix()
    {
        var result = await _engine.GenerateRegistrationNumberAsync(ProgramType.Training, CancellationToken.None);

        result.Should().StartWith("TRN-REG-");
    }

    [Fact]
    public async Task GenerateRegistrationNumberAsync_WorkshopType_ReturnsWRK_Prefix()
    {
        var result = await _engine.GenerateRegistrationNumberAsync(ProgramType.Workshop, CancellationToken.None);

        result.Should().StartWith("WRK-REG-");
    }

    [Fact]
    public async Task DetermineInitialStatusAsync_FreeRegistration_ReturnsApproved()
    {
        var result = await _engine.DetermineInitialStatusAsync(ProgramType.Event, EventRegistrationType.Free, CancellationToken.None);

        result.Should().Be(PlatformRegistrationStatus.Approved);
    }

    [Fact]
    public async Task DetermineInitialStatusAsync_PaidRegistration_ReturnsPending()
    {
        var result = await _engine.DetermineInitialStatusAsync(ProgramType.Event, EventRegistrationType.Paid, CancellationToken.None);

        result.Should().Be(PlatformRegistrationStatus.Pending);
    }

    [Fact]
    public async Task DetermineInitialStatusAsync_WaitlistRegistration_ReturnsWaitlisted()
    {
        var result = await _engine.DetermineInitialStatusAsync(ProgramType.Event, EventRegistrationType.Waitlist, CancellationToken.None);

        result.Should().Be(PlatformRegistrationStatus.Waitlisted);
    }

    [Fact]
    public async Task ValidateRegistrationEligibilityAsync_NoParticipant_ReturnsFalse()
    {
        var result = await _engine.ValidateRegistrationEligibilityAsync(
            ProgramType.Event, Guid.NewGuid(), null, null, CancellationToken.None);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task ValidateRegistrationEligibilityAsync_WithAthleteId_ReturnsTrue()
    {
        var result = await _engine.ValidateRegistrationEligibilityAsync(
            ProgramType.Event, Guid.NewGuid(), Guid.NewGuid(), null, CancellationToken.None);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateRegistrationEligibilityAsync_WithUserId_ReturnsTrue()
    {
        var result = await _engine.ValidateRegistrationEligibilityAsync(
            ProgramType.Event, Guid.NewGuid(), null, Guid.NewGuid(), CancellationToken.None);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsDuplicateRegistrationAsync_NotDuplicate_ReturnsFalse()
    {
        Func<ProgramType, Guid, Guid?, Guid?, CancellationToken, Task<bool>> duplicateCheck =
            (_, _, _, _, _) => Task.FromResult(false);

        var result = await _engine.IsDuplicateRegistrationAsync(
            ProgramType.Event, Guid.NewGuid(), Guid.NewGuid(), null, duplicateCheck, CancellationToken.None);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsDuplicateRegistrationAsync_IsDuplicate_ReturnsTrue()
    {
        Func<ProgramType, Guid, Guid?, Guid?, CancellationToken, Task<bool>> duplicateCheck =
            (_, _, _, _, _) => Task.FromResult(true);

        var result = await _engine.IsDuplicateRegistrationAsync(
            ProgramType.Event, Guid.NewGuid(), Guid.NewGuid(), null, duplicateCheck, CancellationToken.None);

        result.Should().BeTrue();
    }
}
