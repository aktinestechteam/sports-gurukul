using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Features.SharedScheduling.Commands.GenerateAvailableSlots;
using SportsGurukul.Application.Features.SharedScheduling.Commands.OptimizeSchedule;
using SportsGurukul.Application.Features.SharedScheduling.Commands.ResolveSchedulingConflict;
using SportsGurukul.Application.Features.SharedScheduling.Commands.ValidateBookingSlot;
using SportsGurukul.Application.Features.SharedScheduling.Engine;
using SportsGurukul.Application.Features.SharedScheduling.Models;
using SportsGurukul.Application.Features.SharedScheduling.Queries.GetAvailableSlots;
using SportsGurukul.Application.Features.SharedScheduling.Queries.GetResourceAvailability;
using SportsGurukul.Application.Features.SharedScheduling.Queries.GetSchedulingConflicts;

namespace SportsGurukul.Application.Tests.SharedScheduling.CQRS;

public class SharedSchedulingCommandHandlerTests
{
    private readonly Mock<IAvailabilityEngine> _availabilityMock = new();
    private readonly Mock<ISchedulingEngine> _schedulingMock = new();
    private readonly Mock<IConflictDetectionEngine> _conflictMock = new();
    private readonly Mock<IOptimizationEngine> _optimizationMock = new();
    private static Mock<ILogger<T>> CreateLogger<T>() where T : class => new();

