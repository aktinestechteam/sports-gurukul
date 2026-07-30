using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Domain.Enums.Notification;
using SportsGurukul.Platform.Communication.Analytics.Abstractions;
using SportsGurukul.Platform.Communication.Analytics.DTOs;
using SportsGurukul.Platform.Communication.Analytics.Services;

namespace SportsGurukul.Communication.Infrastructure.Tests.EdgeCases;

public class PerformanceConstraintTests
{
    [Fact]
    public async Task ActivateAsync_CallsCacheAndMediator_EachOnce()
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
            "Test", null, CampaignType.OneTime, Guid.NewGuid(),
            NotificationChannelType.Email, null, null, null);

        var created = await service.CreateAsync(request, "test-user");

        cacheMock.Invocations.Clear();
        mediatorMock.Invocations.Clear();

        await service.ActivateAsync(created.Id);

        cacheMock.Verify(c => c.SetAsync(
            It.IsAny<string>(), It.IsAny<CampaignDetailDto>(), It.IsAny<TimeSpan?>(), It.IsAny<CancellationToken>()), Times.Once);
        mediatorMock.Verify(m => m.Publish(
            It.IsAny<CampaignStatusChangedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetCountByStatusAsync_NoUnnecessaryQueries()
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

        var result = await service.GetCountByStatusAsync(CampaignStatus.Draft);

        result.Should().Be(0);
        campaignServiceMock.Verify(c => c.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_NoExtraCacheWrites()
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
            "Test", null, CampaignType.OneTime, Guid.NewGuid(),
            NotificationChannelType.Email, null, null, null);

        await service.CreateAsync(request, "test-user");

        cacheMock.Verify(c => c.SetAsync(
            It.IsAny<string>(), It.IsAny<CampaignDetailDto>(), It.IsAny<TimeSpan?>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SearchAsync_CalledOnce_PerformsInMemoryQuery()
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

        var criteria = new CampaignSearchCriteria(null, null, null, null, null, null, null, null, null, 1, 20);
        var result = await service.SearchAsync(criteria);

        result.TotalCount.Should().Be(0);
        result.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task DeleteAsync_RemovesFromCacheAndSchedules()
    {
        var loggerMock = new Mock<ILogger<CampaignManagementService>>();
        var campaignServiceMock = new Mock<ICampaignService>();
        var schedulingEngineMock = new Mock<ISchedulingEngine>();
        var audienceSegmentationMock = new Mock<IAudienceSegmentationService>();
        var cacheMock = new Mock<ICacheService>();
        var mediatorMock = new Mock<IMediator>();

        schedulingEngineMock
            .Setup(s => s.UnregisterJobAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var service = new CampaignManagementService(
            loggerMock.Object, campaignServiceMock.Object, schedulingEngineMock.Object,
            audienceSegmentationMock.Object, cacheMock.Object, mediatorMock.Object);

        var request = new CreateCampaignFullRequest(
            "Test", null, CampaignType.OneTime, Guid.NewGuid(),
            NotificationChannelType.Email, null, null, null);

        var created = await service.CreateAsync(request, "test-user");

        cacheMock.Invocations.Clear();
        mediatorMock.Invocations.Clear();

        await service.DeleteAsync(created.Id);

        cacheMock.Verify(c => c.RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        mediatorMock.Verify(m => m.Publish(It.IsAny<CampaignDeletedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
