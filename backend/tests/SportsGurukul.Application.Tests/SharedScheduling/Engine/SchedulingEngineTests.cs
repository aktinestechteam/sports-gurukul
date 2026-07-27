using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Features.SharedScheduling.Engine;
using SportsGurukul.Application.Features.SharedScheduling.Models;

namespace SportsGurukul.Application.Tests.SharedScheduling.Engine;

public class SchedulingEngineTests
{
    private readonly Mock<IAvailabilityEngine> _availabilityMock = new();
    private readonly Mock<IConflictDetectionEngine> _conflictMock = new();
    private readonly Mock<IRecurrenceEngine> _recurrenceMock = new();
    private readonly Mock<ITimeSlotGenerator> _slotGeneratorMock = new();
    private readonly Mock<IBusinessHoursProvider> _businessHoursMock = new();
    private readonly Mock<ILogger<SchedulingEngine>> _loggerMock = new();
    private readonly SchedulingEngine _engine;

    public SchedulingEngineTests()
    {
        _engine = new SchedulingEngine(
            _availabilityMock.Object,
            _conflictMock.Object,
            _recurrenceMock.Object,
            _slotGeneratorMock.Object,
            _businessHoursMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task ScheduleAsync_NoConflicts_ReturnsSuccess()
    {
        var request = new SchedulingRequest
        {
            RequestType = "Training",
            AcademyId = Guid.NewGuid(),
            TimeSlot = TimeSlot.Create(new DateTime(2026, 1, 15), TimeSpan.FromHours(9), TimeSpan.FromHours(10)),
            Resources = [],
            CheckHolidays = false
        };
        var context = new SchedulingContext { Holidays = [] };
        _conflictMock.Setup(c => c.DetectConflictsAsync(It.IsAny<TimeSlot>(), It.IsAny<IReadOnlyList<ResourceRequirement>>(), It.IsAny<SchedulingContext>(), It.IsAny<Guid?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ConflictInfo>());

        var result = await _engine.ScheduleAsync(request, context);

        result.IsSuccess.Should().BeTrue();
        result.GeneratedSlots.Should().HaveCount(1);
    }

    [Fact]
    public async Task ScheduleAsync_WithConflictsAndNotAllowed_ReturnsFailure()
    {
        var request = new SchedulingRequest
        {
            RequestType = "Training",
            AcademyId = Guid.NewGuid(),
            TimeSlot = TimeSlot.Create(new DateTime(2026, 1, 15), TimeSpan.FromHours(9), TimeSpan.FromHours(10)),
            Resources = [new ResourceRequirement { ResourceType = "Facility", ResourceId = Guid.NewGuid() }],
            AllowConflicts = false,
            CheckHolidays = false
        };
        var context = new SchedulingContext { Holidays = [] };
        var conflicts = new List<ConflictInfo>
        {
            new()
            {
                Type = ConflictType.TimeOverlap,
                Severity = ConflictSeverity.High,
                ResourceType = "Facility",
                ResourceId = Guid.NewGuid(),
                OverlappingSlot = TimeSlot.Create(new DateTime(2026, 1, 15), TimeSpan.FromHours(9), TimeSpan.FromHours(10))
            }
        };
        _conflictMock.Setup(c => c.DetectConflictsAsync(It.IsAny<TimeSlot>(), It.IsAny<IReadOnlyList<ResourceRequirement>>(), It.IsAny<SchedulingContext>(), It.IsAny<Guid?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(conflicts);
        _availabilityMock.Setup(a => a.GetAlternativeSlotsAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<TimeSlot>(), 3, It.IsAny<SchedulingContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TimeSlot>());
        _slotGeneratorMock.Setup(s => s.MergeSlots(It.IsAny<IReadOnlyList<TimeSlot>>()))
            .Returns(new List<TimeSlot>());

        var result = await _engine.ScheduleAsync(request, context);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Contain("conflict");
    }

    [Fact]
    public async Task ScheduleAsync_WithConflictsAndAllowed_ReturnsSuccess()
    {
        var request = new SchedulingRequest
        {
            RequestType = "Training",
            AcademyId = Guid.NewGuid(),
            TimeSlot = TimeSlot.Create(new DateTime(2026, 1, 15), TimeSpan.FromHours(9), TimeSpan.FromHours(10)),
            AllowConflicts = true,
            CheckHolidays = false
        };
        var context = new SchedulingContext { Holidays = [] };
        _conflictMock.Setup(c => c.DetectConflictsAsync(It.IsAny<TimeSlot>(), It.IsAny<IReadOnlyList<ResourceRequirement>>(), It.IsAny<SchedulingContext>(), It.IsAny<Guid?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ConflictInfo> { new() { Type = ConflictType.TimeOverlap, OverlappingSlot = TimeSlot.Create(new DateTime(2026, 1, 15), TimeSpan.FromHours(9), TimeSpan.FromHours(10)) } });

        var result = await _engine.ScheduleAsync(request, context);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task ScheduleAsync_IsHoliday_ReturnsFailure()
    {
        var request = new SchedulingRequest
        {
            RequestType = "Training",
            AcademyId = Guid.NewGuid(),
            TimeSlot = TimeSlot.Create(new DateTime(2026, 1, 26), TimeSpan.FromHours(9), TimeSpan.FromHours(10)),
            CheckHolidays = true
        };
        var context = new SchedulingContext
        {
            Holidays = [new Holiday { Date = new DateTime(2026, 1, 26), Name = "Republic Day" }]
        };

        var result = await _engine.ScheduleAsync(request, context);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Contain("holiday");
    }

    [Fact]
    public async Task GenerateOccurrenceSlotsAsync_GeneratesCorrectSlots()
    {
        var baseSlot = TimeSlot.Create(new DateTime(2026, 1, 5), TimeSpan.FromHours(9), TimeSpan.FromHours(10));
        var pattern = new RecurrencePattern
        {
            Frequency = RecurrenceFrequency.Weekly,
            MaxOccurrences = 4,
            SkipHolidays = false
        };
        var context = new SchedulingContext();

        _recurrenceMock.Setup(r => r.GenerateOccurrences(pattern, baseSlot.Date))
            .Returns(new List<DateTime>
            {
                new(2026, 1, 5),
                new(2026, 1, 12),
                new(2026, 1, 19),
                new(2026, 1, 26)
            });

        var slots = await _engine.GenerateOccurrenceSlotsAsync(baseSlot, pattern, context);

        slots.Should().HaveCount(4);
        slots.All(s => s.StartTime == TimeSpan.FromHours(9)).Should().BeTrue();
    }

    [Fact]
    public async Task GenerateScheduleNumberAsync_ReturnsFormattedNumber()
    {
        var number = await _engine.GenerateScheduleNumberAsync("SS");

        number.Should().StartWith("SS-");
        number.Should().Contain(DateTime.UtcNow.ToString("yyyyMMdd"));
    }

    [Fact]
    public async Task ValidateSlotAsync_WithinBusinessHours_ReturnsTrue()
    {
        var slot = TimeSlot.Create(new DateTime(2026, 1, 15), TimeSpan.FromHours(9), TimeSpan.FromHours(10));
        var context = new SchedulingContext { CheckBusinessHours = true };

        _businessHoursMock.Setup(b => b.IsWithinBusinessHoursAsync(Guid.Empty, "System", slot, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _engine.ValidateSlotAsync(slot, context);

        result.Should().BeTrue();
    }
}