    [Fact]
    public async Task GenerateAvailableSlotsCommandHandler_ReturnsSlots()
    {
        var handler = new GenerateAvailableSlotsCommandHandler(_availabilityMock.Object, CreateLogger<GenerateAvailableSlotsCommandHandler>().Object);
        var command = new GenerateAvailableSlotsCommand
        {
            ResourceId = Guid.NewGuid(),
            ResourceType = "Facility",
            StartDate = new DateTime(2026, 1, 15),
            EndDate = new DateTime(2026, 1, 16),
            AcademyId = Guid.NewGuid()
        };
        var expectedSlots = new List<TimeSlot>
        {
            TimeSlot.Create(new DateTime(2026, 1, 15), TimeSpan.FromHours(9), TimeSpan.FromHours(10)),
            TimeSlot.Create(new DateTime(2026, 1, 16), TimeSpan.FromHours(9), TimeSpan.FromHours(10))
        };
        _availabilityMock.Setup(a => a.GetAvailableSlotsAsync(command.ResourceId, command.ResourceType, It.IsAny<DateTime>(), It.IsAny<SchedulingContext>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedSlots);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(4);
    }

    [Fact]
    public async Task ValidateBookingSlotCommandHandler_ValidSlot_ReturnsTrue()
    {
        var handler = new ValidateBookingSlotCommandHandler(_schedulingMock.Object, CreateLogger<ValidateBookingSlotCommandHandler>().Object);
        var command = new ValidateBookingSlotCommand
        {
            Date = new DateTime(2026, 1, 15),
            StartTime = TimeSpan.FromHours(9),
            EndTime = TimeSpan.FromHours(10),
            AcademyId = Guid.NewGuid()
        };
        _schedulingMock.Setup(s => s.ValidateSlotAsync(It.IsAny<TimeSlot>(), It.IsAny<SchedulingContext>(), It.IsAny<IReadOnlyList<ResourceRequirement>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateBookingSlotCommandHandler_InvalidSlot_ReturnsFalse()
    {
        var handler = new ValidateBookingSlotCommandHandler(_schedulingMock.Object, CreateLogger<ValidateBookingSlotCommandHandler>().Object);
        var command = new ValidateBookingSlotCommand
        {
            Date = new DateTime(2026, 1, 15),
            StartTime = TimeSpan.FromHours(9),
            EndTime = TimeSpan.FromHours(10),
            AcademyId = Guid.NewGuid()
        };
        _schedulingMock.Setup(s => s.ValidateSlotAsync(It.IsAny<TimeSlot>(), It.IsAny<SchedulingContext>(), It.IsAny<IReadOnlyList<ResourceRequirement>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeFalse();
    }

    [Fact]
    public async Task ResolveSchedulingConflictCommandHandler_Success_ReturnsTrue()
    {
        var handler = new ResolveSchedulingConflictCommandHandler(_conflictMock.Object, CreateLogger<ResolveSchedulingConflictCommandHandler>().Object);
        var command = new ResolveSchedulingConflictCommand
        {
            ConflictId = Guid.NewGuid(),
            ResolutionNotes = "Resolved"
        };
        _conflictMock.Setup(c => c.ResolveConflictAsync(command.ConflictId, command.ResolutionNotes, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
    }

    [Fact]
    public async Task ResolveSchedulingConflictCommandHandler_Failure_ReturnsError()
    {
        var handler = new ResolveSchedulingConflictCommandHandler(_conflictMock.Object, CreateLogger<ResolveSchedulingConflictCommandHandler>().Object);
        var command = new ResolveSchedulingConflictCommand
        {
            ConflictId = Guid.NewGuid(),
            ResolutionNotes = "Resolved"
        };
        _conflictMock.Setup(c => c.ResolveConflictAsync(command.ConflictId, command.ResolutionNotes, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task OptimizeScheduleCommandHandler_FindsSlot_ReturnsSlot()
    {
        var handler = new OptimizeScheduleCommandHandler(_optimizationMock.Object, CreateLogger<OptimizeScheduleCommandHandler>().Object);
        var command = new OptimizeScheduleCommand
        {
            ResourceType = "Facility",
            ResourceIds = [Guid.NewGuid()],
            PreferredDate = new DateTime(2026, 1, 15),
            Duration = TimeSpan.FromHours(1),
            AcademyId = Guid.NewGuid()
        };
        var expected = TimeSlot.Create(new DateTime(2026, 1, 15), TimeSpan.FromHours(9), TimeSpan.FromHours(10));
        _optimizationMock.Setup(o => o.FindBestAvailableSlotAsync(command.ResourceType, command.ResourceIds, command.PreferredDate, command.Duration, It.IsAny<SchedulingContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task OptimizeScheduleCommandHandler_NoSlot_ReturnsNull()
    {
        var handler = new OptimizeScheduleCommandHandler(_optimizationMock.Object, CreateLogger<OptimizeScheduleCommandHandler>().Object);
        var command = new OptimizeScheduleCommand
        {
            ResourceType = "Facility",
            ResourceIds = [Guid.NewGuid()],
            PreferredDate = new DateTime(2026, 1, 15),
            Duration = TimeSpan.FromHours(1),
            AcademyId = Guid.NewGuid()
        };
        _optimizationMock.Setup(o => o.FindBestAvailableSlotAsync(command.ResourceType, command.ResourceIds, command.PreferredDate, command.Duration, It.IsAny<SchedulingContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TimeSlot?)null);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeNull();
    }

    [Fact]
    public async Task GetAvailableSlotsQueryHandler_ReturnsSlots()
    {
        var handler = new GetAvailableSlotsQueryHandler(_availabilityMock.Object, CreateLogger<GetAvailableSlotsQueryHandler>().Object);
        var query = new GetAvailableSlotsQuery
        {
            ResourceId = Guid.NewGuid(),
            ResourceType = "Facility",
            Date = new DateTime(2026, 1, 15),
            AcademyId = Guid.NewGuid()
        };
        var slots = new List<TimeSlot>
        {
            TimeSlot.Create(new DateTime(2026, 1, 15), TimeSpan.FromHours(9), TimeSpan.FromHours(10))
        };
        _availabilityMock.Setup(a => a.GetAvailableSlotsAsync(query.ResourceId, query.ResourceType, query.Date, It.IsAny<SchedulingContext>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(slots);

        var result = await handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetResourceAvailabilityQueryHandler_ReturnsWindow()
    {
        var handler = new GetResourceAvailabilityQueryHandler(_availabilityMock.Object, CreateLogger<GetResourceAvailabilityQueryHandler>().Object);
        var query = new GetResourceAvailabilityQuery
        {
            ResourceId = Guid.NewGuid(),
            ResourceType = "Facility",
            Date = new DateTime(2026, 1, 15),
            AcademyId = Guid.NewGuid()
        };
        var window = new AvailabilityWindow
        {
            ResourceId = query.ResourceId,
            ResourceType = query.ResourceType,
            Date = query.Date,
            AvailableSlots = [TimeSlot.Create(query.Date, TimeSpan.FromHours(9), TimeSpan.FromHours(10))],
            BlockedSlots = []
        };
        _availabilityMock.Setup(a => a.GetAvailabilityWindowAsync(query.ResourceId, query.ResourceType, query.Date, It.IsAny<SchedulingContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(window);

        var result = await handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.AvailableSlots.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetSchedulingConflictsQueryHandler_ReturnsConflicts()
    {
        var handler = new GetSchedulingConflictsQueryHandler(_conflictMock.Object, CreateLogger<GetSchedulingConflictsQueryHandler>().Object);
        var query = new GetSchedulingConflictsQuery
        {
            ResourceId = Guid.NewGuid(),
            ResourceType = "Facility"
        };
        var conflicts = new List<ConflictInfo>
        {
            new() { ConflictId = Guid.NewGuid(), Type = ConflictType.TimeOverlap, ResourceType = "Facility" }
        };
        _conflictMock.Setup(c => c.GetUnresolvedConflictsAsync(query.ResourceId, query.ResourceType, It.IsAny<CancellationToken>()))
            .ReturnsAsync(conflicts);

        var result = await handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
    }
}
