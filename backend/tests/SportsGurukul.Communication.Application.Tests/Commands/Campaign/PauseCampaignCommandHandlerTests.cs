using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.Commands.Campaign;

namespace SportsGurukul.Communication.Application.Tests.Commands.Campaign;

public class PauseCampaignCommandHandlerTests
{
    private readonly Mock<ICampaignService> _campaignServiceMock;
    private readonly PauseCampaignCommandHandler _handler;

    public PauseCampaignCommandHandlerTests()
    {
        _campaignServiceMock = new Mock<ICampaignService>();
        _handler = new PauseCampaignCommandHandler(_campaignServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldPauseActiveCampaign()
    {
        var command = new PauseCampaignCommand(Guid.NewGuid());
        var expectedResult = Result<bool>.Success(true);

        _campaignServiceMock
            .Setup(s => s.PauseAsync(command.CampaignId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
        _campaignServiceMock.Verify(s => s.PauseAsync(command.CampaignId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldUpdateStatusToPaused()
    {
        var command = new PauseCampaignCommand(Guid.NewGuid());

        _campaignServiceMock
            .Setup(s => s.PauseAsync(command.CampaignId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _campaignServiceMock.Verify(s => s.PauseAsync(command.CampaignId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenAlreadyPaused_ShouldReturnFailureResult()
    {
        var command = new PauseCampaignCommand(Guid.NewGuid());
        var failureResult = Result<bool>.Failure("Campaign is already paused");

        _campaignServiceMock
            .Setup(s => s.PauseAsync(command.CampaignId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Campaign is already paused");
    }
}
