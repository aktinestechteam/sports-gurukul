using FluentAssertions;
using FluentValidation.TestHelper;
using SportsGurukul.Application.Features.SharedScheduling.Commands.GenerateAvailableSlots;
using SportsGurukul.Application.Features.SharedScheduling.Commands.OptimizeSchedule;
using SportsGurukul.Application.Features.SharedScheduling.Commands.ResolveSchedulingConflict;
using SportsGurukul.Application.Features.SharedScheduling.Commands.ValidateBookingSlot;
using SportsGurukul.Application.Features.SharedScheduling.Models;
using SportsGurukul.Application.Features.SharedScheduling.Queries.GetAvailableSlots;
using SportsGurukul.Application.Features.SharedScheduling.Queries.GetResourceAvailability;
using SportsGurukul.Application.Features.SharedScheduling.Queries.GetResourceUtilization;
using SportsGurukul.Application.Features.SharedScheduling.Queries.GetSchedulingConflicts;
using SportsGurukul.Application.Features.SharedScheduling.Validators;

namespace SportsGurukul.Application.Tests.SharedScheduling.CQRS;

public class SharedSchedulingValidatorTests
{
    [Fact]
    public async Task GenerateAvailableSlotsCommandValidator_ValidCommand_NoErrors()
    {
        var validator = new GenerateAvailableSlotsCommandValidator();
        var command = new GenerateAvailableSlotsCommand
        {
            ResourceId = Guid.NewGuid(),
            ResourceType = "Facility",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(7),
            AcademyId = Guid.NewGuid()
        };

        var result = await validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task GenerateAvailableSlotsCommandValidator_EmptyResourceId_HasError()
    {
        var validator = new GenerateAvailableSlotsCommandValidator();
        var command = new GenerateAvailableSlotsCommand
        {
            ResourceType = "Facility",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(7),
            AcademyId = Guid.NewGuid()
        };

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.ResourceId);
    }

