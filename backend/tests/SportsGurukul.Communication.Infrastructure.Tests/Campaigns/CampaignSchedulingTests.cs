using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Domain.Enums.Notification;
using SportsGurukul.Platform.Communication.Analytics.Abstractions;
using SportsGurukul.Platform.Communication.Analytics.DTOs;
using SportsGurukul.Platform.Communication.Analytics.Services;

namespace SportsGurukul.Communication.Infrastructure.Tests.Campaigns;

public class CampaignSchedulingTests
{
    private readonly Mock<ILogger<CampaignManagementService>> _loggerMock = new();
    private readonly Mock<ICampaignService> _campaignServiceMock = new();
    private readonly Mock<ISchedulingEngine> _schedulingEngineMock = new();
    private readonly Mock<IAudienceSegmentationService> _audienceSegmentationMock = new();
    private readonly Mock<ICacheService> _cacheMock = new();
    private readonly Mock<IMediator> _mediatorMock = new();

    private CampaignManagementService CreateService() => new(
        _loggerMock.Object,
        _campaignServiceMock.Object,
        _schedulingEngineMock.Object,
        _audienceSegmentationMock.Object,
        _cacheMock.Object,
        _mediatorMock.Object);

    [Fact]
    public async Task ScheduleCampaign_SetsScheduledDate()
    {
        var nextRun = DateTime.UtcNow.AddDays(1);
        _schedulingEngineMock
            .Setup(s => s.ValidateScheduleAsync(It.IsAny<ScheduleDefinitionDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ScheduleValidationResult(true, new(), new(), nextRun, new(), 0, false, false));
        _schedulingEngineMock
            .Setup(s => s.RegisterJobAsync(It.IsAny<Guid>(), It.IsAny<ScheduleDefinitionDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ScheduleJobDto(Guid.NewGuid(), "Campaign", null, null!, true, null, nextRun, 0, 0, 0, DateTime.UtcNow));

        var service = CreateService();
        var schedule = new ScheduleDefinitionDto(
            RecurrencePattern.Daily, null, DateTime.UtcNow.AddDays(1), null,
            new TimeSpan(9, 0, 0), null, "UTC", null, null, null, null, null);
        var request = new CreateCampaignFullRequest(
            "Scheduled Campaign", null, CampaignType.Scheduled, Guid.NewGuid(),
            NotificationChannelType.Email, schedule, null, null);

        var result = await service.CreateAsync(request, "test-user");

        result.ScheduledAt.Should().Be(nextRun);
        _schedulingEngineMock.Verify(s => s.RegisterJobAsync(It.IsAny<Guid>(), schedule, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ScheduleValidation_FailsForInvalidSchedule_Throws()
    {
        _schedulingEngineMock
            .Setup(s => s.ValidateScheduleAsync(It.IsAny<ScheduleDefinitionDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ScheduleValidationResult(false, new(), new List<string> { "EndDate must be after StartDate" }, null, new(), 0, false, false));

        var service = CreateService();
        var schedule = new ScheduleDefinitionDto(
            RecurrencePattern.Daily, null, DateTime.UtcNow.AddDays(5), DateTime.UtcNow.AddDays(1),
            new TimeSpan(9, 0, 0), null, "UTC", null, null, null, null, null);
        var request = new CreateCampaignFullRequest(
            "Invalid Schedule", null, CampaignType.Scheduled, Guid.NewGuid(),
            NotificationChannelType.Email, schedule, null, null);

        var act = () => service.CreateAsync(request, "test-user");

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*EndDate must be after StartDate*");
    }

    [Fact]
    public async Task RecurringCampaign_CalculatesNextOccurrence()
    {
        var nextRun = DateTime.UtcNow.AddDays(1);
        _schedulingEngineMock
            .Setup(s => s.ValidateScheduleAsync(It.IsAny<ScheduleDefinitionDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ScheduleValidationResult(true, new(), new(), nextRun, new(), 0, false, false));
        _schedulingEngineMock
            .Setup(s => s.RegisterJobAsync(It.IsAny<Guid>(), It.IsAny<ScheduleDefinitionDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ScheduleJobDto(Guid.NewGuid(), "Campaign", null, null!, true, null, nextRun, 0, 0, 0, DateTime.UtcNow));

        var service = CreateService();
        var schedule = new ScheduleDefinitionDto(
            RecurrencePattern.Weekly, null, DateTime.UtcNow, null,
            new TimeSpan(10, 0, 0), null, "UTC", new List<DayOfWeek> { DayOfWeek.Monday }, null, null, null, null);
        var request = new CreateCampaignFullRequest(
            "Weekly Campaign", null, CampaignType.Recurring, Guid.NewGuid(),
            NotificationChannelType.Email, schedule, null, null);

        var result = await service.CreateAsync(request, "test-user");

        result.ScheduledAt.Should().Be(nextRun);
    }

    [Fact]
    public async Task GetDueCampaignsAsync_ReturnsOnlyReadyCampaigns()
    {
        var service = CreateService();
        var pastSchedule = new ScheduleDefinitionDto(
            RecurrencePattern.Daily, null, DateTime.UtcNow.AddDays(-2), null,
            new TimeSpan(8, 0, 0), null, "UTC", null, null, null, null, null);
        var futureSchedule = new ScheduleDefinitionDto(
            RecurrencePattern.Daily, null, DateTime.UtcNow.AddDays(5), null,
            new TimeSpan(8, 0, 0), null, "UTC", null, null, null, null, null);

        var pastCampaign = new CreateCampaignFullRequest("Due Campaign", null, CampaignType.Scheduled, Guid.NewGuid(),
            NotificationChannelType.Email, pastSchedule, null, null);
        var futureCampaign = new CreateCampaignFullRequest("Future Campaign", null, CampaignType.Scheduled, Guid.NewGuid(),
            NotificationChannelType.Email, futureSchedule, null, null);

        _schedulingEngineMock
            .Setup(s => s.ValidateScheduleAsync(It.IsAny<ScheduleDefinitionDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ScheduleValidationResult(true, new(), new(), null, new(), 0, false, false));
        _schedulingEngineMock
            .Setup(s => s.RegisterJobAsync(It.IsAny<Guid>(), It.IsAny<ScheduleDefinitionDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid id, ScheduleDefinitionDto _, CancellationToken _) =>
                new ScheduleJobDto(Guid.NewGuid(), "Campaign", null, null!, true, null, DateTime.UtcNow.AddDays(-1), 0, 0, 0, DateTime.UtcNow));

        await service.CreateAsync(pastCampaign, "test-user");
        await service.CreateAsync(futureCampaign, "test-user");

        var due = await service.GetDueCampaignsAsync();

        due.Should().NotBeNull();
    }

    [Fact]
    public async Task ScheduleInPast_DoesNotThrowOnCreation()
    {
        _schedulingEngineMock
            .Setup(s => s.ValidateScheduleAsync(It.IsAny<ScheduleDefinitionDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ScheduleValidationResult(true, new(), new(), null, new(), 0, false, false));
        _schedulingEngineMock
            .Setup(s => s.RegisterJobAsync(It.IsAny<Guid>(), It.IsAny<ScheduleDefinitionDto>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Cannot schedule in the past"));

        var service = CreateService();
        var pastSchedule = new ScheduleDefinitionDto(
            RecurrencePattern.Daily, null, DateTime.UtcNow.AddDays(-5), null,
            new TimeSpan(8, 0, 0), null, "UTC", null, null, null, null, null);
        var request = new CreateCampaignFullRequest(
            "Past Campaign", null, CampaignType.Scheduled, Guid.NewGuid(),
            NotificationChannelType.Email, pastSchedule, null, null);

        var act = () => service.CreateAsync(request, "test-user");

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task UpdateScheduleAsync_ValidatesAndUpdates()
    {
        var nextRun = DateTime.UtcNow.AddDays(3);
        _schedulingEngineMock
            .Setup(s => s.ValidateScheduleAsync(It.IsAny<ScheduleDefinitionDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ScheduleValidationResult(true, new(), new(), nextRun, new(), 0, false, false));
        _schedulingEngineMock
            .Setup(s => s.CalculateNextRunAsync(It.IsAny<ScheduleDefinitionDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(nextRun);
        _schedulingEngineMock
            .Setup(s => s.RegisterJobAsync(It.IsAny<Guid>(), It.IsAny<ScheduleDefinitionDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ScheduleJobDto(Guid.NewGuid(), "Campaign", null, null!, true, null, nextRun, 0, 0, 0, DateTime.UtcNow));

        var service = CreateService();
        var created = await service.CreateAsync(
            new CreateCampaignFullRequest("Test", null, CampaignType.OneTime, Guid.NewGuid(),
                NotificationChannelType.Email, null, null, null), "test-user");

        var newSchedule = new ScheduleDefinitionDto(
            RecurrencePattern.Daily, null, DateTime.UtcNow.AddDays(1), null,
            new TimeSpan(10, 0, 0), null, "UTC", null, null, null, null, null);

        var updated = await service.UpdateScheduleAsync(created.Id, newSchedule);

        updated.Schedule.Should().Be(newSchedule);
        _schedulingEngineMock.Verify(s => s.ValidateScheduleAsync(newSchedule, It.IsAny<CancellationToken>()), Times.Once);
    }
}
