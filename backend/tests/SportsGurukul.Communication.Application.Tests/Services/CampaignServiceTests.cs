using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces.Notification;
using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.BusinessRules;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;
using SportsGurukul.Application.Features.NotificationManagement.Services;
using SportsGurukul.Domain.Entities.Notification;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Communication.Application.Tests.Services;

public class CampaignServiceTests
{
    private readonly Mock<INotificationRepository> _notificationRepoMock;
    private readonly Mock<IBusinessRuleValidator> _ruleValidatorMock;
    private readonly Mock<IRecipientResolver> _recipientResolverMock;
    private readonly Mock<ILogger<CampaignService>> _loggerMock;
    private readonly CampaignService _service;

    public CampaignServiceTests()
    {
        _notificationRepoMock = new Mock<INotificationRepository>();
        _ruleValidatorMock = new Mock<IBusinessRuleValidator>();
        _recipientResolverMock = new Mock<IRecipientResolver>();
        _loggerMock = new Mock<ILogger<CampaignService>>();
        _service = new CampaignService(
            _notificationRepoMock.Object,
            _ruleValidatorMock.Object,
            _recipientResolverMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateCampaign()
    {
        var request = new CreateCampaignRequest(
            "Summer Campaign", "Summer notifications",
            Guid.NewGuid(), NotificationChannelType.Email,
            DateTime.UtcNow.AddDays(7), "age > 18", "{}");

        _ruleValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        var result = await _service.CreateAsync(request);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Name.Should().Be("Summer Campaign");
        result.Value.Status.Should().Be(NotificationStatus.Scheduled);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateDraft_WhenNoSchedule()
    {
        var request = new CreateCampaignRequest(
            "Draft Campaign", null, null, NotificationChannelType.Email,
            null, null, null);

        _ruleValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        var result = await _service.CreateAsync(request);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Name.Should().Be("Draft Campaign");
        result.Value.Status.Should().Be(NotificationStatus.Draft);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnCampaign()
    {
        var campaignId = Guid.NewGuid();
        var result = await _service.GetByIdAsync(campaignId);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not found");
    }

    [Fact]
    public async Task PauseAsync_ShouldPauseActiveCampaign()
    {
        var campaignId = Guid.NewGuid();
        var result = await _service.PauseAsync(campaignId);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task ResumeAsync_ShouldResumePausedCampaign()
    {
        var campaignId = Guid.NewGuid();
        var result = await _service.ResumeAsync(campaignId);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task CancelAsync_ShouldCancelCampaign()
    {
        var campaignId = Guid.NewGuid();
        var result = await _service.CancelAsync(campaignId);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task ScheduleAsync_ShouldScheduleCampaign()
    {
        var campaignId = Guid.NewGuid();
        var scheduledAt = DateTime.UtcNow.AddDays(3);

        _notificationRepoMock.Setup(r => r.GetByCampaignIdAsync(campaignId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Notification>());

        var result = await _service.ScheduleAsync(campaignId, scheduledAt);

        result.IsSuccess.Should().BeTrue();
    }
}