    [Fact]
    public async Task GenerateAvailableSlotsCommandValidator_EmptyResourceType_HasError()
    {
        var validator = new GenerateAvailableSlotsCommandValidator();
        var command = new GenerateAvailableSlotsCommand
        {
            ResourceId = Guid.NewGuid(),
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(7),
            AcademyId = Guid.NewGuid()
        };

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.ResourceType);
    }

    [Fact]
    public async Task GenerateAvailableSlotsCommandValidator_EndDateBeforeStart_HasError()
    {
        var validator = new GenerateAvailableSlotsCommandValidator();
        var command = new GenerateAvailableSlotsCommand
        {
            ResourceId = Guid.NewGuid(),
            ResourceType = "Facility",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(-1),
            AcademyId = Guid.NewGuid()
        };

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.EndDate);
    }

    [Fact]
    public async Task ValidateBookingSlotCommandValidator_ValidCommand_NoErrors()
    {
        var validator = new ValidateBookingSlotCommandValidator();
        var command = new ValidateBookingSlotCommand
        {
            Date = DateTime.UtcNow,
            StartTime = TimeSpan.FromHours(9),
            EndTime = TimeSpan.FromHours(10),
            AcademyId = Guid.NewGuid()
        };

        var result = await validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task ValidateBookingSlotCommandValidator_EndTimeBeforeStart_HasError()
    {
        var validator = new ValidateBookingSlotCommandValidator();
        var command = new ValidateBookingSlotCommand
        {
            Date = DateTime.UtcNow,
            StartTime = TimeSpan.FromHours(10),
            EndTime = TimeSpan.FromHours(9),
            AcademyId = Guid.NewGuid()
        };

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.EndTime);
    }

    [Fact]
    public async Task ValidateBookingSlotCommandValidator_EmptyAcademyId_HasError()
    {
        var validator = new ValidateBookingSlotCommandValidator();
        var command = new ValidateBookingSlotCommand
        {
            Date = DateTime.UtcNow,
            StartTime = TimeSpan.FromHours(9),
            EndTime = TimeSpan.FromHours(10)
        };

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.AcademyId);
    }

    [Fact]
    public async Task ResolveSchedulingConflictCommandValidator_ValidCommand_NoErrors()
    {
        var validator = new ResolveSchedulingConflictCommandValidator();
        var command = new ResolveSchedulingConflictCommand
        {
            ConflictId = Guid.NewGuid(),
            ResolutionNotes = "Resolved by admin"
        };

        var result = await validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task ResolveSchedulingConflictCommandValidator_EmptyNotes_HasError()
    {
        var validator = new ResolveSchedulingConflictCommandValidator();
        var command = new ResolveSchedulingConflictCommand
        {
            ConflictId = Guid.NewGuid()
        };

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.ResolutionNotes);
    }

    [Fact]
    public async Task OptimizeScheduleCommandValidator_ValidCommand_NoErrors()
    {
        var validator = new OptimizeScheduleCommandValidator();
        var command = new OptimizeScheduleCommand
        {
            ResourceType = "Facility",
            ResourceIds = [Guid.NewGuid()],
            PreferredDate = DateTime.UtcNow,
            Duration = TimeSpan.FromHours(1),
            AcademyId = Guid.NewGuid()
        };

        var result = await validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task OptimizeScheduleCommandValidator_EmptyResourceIds_HasError()
    {
        var validator = new OptimizeScheduleCommandValidator();
        var command = new OptimizeScheduleCommand
        {
            ResourceType = "Facility",
            ResourceIds = [],
            PreferredDate = DateTime.UtcNow,
            Duration = TimeSpan.FromHours(1),
            AcademyId = Guid.NewGuid()
        };

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.ResourceIds);
    }

    [Fact]
    public async Task OptimizeScheduleCommandValidator_ZeroDuration_HasError()
    {
        var validator = new OptimizeScheduleCommandValidator();
        var command = new OptimizeScheduleCommand
        {
            ResourceType = "Facility",
            ResourceIds = [Guid.NewGuid()],
            PreferredDate = DateTime.UtcNow,
            Duration = TimeSpan.Zero,
            AcademyId = Guid.NewGuid()
        };

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Duration);
    }

    [Fact]
    public async Task GetAvailableSlotsQueryValidator_ValidQuery_NoErrors()
    {
        var validator = new GetAvailableSlotsQueryValidator();
        var query = new GetAvailableSlotsQuery
        {
            ResourceId = Guid.NewGuid(),
            ResourceType = "Facility",
            Date = DateTime.UtcNow,
            AcademyId = Guid.NewGuid()
        };

        var result = await validator.TestValidateAsync(query);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task GetAvailableSlotsQueryValidator_EmptyResourceId_HasError()
    {
        var validator = new GetAvailableSlotsQueryValidator();
        var query = new GetAvailableSlotsQuery
        {
            ResourceType = "Facility",
            Date = DateTime.UtcNow,
            AcademyId = Guid.NewGuid()
        };

        var result = await validator.TestValidateAsync(query);

        result.ShouldHaveValidationErrorFor(x => x.ResourceId);
    }

    [Fact]
    public async Task GetResourceAvailabilityQueryValidator_ValidQuery_NoErrors()
    {
        var validator = new GetResourceAvailabilityQueryValidator();
        var query = new GetResourceAvailabilityQuery
        {
            ResourceId = Guid.NewGuid(),
            ResourceType = "Facility",
            Date = DateTime.UtcNow,
            AcademyId = Guid.NewGuid()
        };

        var result = await validator.TestValidateAsync(query);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task GetSchedulingConflictsQueryValidator_ValidQuery_NoErrors()
    {
        var validator = new GetSchedulingConflictsQueryValidator();
        var query = new GetSchedulingConflictsQuery
        {
            ResourceId = Guid.NewGuid(),
            ResourceType = "Facility"
        };

        var result = await validator.TestValidateAsync(query);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task GetResourceUtilizationQueryValidator_ValidQuery_NoErrors()
    {
        var validator = new GetResourceUtilizationQueryValidator();
        var query = new GetResourceUtilizationQuery
        {
            ResourceType = "Facility",
            ResourceIds = [Guid.NewGuid()],
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(7),
            AcademyId = Guid.NewGuid()
        };

        var result = await validator.TestValidateAsync(query);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task GetResourceUtilizationQueryValidator_EndDateBeforeStart_HasError()
    {
        var validator = new GetResourceUtilizationQueryValidator();
        var query = new GetResourceUtilizationQuery
        {
            ResourceType = "Facility",
            ResourceIds = [Guid.NewGuid()],
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(-1),
            AcademyId = Guid.NewGuid()
        };

        var result = await validator.TestValidateAsync(query);

        result.ShouldHaveValidationErrorFor(x => x.EndDate);
    }
}
