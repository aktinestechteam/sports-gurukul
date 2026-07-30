using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.Commands.Template;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Communication.Application.Tests.Commands.Template;

public class CreateTemplateCommandHandlerTests
{
    private readonly Mock<ITemplateService> _templateServiceMock;
    private readonly CreateTemplateCommandHandler _handler;

    public CreateTemplateCommandHandlerTests()
    {
        _templateServiceMock = new Mock<ITemplateService>();
        _handler = new CreateTemplateCommandHandler(_templateServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreateTemplateViaService()
    {
        var command = new CreateTemplateCommand(
            "Welcome Template",
            "Template for welcome emails",
            NotificationChannelType.Email,
            "Welcome {{name}}!",
            "Hello {{name}}, welcome to our platform.",
            null
        );

        var expectedDto = new TemplateDto(
            Guid.NewGuid(), command.Name, command.Description,
            command.ChannelType, command.SubjectTemplate, command.BodyTemplate,
            true, 1, DateTime.UtcNow,
            new List<TemplateVersionDto>(), new List<TemplateVariableDto>()
        );

        var expectedResult = Result<TemplateDto>.Success(expectedDto);
        _templateServiceMock
            .Setup(s => s.CreateAsync(It.IsAny<CreateTemplateRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(expectedDto);
        _templateServiceMock.Verify(s => s.CreateAsync(
            It.Is<CreateTemplateRequest>(r =>
                r.Name == command.Name &&
                r.Description == command.Description &&
                r.ChannelType == command.ChannelType &&
                r.SubjectTemplate == command.SubjectTemplate &&
                r.BodyTemplate == command.BodyTemplate &&
                r.Variables == command.Variables
            ),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldMapTemplateProperties()
    {
        var variables = new List<CreateTemplateVariableRequest>
        {
            new("name", "User display name", true, null, "string"),
            new("link", "Confirmation link", true, null, "string")
        };

        var command = new CreateTemplateCommand(
            "Verification Template",
            null,
            NotificationChannelType.SMS,
            "Verify your account",
            "Your code is {{code}}",
            variables
        );

        var expectedDto = new TemplateDto(
            Guid.NewGuid(), command.Name, command.Description,
            command.ChannelType, command.SubjectTemplate, command.BodyTemplate,
            true, 1, DateTime.UtcNow,
            new List<TemplateVersionDto>(), new List<TemplateVariableDto>()
        );

        var expectedResult = Result<TemplateDto>.Success(expectedDto);
        _templateServiceMock
            .Setup(s => s.CreateAsync(It.IsAny<CreateTemplateRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _templateServiceMock.Verify(s => s.CreateAsync(
            It.Is<CreateTemplateRequest>(r =>
                r.Name == "Verification Template" &&
                r.Description == null &&
                r.ChannelType == NotificationChannelType.SMS &&
                r.SubjectTemplate == "Verify your account" &&
                r.BodyTemplate == "Your code is {{code}}" &&
                r.Variables == variables
            ),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenServiceFails_ShouldReturnFailureResult()
    {
        var command = new CreateTemplateCommand(
            "Test Template",
            null,
            NotificationChannelType.Email,
            "Subject",
            "Body",
            null
        );

        var failureResult = Result<TemplateDto>.Failure("Template creation failed");
        _templateServiceMock
            .Setup(s => s.CreateAsync(It.IsAny<CreateTemplateRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Template creation failed");
    }
}
