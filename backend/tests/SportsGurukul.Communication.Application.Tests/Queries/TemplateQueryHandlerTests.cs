using MediatR;
using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;
using SportsGurukul.Application.Features.NotificationManagement.Queries;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Communication.Application.Tests.Queries;

public class TemplateQueryHandlerTests
{
    private readonly Mock<ITemplateService> _templateServiceMock;
    private readonly TemplateQueryHandler _handler;
    private readonly TemplateVersionsQueryHandler _versionsHandler;

    public TemplateQueryHandlerTests()
    {
        _templateServiceMock = new Mock<ITemplateService>();
        _handler = new TemplateQueryHandler(_templateServiceMock.Object);
        _versionsHandler = new TemplateVersionsQueryHandler(_templateServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnTemplateById()
    {
        var id = Guid.NewGuid();
        var dto = new TemplateDto(id, "Welcome Email", "Welcome template", NotificationChannelType.Email,
            "Welcome {{name}}", "Hello {{name}}", true, 1, DateTime.UtcNow, [], []);

        _templateServiceMock.Setup(s => s.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<TemplateDto>.Success(dto));

        var result = await _handler.Handle(new TemplateQuery(id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(dto);
        result.Value!.Id.Should().Be(id);
        result.Value.Name.Should().Be("Welcome Email");
    }

    [Fact]
    public async Task Handle_ShouldReturnAllTemplates()
    {
        var templateId = Guid.NewGuid();
        var templates = new List<TemplateVersionDto>
        {
            new(Guid.NewGuid(), 1, "Subject v1", "Body v1", "Initial", DateTime.UtcNow),
            new(Guid.NewGuid(), 2, "Subject v2", "Body v2", "Updated", DateTime.UtcNow),
        };

        _templateServiceMock.Setup(s => s.GetVersionsAsync(templateId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<List<TemplateVersionDto>>.Success(templates));

        var result = await _versionsHandler.Handle(new TemplateVersionsQuery(templateId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenUnknownTemplate()
    {
        var id = Guid.NewGuid();
        _templateServiceMock.Setup(s => s.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<TemplateDto>.Failure($"Template {id} not found"));

        var result = await _handler.Handle(new TemplateQuery(id), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not found");
    }
}
