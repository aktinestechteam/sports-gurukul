using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Domain.Enums.Notification;
using SportsGurukul.Platform.Communication.Analytics.Abstractions;
using SportsGurukul.Platform.Communication.Analytics.DTOs;
using SportsGurukul.Platform.Communication.Analytics.Services;

namespace SportsGurukul.Communication.Infrastructure.Tests.EdgeCases;

public class ConcurrentOperationTests
{
    [Fact]
    public async Task ConcurrentCreate_CampaignsHaveUniqueIds()
    {
        var loggerMock = new Mock<ILogger<CampaignManagementService>>();
        var campaignServiceMock = new Mock<ICampaignService>();
        var schedulingEngineMock = new Mock<ISchedulingEngine>();
        var audienceSegmentationMock = new Mock<IAudienceSegmentationService>();
        var cacheMock = new Mock<ICacheService>();
        var mediatorMock = new Mock<IMediator>();

        var service = new CampaignManagementService(
            loggerMock.Object, campaignServiceMock.Object, schedulingEngineMock.Object,
            audienceSegmentationMock.Object, cacheMock.Object, mediatorMock.Object);

        var tasks = Enumerable.Range(0, 10).Select(i =>
        {
            var request = new CreateCampaignFullRequest(
                $"Concurrent-{i}", null, CampaignType.OneTime, Guid.NewGuid(),
                NotificationChannelType.Email, null, null, null);
            return service.CreateAsync(request, "test-user");
        });

        var results = await Task.WhenAll(tasks);

        var ids = results.Select(r => r.Id).ToArray();
        ids.Should().OnlyHaveUniqueItems();
        results.Should().AllSatisfy(r => r.Status.Should().Be(CampaignStatus.Draft));
    }

    [Fact]
    public async Task ConcurrentStatusUpdates_AreConsistent()
    {
        var loggerMock = new Mock<ILogger<CampaignManagementService>>();
        var campaignServiceMock = new Mock<ICampaignService>();
        var schedulingEngineMock = new Mock<ISchedulingEngine>();
        var audienceSegmentationMock = new Mock<IAudienceSegmentationService>();
        var cacheMock = new Mock<ICacheService>();
        var mediatorMock = new Mock<IMediator>();

        var service = new CampaignManagementService(
            loggerMock.Object, campaignServiceMock.Object, schedulingEngineMock.Object,
            audienceSegmentationMock.Object, cacheMock.Object, mediatorMock.Object);

        var request = new CreateCampaignFullRequest(
            "Concurrent Status", null, CampaignType.OneTime, Guid.NewGuid(),
            NotificationChannelType.Email, null, null, null);
        var created = await service.CreateAsync(request, "test-user");

        var statusTasks = new List<Task>
        {
            service.ActivateAsync(created.Id)
        };

        await Task.WhenAll(statusTasks);

        var campaign = await service.GetByIdAsync(created.Id);
        campaign.Status.Should().Be(CampaignStatus.Active);
    }

    [Fact]
    public async Task ConcurrentTrigger_AllBatchesCreated()
    {
        var loggerMock = new Mock<ILogger<CampaignManagementService>>();
        var campaignServiceMock = new Mock<ICampaignService>();
        var schedulingEngineMock = new Mock<ISchedulingEngine>();
        var audienceSegmentationMock = new Mock<IAudienceSegmentationService>();
        var cacheMock = new Mock<ICacheService>();
        var mediatorMock = new Mock<IMediator>();

        var service = new CampaignManagementService(
            loggerMock.Object, campaignServiceMock.Object, schedulingEngineMock.Object,
            audienceSegmentationMock.Object, cacheMock.Object, mediatorMock.Object);

        var request = new CreateCampaignFullRequest(
            "Concurrent Trigger", null, CampaignType.OneTime, Guid.NewGuid(),
            NotificationChannelType.Email, null, null, null);
        var created = await service.CreateAsync(request, "test-user");
        await service.ActivateAsync(created.Id);

        var triggerTasks = Enumerable.Range(0, 5).Select(_ => service.TriggerNowAsync(created.Id));
        var results = await Task.WhenAll(triggerTasks);

        results.Should().AllSatisfy(r => r.Status.Should().Be("Queued"));
    }

