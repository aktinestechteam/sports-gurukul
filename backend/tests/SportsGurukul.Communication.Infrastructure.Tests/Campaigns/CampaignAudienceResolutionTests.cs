using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Domain.Enums.Notification;
using SportsGurukul.Platform.Communication.Analytics.Abstractions;
using SportsGurukul.Platform.Communication.Analytics.DTOs;
using SportsGurukul.Platform.Communication.Analytics.Services;

namespace SportsGurukul.Communication.Infrastructure.Tests.Campaigns;

public class CampaignAudienceResolutionTests
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
    public async Task TriggerNowAsync_WithSegmentAudience_ResolvesUsers()
    {
        var segmentId = Guid.NewGuid().ToString();
        var segmentUsers = new List<string> { "athlete-0001", "athlete-0002", "athlete-0003" };
        var audience = new AudienceDefinitionDto(
            new List<string> { segmentId }, null, null, null, null, false, null);

        _audienceSegmentationMock
            .Setup(a => a.EvaluateSegmentAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SegmentResultDto(Guid.Parse(segmentId), "Test Segment", segmentUsers, 3,
                DateTime.UtcNow, 10, new(), null));

        var service = CreateService();
        var request = new CreateCampaignFullRequest(
            "Segment Campaign", null, CampaignType.OneTime, Guid.NewGuid(),
            NotificationChannelType.Email, null, audience, null);
        var created = await service.CreateAsync(request, "test-user");
        await service.ActivateAsync(created.Id);

        var result = await service.TriggerNowAsync(created.Id);

        result.RecipientsQueued.Should().Be(3);
        _audienceSegmentationMock.Verify(a => a.EvaluateSegmentAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task TriggerNowAsync_WithExplicitUserIds_ResolvesUsers()
    {
        var userIds = new List<string> { "user-0001", "user-0002" };
        var audience = new AudienceDefinitionDto(null, userIds, null, null, null, false, null);

        var service = CreateService();
        var request = new CreateCampaignFullRequest(
            "Explicit Users", null, CampaignType.OneTime, Guid.NewGuid(),
            NotificationChannelType.Email, null, audience, null);
        var created = await service.CreateAsync(request, "test-user");
        await service.ActivateAsync(created.Id);

        var result = await service.TriggerNowAsync(created.Id);

        result.RecipientsQueued.Should().Be(2);
    }

    [Fact]
    public async Task TriggerNowAsync_WithRoleFilter_ResolvesUsers()
    {
        var audience = new AudienceDefinitionDto(null, null, new List<string> { "athlete" }, null, null, false, null);

        _audienceSegmentationMock
            .Setup(a => a.ResolveSegmentAsync(SegmentType.ByRole,
                It.Is<Dictionary<string, object>>(d => (string)d["role"] == "athlete"),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SegmentResultDto(Guid.Empty, "Athletes",
                new List<string> { "athlete-0001", "athlete-0002" }, 2, DateTime.UtcNow, 5, new(), null));

        var service = CreateService();
        var request = new CreateCampaignFullRequest(
            "Role Campaign", null, CampaignType.OneTime, Guid.NewGuid(),
            NotificationChannelType.Email, null, audience, null);
        var created = await service.CreateAsync(request, "test-user");
        await service.ActivateAsync(created.Id);

        var result = await service.TriggerNowAsync(created.Id);

        result.RecipientsQueued.Should().Be(2);
    }

    [Fact]
    public async Task TriggerNowAsync_WithTagFilter_ResolvesUsers()
    {
        var audience = new AudienceDefinitionDto(null, null, null, new List<string> { "premium" }, null, false, null);

        _audienceSegmentationMock
            .Setup(a => a.ResolveSegmentAsync(SegmentType.ByTag,
                It.IsAny<Dictionary<string, object>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SegmentResultDto(Guid.Empty, "Tagged Users",
                new List<string> { "premium-0001" }, 1, DateTime.UtcNow, 3, new(), null));

        var service = CreateService();
        var request = new CreateCampaignFullRequest(
            "Tag Campaign", null, CampaignType.OneTime, Guid.NewGuid(),
            NotificationChannelType.Email, null, audience, null);
        var created = await service.CreateAsync(request, "test-user");
        await service.ActivateAsync(created.Id);

        var result = await service.TriggerNowAsync(created.Id);

        result.RecipientsQueued.Should().Be(1);
    }

    [Fact]
    public async Task TriggerNowAsync_WithIncludeAllUsers_ResolvesAll()
    {
        var audience = new AudienceDefinitionDto(null, null, null, null, null, true, null);

        _audienceSegmentationMock
            .Setup(a => a.ResolveSegmentAsync(SegmentType.AllUsers, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SegmentResultDto(Guid.Empty, "All Users",
                new List<string> { "user-0001", "user-0002" }, 2, DateTime.UtcNow, 50, new(), null));

        var service = CreateService();
        var request = new CreateCampaignFullRequest(
            "All Users Campaign", null, CampaignType.OneTime, Guid.NewGuid(),
            NotificationChannelType.Email, null, audience, null);
        var created = await service.CreateAsync(request, "test-user");
        await service.ActivateAsync(created.Id);

        var result = await service.TriggerNowAsync(created.Id);

        result.RecipientsQueued.Should().Be(2);
    }

    [Fact]
    public async Task TriggerNowAsync_WithNullAudience_ReturnsEmpty()
    {
        var service = CreateService();
        var request = new CreateCampaignFullRequest(
            "No Audience", null, CampaignType.OneTime, Guid.NewGuid(),
            NotificationChannelType.Email, null, null, null);
        var created = await service.CreateAsync(request, "test-user");
        await service.ActivateAsync(created.Id);

        var result = await service.TriggerNowAsync(created.Id);

        result.RecipientsQueued.Should().Be(0);
    }

    [Fact]
    public async Task TriggerNowAsync_WithInvalidSegment_ReturnsEmpty()
    {
        var audience = new AudienceDefinitionDto(new List<string> { Guid.NewGuid().ToString() }, null, null, null, null, false, null);

        _audienceSegmentationMock
            .Setup(a => a.EvaluateSegmentAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException("Segment not found"));

        var service = CreateService();
        var request = new CreateCampaignFullRequest(
            "Invalid Segment", null, CampaignType.OneTime, Guid.NewGuid(),
            NotificationChannelType.Email, null, audience, null);
        var created = await service.CreateAsync(request, "test-user");
        await service.ActivateAsync(created.Id);

        var result = await service.TriggerNowAsync(created.Id);

        result.RecipientsQueued.Should().Be(0);
    }
}
