using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.Commands.Template;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Communication.Application.Tests.Commands.Template;

public class PublishTemplateCommandHandlerTests
{
    private readonly Mock<ITemplateService> _templateServiceMock;
    private readonly PublishTemplateCommandHandler _handler;

    public PublishTemplateCommandHandlerTests()
    {
        _templateServiceMock = new Mock<ITemplateService>();
        _handler = new PublishTemplateCommandHandler(_templateServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldPublishDraftTemplate()
    {
        var command = new PublishTemplateCommand(Guid.NewGuid());

        var expectedDto = new TemplateDto(
            command.Id, "Published Template", null,
            NotificationChannelType.Email, "Subject", "Body",
            true, 2, DateTime.UtcNow,
            new List<TemplateVersionDto>(), new List<TemplateVariableDto>()
        );

        var expectedResult = Result<TemplateDto>.Success(expectedDto);
        _templateServiceMock
            .Setup(s => s.PublishAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(expectedDto);
        _templateServiceMock.Verify(s => s.PublishAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenTemplateNotFound_ShouldReturnFailureResult()
    {
        var command = new PublishTemplateCommand(Guid.NewGuid());
        var failureResult = Result<TemplateDto>.Failure("Template not found");

        _templateServiceMock
            .Setup(s => s.PublishAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Template not found");
    }

    [Fact]
    public async Task Handle_WhenAlreadyPublished_ShouldReturnFailureResult()
    {
        var command = new PublishTemplateCommand(Guid.NewGuid());
        var failureResult = Result<TemplateDto>.Failure("Template is already published");

        _templateServiceMock
            .Setup(s => s.PublishAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Template is already published");
    }
}