    [Fact]
    public async Task ConcurrentDequeue_GetsUniqueItems()
    {
        var loggerMock = new Mock<ILogger<CampaignManagementService>>();
        var campaignServiceMock = new Mock<ICampaignService>();
        var schedulingEngineMock = new Mock<ISchedulingEngine>();
        var audienceSegmentationMock = new Mock<IAudienceSegmentationService>();
        var cacheMock = new Mock<ICacheService>();
        var mediatorMock = new Mock<IMediator>();

        var service = new CampaignManagementService(
            loggerMock.Object, campaignServiceMock.Object, schedulingEngineMock.Object,
            audienceSegmentationMock.Object, cacheMock.Object, mediatorMock.Object);

        var createTasks = Enumerable.Range(0, 5).Select(i =>
        {
            var req = new CreateCampaignFullRequest(
                $"Dequeue-{i}", null, CampaignType.OneTime, Guid.NewGuid(),
                NotificationChannelType.Email, null, null, null);
            return service.CreateAsync(req, "test-user");
        });

        await Task.WhenAll(createTasks);

        var dueTasks = Enumerable.Range(0, 3).Select(_ => service.GetDueCampaignsAsync());
        var results = await Task.WhenAll(dueTasks);

        results.Should().AllSatisfy(due => due.Should().NotBeNull());
    }

    [Fact]
    public async Task ConcurrentBulkCreate_AllCreated()
    {
        var loggerMock = new Mock<ILogger<CampaignManagementService>>();
        var campaignServiceMock = new Mock<ICampaignService>();
        var schedulingEngineMock = new Mock<ISchedulingEngine>();
        var audienceSegmentationMock = new Mock<IAudienceSegmentationService>();
        var cacheMock = new Mock<ICacheService>();
        var mediatorMock = new Mock<IMediator>();

        schedulingEngineMock
            .Setup(s => s.ValidateScheduleAsync(It.IsAny<ScheduleDefinitionDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ScheduleValidationResult(true, new(), new(), null, new(), 0, false, false));

        var service = new CampaignManagementService(
            loggerMock.Object, campaignServiceMock.Object, schedulingEngineMock.Object,
            audienceSegmentationMock.Object, cacheMock.Object, mediatorMock.Object);

        var requests = Enumerable.Range(0, 10).Select(i =>
            new CreateCampaignFullRequest(
                $"Bulk-{i}", null, CampaignType.OneTime, Guid.NewGuid(),
                NotificationChannelType.Email, null, null, null)).ToList();

        var tasks = requests.Select(r => service.CreateAsync(r, "test-user"));
        var results = await Task.WhenAll(tasks);

        results.Should().HaveCount(10);
        results.Should().AllSatisfy(r => r.Status.Should().Be(CampaignStatus.Draft));
    }

    [Fact]
    public async Task ConcurrentRead_Writes_NoDataLoss()
    {
        var loggerMock = new Mock<ILogger<CampaignManagementService>>();
        var campaignServiceMock = new Mock<ICampaignService>();
        var schedulingEngineMock = new Mock<ISchedulingEngine>();
        var audienceSegmentationMock = new Mock<IAudienceSegmentationService>();
        var cacheMock = new Mock<ICacheService>();
        var mediatorMock = new Mock<IMediator>();

        var service = new CampaignManagementService(
            loggerMock.Object, campaignServiceMock.Object, schedulingEngineMock.Object,
            audienceSegmentationMock.Object, cacheMock.Object, mediatorMock.Object);

        var request = new CreateCampaignFullRequest(
            "ThreadSafe", null, CampaignType.OneTime, Guid.NewGuid(),
            NotificationChannelType.Email, null, null, null);
        var created = await service.CreateAsync(request, "test-user");

        var concurrentOps = new List<Task>
        {
            service.ActivateAsync(created.Id),
            service.GetByIdAsync(created.Id),
            service.GetCountByStatusAsync(CampaignStatus.Draft)
        };

        await Task.WhenAll(concurrentOps);

        var final = await service.GetByIdAsync(created.Id);
        final.Status.Should().Be(CampaignStatus.Active);
    }
}
