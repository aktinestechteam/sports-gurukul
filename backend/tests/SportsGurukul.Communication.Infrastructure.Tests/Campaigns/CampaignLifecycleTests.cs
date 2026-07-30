using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Domain.Enums.Notification;
using SportsGurukul.Platform.Communication.Analytics.Abstractions;
using SportsGurukul.Platform.Communication.Analytics.DTOs;
using SportsGurukul.Platform.Communication.Analytics.Services;

namespace SportsGurukul.Communication.Infrastructure.Tests.Campaigns;

public class CampaignLifecycleTests
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

    private static CreateCampaignFullRequest MakeRequest(string name = "Test Campaign") => new(
        name, null, CampaignType.OneTime, Guid.NewGuid(),
        NotificationChannelType.Email, null, null, null);

    [Fact]
    public async Task CreateAsync_WithValidRequest_ReturnsDraftCampaign()
    {
        var service = CreateService();
        var request = MakeRequest();

        var result = await service.CreateAsync(request, "test-user");

        result.Status.Should().Be(CampaignStatus.Draft);
        result.Name.Should().Be("Test Campaign");
        result.CreatedBy.Should().Be("test-user");
    }

    [Fact]
    public async Task ActivateAsync_FromDraft_MovesToActive()
    {
        var service = CreateService();
        var created = await service.CreateAsync(MakeRequest(), "test-user");

        var activated = await service.ActivateAsync(created.Id);

        activated.Status.Should().Be(CampaignStatus.Active);
        activated.StartedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task PauseAsync_FromActive_MovesToPaused()
    {
        var service = CreateService();
        var created = await service.CreateAsync(MakeRequest(), "test-user");
        await service.ActivateAsync(created.Id);

        var result = await service.PauseAsync(created.Id);

        result.PreviousStatus.Should().Be(CampaignStatus.Active);
        result.NewStatus.Should().Be(CampaignStatus.Paused);
    }

    [Fact]
    public async Task ResumeAsync_FromPaused_MovesToActive()
    {
        var service = CreateService();
        var created = await service.CreateAsync(MakeRequest(), "test-user");
        await service.ActivateAsync(created.Id);
        await service.PauseAsync(created.Id);

        var result = await service.ResumeAsync(created.Id);

        result.PreviousStatus.Should().Be(CampaignStatus.Paused);
        result.NewStatus.Should().Be(CampaignStatus.Active);
    }

    [Fact]
    public async Task CancelAsync_FromActive_MovesToCancelled()
    {
        var service = CreateService();
        var created = await service.CreateAsync(MakeRequest(), "test-user");
        await service.ActivateAsync(created.Id);

        var cancelled = await service.CancelAsync(created.Id);

        cancelled.Status.Should().Be(CampaignStatus.Cancelled);
        cancelled.CompletedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task CancelAsync_AlreadyCancelled_ThrowsInvalidOperationException()
    {
        var service = CreateService();
        var created = await service.CreateAsync(MakeRequest(), "test-user");
        await service.ActivateAsync(created.Id);
        await service.CancelAsync(created.Id);

        var act = () => service.CancelAsync(created.Id);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Cannot cancel*Cancelled*");
    }

    [Fact]
    public async Task TriggerNowAsync_WithRecipients_UpdatesSentCount()
    {
        var service = CreateService();
        var request = MakeRequest();
        var created = await service.CreateAsync(request, "test-user");
        await service.ActivateAsync(created.Id);

        var result = await service.TriggerNowAsync(created.Id);

        result.RecipientsQueued.Should().Be(0);
        result.Status.Should().Be("Queued");
    }

    [Fact]
    public async Task InvalidTransition_ActivateFromActive_Throws()
    {
        var service = CreateService();
        var created = await service.CreateAsync(MakeRequest(), "test-user");
        await service.ActivateAsync(created.Id);

        var act = () => service.ActivateAsync(created.Id);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task InvalidTransition_PauseFromDraft_Throws()
    {
        var service = CreateService();
        var created = await service.CreateAsync(MakeRequest(), "test-user");

        var act = () => service.PauseAsync(created.Id);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task InvalidTransition_ResumeFromActive_Throws()
    {
        var service = CreateService();
        var created = await service.CreateAsync(MakeRequest(), "test-user");
        await service.ActivateAsync(created.Id);

        var act = () => service.ResumeAsync(created.Id);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task Operation_OnNonexistentCampaign_ThrowsKeyNotFound()
    {
        var service = CreateService();
        var id = Guid.NewGuid();

        var act = () => service.ActivateAsync(id);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
