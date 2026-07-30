using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.Commands.Preference;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Communication.Application.Tests.Commands.Preference;

public class UpdatePreferenceCommandHandlerTests
{
    private readonly Mock<IPreferenceService> _preferenceServiceMock;
    private readonly UpdatePreferenceCommandHandler _handler;

    public UpdatePreferenceCommandHandlerTests()
    {
        _preferenceServiceMock = new Mock<IPreferenceService>();
        _handler = new UpdatePreferenceCommandHandler(_preferenceServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldUpdatePreferenceViaService()
    {
        var command = new UpdatePreferenceCommand(
            Guid.NewGuid(),
            NotificationChannelType.Email,
            true,
            new TimeOnly(9, 0),
            new TimeOnly(17, 0),
            50
        );

        var expectedDto = new PreferenceDto(
            Guid.NewGuid(), command.UserId, command.ChannelType,
            command.IsEnabled!.Value, command.QuietHoursStart,
            command.QuietHoursEnd, command.MaxPerDay
        );

        var expectedResult = Result<PreferenceDto>.Success(expectedDto);
        _preferenceServiceMock
            .Setup(s => s.UpdateAsync(It.IsAny<UpdatePreferenceRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(expectedDto);
        _preferenceServiceMock.Verify(s => s.UpdateAsync(
            It.Is<UpdatePreferenceRequest>(r =>
                r.UserId == command.UserId &&
                r.ChannelType == command.ChannelType &&
                r.IsEnabled == command.IsEnabled &&
                r.QuietHoursStart == command.QuietHoursStart &&
                r.QuietHoursEnd == command.QuietHoursEnd &&
                r.MaxPerDay == command.MaxPerDay
            ),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldMapAllPreferenceProperties()
    {
        var command = new UpdatePreferenceCommand(
            Guid.NewGuid(),
            NotificationChannelType.SMS,
            false,
            null,
            null,
            null
        );

        var expectedDto = new PreferenceDto(
            Guid.NewGuid(), command.UserId, command.ChannelType,
            false, null, null, null
        );

        var expectedResult = Result<PreferenceDto>.Success(expectedDto);
        _preferenceServiceMock
            .Setup(s => s.UpdateAsync(It.IsAny<UpdatePreferenceRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _preferenceServiceMock.Verify(s => s.UpdateAsync(
            It.Is<UpdatePreferenceRequest>(r =>
                r.UserId == command.UserId &&
                r.ChannelType == NotificationChannelType.SMS &&
                r.IsEnabled == false &&
                r.QuietHoursStart == null &&
                r.QuietHoursEnd == null &&
                r.MaxPerDay == null
            ),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Fact]
    public async Task Handle_WithPartialUpdate_ShouldUpdateOnlyProvidedProperties()
    {
        var command = new UpdatePreferenceCommand(
            Guid.NewGuid(),
            NotificationChannelType.PushNotification,
            null,
            new TimeOnly(22, 0),
            new TimeOnly(8, 0),
            100
        );

        var expectedDto = new PreferenceDto(
            Guid.NewGuid(), command.UserId, command.ChannelType,
            true, command.QuietHoursStart, command.QuietHoursEnd, command.MaxPerDay
        );

        var expectedResult = Result<PreferenceDto>.Success(expectedDto);
        _preferenceServiceMock
            .Setup(s => s.UpdateAsync(It.IsAny<UpdatePreferenceRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _preferenceServiceMock.Verify(s => s.UpdateAsync(
            It.Is<UpdatePreferenceRequest>(r =>
                r.UserId == command.UserId &&
                r.ChannelType == NotificationChannelType.PushNotification &&
                r.IsEnabled == null &&
                r.QuietHoursStart == new TimeOnly(22, 0) &&
                r.QuietHoursEnd == new TimeOnly(8, 0) &&
                r.MaxPerDay == 100
            ),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }
}
