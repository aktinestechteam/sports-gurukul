using MediatR;
using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;
using SportsGurukul.Application.Features.NotificationManagement.Queries;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Communication.Application.Tests.Queries;

public class PreferenceQueryHandlerTests
{
    private readonly Mock<IPreferenceService> _preferenceServiceMock;
    private readonly PreferenceQueryHandler _handler;

    public PreferenceQueryHandlerTests()
    {
        _preferenceServiceMock = new Mock<IPreferenceService>();
        _handler = new PreferenceQueryHandler(_preferenceServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnPreferenceByUserId()
    {
        var userId = Guid.NewGuid();
        var preferences = new List<PreferenceDto>
        {
            new(Guid.NewGuid(), userId, NotificationChannelType.Email, true, new TimeOnly(9, 0), new TimeOnly(18, 0), 10),
            new(Guid.NewGuid(), userId, NotificationChannelType.SMS, false, null, null, 5),
        };

        _preferenceServiceMock.Setup(s => s.GetByUserAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<List<PreferenceDto>>.Success(preferences));

        var result = await _handler.Handle(new PreferenceQuery(userId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value.Should().BeEquivalentTo(preferences);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoPreferenceExists()
    {
        var userId = Guid.NewGuid();

        _preferenceServiceMock.Setup(s => s.GetByUserAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<List<PreferenceDto>>.Success(new List<PreferenceDto>()));

        var result = await _handler.Handle(new PreferenceQuery(userId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }
}
