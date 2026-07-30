using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.Commands.Campaign;

namespace SportsGurukul.Communication.Application.Tests.Commands.Campaign;

public class ScheduleCampaignCommandHandlerTests
{
    private readonly Mock<ICampaignService> _campaignServiceMock;
    private readonly ScheduleCampaignCommandHandler _handler;

    public ScheduleCampaignCommandHandlerTests()
    {
        _campaignServiceMock = new Mock<ICampaignService>();
        _handler = new ScheduleCampaignCommandHandler(_campaignServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldScheduleCampaign()
    {
        var command = new ScheduleCampaignCommand(Guid.NewGuid(), new DateTime(2026, 10, 15, 10, 0, 0, DateTimeKind.Utc));
        var expectedResult = Result<bool>.Success(true);

        _campaignServiceMock
            .Setup(s => s.ScheduleAsync(command.CampaignId, command.ScheduledAt, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
        _campaignServiceMock.Verify(s => s.ScheduleAsync(command.CampaignId, command.ScheduledAt, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldSetScheduledStartDate()
    {
        var scheduledAt = new DateTime(2026, 11, 20, 14, 0, 0, DateTimeKind.Utc);
        var command = new ScheduleCampaignCommand(Guid.NewGuid(), scheduledAt);

        _campaignServiceMock
            .Setup(s => s.ScheduleAsync(command.CampaignId, command.ScheduledAt, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _campaignServiceMock.Verify(s => s.ScheduleAsync(
            command.CampaignId,
            It.Is<DateTime>(dt => dt == scheduledAt),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenCampaignNotFound_ShouldReturnFailureResult()
    {
        var command = new ScheduleCampaignCommand(Guid.NewGuid(), DateTime.UtcNow);
        var failureResult = Result<bool>.Failure("Campaign not found");

        _campaignServiceMock
            .Setup(s => s.ScheduleAsync(command.CampaignId, command.ScheduledAt, It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Campaign not found");
    }
}
