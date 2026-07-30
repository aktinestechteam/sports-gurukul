using MediatR;
using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;
using SportsGurukul.Application.Features.NotificationManagement.Queries;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Communication.Application.Tests.Queries;

public class CampaignQueryHandlerTests
{
    private readonly Mock<ICampaignService> _campaignServiceMock;
    private readonly CampaignQueryHandler _handler;

    public CampaignQueryHandlerTests()
    {
        _campaignServiceMock = new Mock<ICampaignService>();
        _handler = new CampaignQueryHandler(_campaignServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnCampaignById()
    {
        var id = Guid.NewGuid();
        var dto = new CampaignDto(id, "Summer Campaign", "Summer notifications",
            Guid.NewGuid(), NotificationChannelType.Email, NotificationStatus.Draft,
            null, null, null, "criteria", 0, 0, 0, null, DateTime.UtcNow);

        _campaignServiceMock.Setup(s => s.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<CampaignDto>.Success(dto));

        var result = await _handler.Handle(new CampaignQuery(id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(dto);
        result.Value!.Id.Should().Be(id);
        result.Value.Name.Should().Be("Summer Campaign");
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenUnknownCampaign()
    {
        var id = Guid.NewGuid();
        _campaignServiceMock.Setup(s => s.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<CampaignDto>.Failure($"Campaign {id} not found"));

        var result = await _handler.Handle(new CampaignQuery(id), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not found");
    }
}
