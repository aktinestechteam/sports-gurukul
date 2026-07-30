using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.Commands.Campaign;

namespace SportsGurukul.Communication.Application.Tests.Commands.Campaign;

public class ResumeCampaignCommandHandlerTests
{
    private readonly Mock<ICampaignService> _campaignServiceMock;
    private readonly ResumeCampaignCommandHandler _handler;

    public ResumeCampaignCommandHandlerTests()
    {
        _campaignServiceMock = new Mock<ICampaignService>();
        _handler = new ResumeCampaignCommandHandler(_campaignServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldResumePausedCampaign()
    {
        var command = new ResumeCampaignCommand(Guid.NewGuid());
        var expectedResult = Result<bool>.Success(true);

        _campaignServiceMock
            .Setup(s => s.ResumeAsync(command.CampaignId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
        _campaignServiceMock.Verify(s => s.ResumeAsync(command.CampaignId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldUpdateStatusToActive()
    {
        var command = new ResumeCampaignCommand(Guid.NewGuid());

        _campaignServiceMock
            .Setup(s => s.ResumeAsync(command.CampaignId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _campaignServiceMock.Verify(s => s.ResumeAsync(command.CampaignId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenNotPaused_ShouldReturnFailureResult()
    {
        var command = new ResumeCampaignCommand(Guid.NewGuid());
        var failureResult = Result<bool>.Failure("Campaign is not paused");

        _campaignServiceMock
            .Setup(s => s.ResumeAsync(command.CampaignId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Campaign is not paused");
    }
}
