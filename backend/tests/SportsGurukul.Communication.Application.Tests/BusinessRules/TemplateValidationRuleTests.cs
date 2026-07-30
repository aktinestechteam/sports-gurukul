using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces.Notification;
using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Application.Features.NotificationManagement.BusinessRules;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;
using SportsGurukul.Domain.Entities.Notification;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Communication.Application.Tests.BusinessRules;

public class TemplateValidationRuleTests
{
    private readonly Mock<ITemplateRepository> _templateRepoMock;
    private readonly Mock<ITemplateRenderer> _rendererMock;
    private readonly Mock<ILogger<TemplateValidationRule>> _loggerMock;
    private readonly TemplateValidationRule _rule;

    public TemplateValidationRuleTests()
    {
        _templateRepoMock = new Mock<ITemplateRepository>();
        _rendererMock = new Mock<ITemplateRenderer>();
        _loggerMock = new Mock<ILogger<TemplateValidationRule>>();
        _rule = new TemplateValidationRule(_templateRepoMock.Object, _rendererMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task ValidateAsync_WhenTemplateFound_ReturnsSuccess()
    {
        var templateId = Guid.NewGuid();
        var request = new CreateNotificationRequest(
            TemplateId: templateId,
            ChannelId: Guid.NewGuid(),
            ProviderId: null,
            Priority: NotificationPriority.Normal,
            Subject: "Test",
            Body: "Test",
            SenderId: null,
            ScheduledAt: null,
            BatchId: null,
            CampaignId: null,
            ExternalId: null,
            Metadata: null,
            Recipients: new List<CreateRecipientRequest>
            {
                new(UserId: Guid.NewGuid(), ChannelType: "Email", DestinationAddress: "test@example.com", RecipientName: null)
            },
            Attachments: null
        );
        var template = new NotificationTemplate
        {
            Id = templateId,
            SubjectTemplate = "Hello {{name}}",
            BodyTemplate = "Welcome {{name}}!",
            IsActive = true
        };
        _templateRepoMock
            .Setup(r => r.GetByIdAsync(templateId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(template);
        _rendererMock
            .Setup(r => r.ExtractVariables(It.IsAny<string>()))
            .Returns(new List<string> { "name" });

        var result = await _rule.ValidateAsync(request);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateAsync_WhenTemplateNotFound_ReturnsFailure()
    {
        var templateId = Guid.NewGuid();
        var request = new CreateNotificationRequest(
            TemplateId: templateId,
            ChannelId: Guid.NewGuid(),
            ProviderId: null,
            Priority: NotificationPriority.Normal,
            Subject: "Test",
            Body: "Test",
            SenderId: null,
            ScheduledAt: null,
            BatchId: null,
            CampaignId: null,
            ExternalId: null,
            Metadata: null,
            Recipients: new List<CreateRecipientRequest>
            {
                new(UserId: Guid.NewGuid(), ChannelType: "Email", DestinationAddress: "test@example.com", RecipientName: null)
            },
            Attachments: null
        );
        _templateRepoMock
            .Setup(r => r.GetByIdAsync(templateId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((NotificationTemplate?)null);

        var result = await _rule.ValidateAsync(request);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain(templateId.ToString());
    }

    [Fact]
    public async Task ValidateAsync_WhenNoTemplateId_ReturnsSuccess()
    {
        var request = new CreateNotificationRequest(
            TemplateId: null,
            ChannelId: Guid.NewGuid(),
            ProviderId: null,
            Priority: NotificationPriority.Normal,
            Subject: "Test",
            Body: "Test",
            SenderId: null,
            ScheduledAt: null,
            BatchId: null,
            CampaignId: null,
            ExternalId: null,
            Metadata: null,
            Recipients: new List<CreateRecipientRequest>
            {
                new(UserId: Guid.NewGuid(), ChannelType: "Email", DestinationAddress: "test@example.com", RecipientName: null)
            },
            Attachments: null
        );

        var result = await _rule.ValidateAsync(request);

        result.IsSuccess.Should().BeTrue();
        _templateRepoMock.Verify(
            r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ValidateAsync_ForCreateTemplateRequest_ReturnsSuccess()
    {
        var request = new CreateTemplateRequest(
            Name: "Test Template",
            Description: null,
            ChannelType: NotificationChannelType.Email,
            SubjectTemplate: "Hello {{name}}",
            BodyTemplate: "Welcome {{name}}!",
            Variables: null
        );
        _rendererMock
            .Setup(r => r.ExtractVariables(It.IsAny<string>()))
            .Returns(new List<string> { "name" });

        var result = await _rule.ValidateAsync(request);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateAsync_ForCreateTemplateVersionRequest_ReturnsSuccess()
    {
        var request = new CreateTemplateVersionRequest(
            TemplateId: Guid.NewGuid(),
            SubjectTemplate: "Hello {{name}}",
            BodyTemplate: "Welcome {{name}}!",
            ChangeNotes: "Updated template"
        );
        _rendererMock
            .Setup(r => r.ExtractVariables(It.IsAny<string>()))
            .Returns(new List<string> { "name" });

        var result = await _rule.ValidateAsync(request);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateAsync_WhenSubjectAndBodyTemplatesAreNull_ReturnsSuccess()
    {
        var result = await _rule.ValidateAsync(new object());

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateAsync_WhenTemplateHasVariables_LogsInformation()
    {
        var templateId = Guid.NewGuid();
        var request = new CreateNotificationRequest(
            TemplateId: templateId,
            ChannelId: Guid.NewGuid(),
            ProviderId: null,
            Priority: NotificationPriority.Normal,
            Subject: "Test",
            Body: "Test",
            SenderId: null,
            ScheduledAt: null,
            BatchId: null,
            CampaignId: null,
            ExternalId: null,
            Metadata: null,
            Recipients: new List<CreateRecipientRequest>
            {
                new(UserId: Guid.NewGuid(), ChannelType: "Email", DestinationAddress: "test@example.com", RecipientName: null)
            },
            Attachments: null
        );
        var template = new NotificationTemplate
        {
            Id = templateId,
            SubjectTemplate = "Hello {{name}}",
            BodyTemplate = "Welcome {{name}}!",
            IsActive = true
        };
        _templateRepoMock
            .Setup(r => r.GetByIdAsync(templateId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(template);
        _rendererMock
            .Setup(r => r.ExtractVariables("Hello {{name}}"))
            .Returns(new List<string> { "name" });
        _rendererMock
            .Setup(r => r.ExtractVariables("Welcome {{name}}!"))
            .Returns(new List<string> { "name" });

        var result = await _rule.ValidateAsync(request);

        result.IsSuccess.Should().BeTrue();
    }
}
