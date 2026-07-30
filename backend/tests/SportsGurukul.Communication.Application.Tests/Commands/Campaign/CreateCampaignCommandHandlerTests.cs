using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.Commands.Campaign;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Communication.Application.Tests.Commands.Campaign;

public class CreateCampaignCommandHandlerTests
{
    private readonly Mock<ICampaignService> _campaignServiceMock;
    private readonly CreateCampaignCommandHandler _handler;

    public CreateCampaignCommandHandlerTests()
    {
        _campaignServiceMock = new Mock<ICampaignService>();
        _handler = new CreateCampaignCommandHandler(_campaignServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreateCampaignViaService()
    {
        var command = new CreateCampaignCommand(
            "Summer Sale",
            "Email campaign for summer sale",
            Guid.NewGuid(),
            NotificationChannelType.Email,
            null,
            "{\"age\":\"18-35\"}",
            "{\"campaign\":\"summer-sale-2026\"}"
        );

        var expectedDto = new CampaignDto(
            Guid.NewGuid(), command.Name, command.Description,
            command.TemplateId, command.ChannelType,
            NotificationStatus.Draft, null, null, null,
            command.TargetCriteria, 0, 0, 0,
            command.Metadata, DateTime.UtcNow
        );

        var expectedResult = Result<CampaignDto>.Success(expectedDto);
        _campaignServiceMock
            .Setup(s => s.CreateAsync(It.IsAny<CreateCampaignRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(expectedDto);
        _campaignServiceMock.Verify(s => s.CreateAsync(
            It.Is<CreateCampaignRequest>(r =>
                r.Name == command.Name &&
                r.Description == command.Description &&
                r.TemplateId == command.TemplateId &&
                r.ChannelType == command.ChannelType &&
                r.ScheduledAt == command.ScheduledAt &&
                r.TargetCriteria == command.TargetCriteria &&
                r.Metadata == command.Metadata
            ),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldMapCampaignProperties()
    {
        var command = new CreateCampaignCommand(
            "Welcome Series",
            null,
            null,
            NotificationChannelType.InAppNotification,
            null,
            null,
            null
        );

        var expectedDto = new CampaignDto(
            Guid.NewGuid(), command.Name, null,
            null, command.ChannelType,
            NotificationStatus.Draft, null, null, null,
            null, 0, 0, 0,
            null, DateTime.UtcNow
        );

        var expectedResult = Result<CampaignDto>.Success(expectedDto);
        _campaignServiceMock
            .Setup(s => s.CreateAsync(It.IsAny<CreateCampaignRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _campaignServiceMock.Verify(s => s.CreateAsync(
            It.Is<CreateCampaignRequest>(r =>
                r.Name == "Welcome Series" &&
                r.Description == null &&
                r.TemplateId == null &&
                r.ChannelType == NotificationChannelType.InAppNotification &&
                r.ScheduledAt == null &&
                r.TargetCriteria == null &&
                r.Metadata == null
            ),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Fact]
    public async Task Handle_WithSchedule_ShouldCreateScheduledCampaign()
    {
        var scheduledAt = new DateTime(2026, 12, 1, 8, 0, 0, DateTimeKind.Utc);
        var command = new CreateCampaignCommand(
            "Scheduled Promo",
            "A scheduled campaign",
            Guid.NewGuid(),
            NotificationChannelType.Email,
            scheduledAt,
            "{\"segment\":\"premium\"}",
            null
        );

        var expectedDto = new CampaignDto(
            Guid.NewGuid(), command.Name, command.Description,
            command.TemplateId, command.ChannelType,
            NotificationStatus.Scheduled, scheduledAt, null, null,
            command.TargetCriteria, 0, 0, 0,
            null, DateTime.UtcNow
        );

        var expectedResult = Result<CampaignDto>.Success(expectedDto);
        _campaignServiceMock
            .Setup(s => s.CreateAsync(It.IsAny<CreateCampaignRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _campaignServiceMock.Verify(s => s.CreateAsync(
            It.Is<CreateCampaignRequest>(r => r.ScheduledAt == scheduledAt),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }
}
