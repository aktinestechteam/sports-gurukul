using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces.Notification;
using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.BusinessRules;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;
using SportsGurukul.Application.Features.NotificationManagement.Services;
using SportsGurukul.Domain.Entities.Notification;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Communication.Application.Tests.Services;

public class NotificationServiceTests
{
    private readonly Mock<INotificationRepository> _notificationRepoMock;
    private readonly Mock<ITemplateRepository> _templateRepoMock;
    private readonly Mock<IPreferenceRepository> _preferenceRepoMock;
    private readonly Mock<IBusinessRuleValidator> _ruleValidatorMock;
    private readonly Mock<IPublisher> _publisherMock;
    private readonly Mock<ILogger<NotificationService>> _loggerMock;
    private readonly NotificationService _service;

    public NotificationServiceTests()
    {
        _notificationRepoMock = new Mock<INotificationRepository>();
        _templateRepoMock = new Mock<ITemplateRepository>();
        _preferenceRepoMock = new Mock<IPreferenceRepository>();
        _ruleValidatorMock = new Mock<IBusinessRuleValidator>();
        _publisherMock = new Mock<IPublisher>();
        _loggerMock = new Mock<ILogger<NotificationService>>();
        _service = new NotificationService(
            _notificationRepoMock.Object,
            _templateRepoMock.Object,
            _preferenceRepoMock.Object,
            _ruleValidatorMock.Object,
            _publisherMock.Object,
            _loggerMock.Object);
    }

    private static Notification CreateNotification(Guid id)
    {
        return new Notification
        {
            Id = id,
            Subject = "Test Subject",
            Body = "Test Body",
            ChannelId = Guid.NewGuid(),
            Channel = new NotificationChannel { Id = Guid.NewGuid(), Name = "Email", Code = "email", ChannelType = NotificationChannelType.Email },
            Priority = NotificationPriority.Normal,
            Status = NotificationStatus.Draft,
            SenderId = "user-1",
            CreatedAt = DateTime.UtcNow,
        };
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateNotificationViaRepository()
    {
        var request = new CreateNotificationRequest(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), NotificationPriority.High,
            "Welcome", "Hello {{name}}", "sender-1", null, null, null, null, null,
            [new CreateRecipientRequest(Guid.NewGuid(), "Email", "test@test.com", "Test")],
            [new CreateAttachmentRequest("file.pdf", "/path/file.pdf", "application/pdf", 1024, "local", null)]);

        _ruleValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        Notification? addedEntity = null;
        _notificationRepoMock.Setup(r => r.AddAsync(It.IsAny<Notification>(), It.IsAny<CancellationToken>()))
            .Callback<Notification, CancellationToken>((e, _) => addedEntity = e)
            .ReturnsAsync((Notification e, CancellationToken _) => e);

        var result = await _service.CreateAsync(request);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Subject.Should().Be("Welcome");
        result.Value.Priority.Should().Be(NotificationPriority.High);
        addedEntity.Should().NotBeNull();
        addedEntity!.Subject.Should().Be("Welcome");
        addedEntity.Status.Should().Be(NotificationStatus.Draft);
    }

    [Fact]
    public async Task CreateAsync_ShouldValidateInput()
    {
        var request = new CreateNotificationRequest(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), NotificationPriority.Normal,
            "Subject", "Body", null, null, null, null, null, null,
            [new CreateRecipientRequest(Guid.NewGuid(), "Email", "test@test.com", null)], null);

        _ruleValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Failure("Subject is required"));

        var result = await _service.CreateAsync(request);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Subject is required");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNotification()
    {
        var id = Guid.NewGuid();
        var entity = CreateNotification(id);

        _notificationRepoMock.Setup(r => r.GetByIdWithDetailsAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var result = await _service.GetByIdAsync(id);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Id.Should().Be(id);
        result.Value.Subject.Should().Be("Test Subject");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
    {
        var id = Guid.NewGuid();
        _notificationRepoMock.Setup(r => r.GetByIdWithDetailsAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Notification?)null);

        var result = await _service.GetByIdAsync(id);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not found");
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateExistingNotification()
    {
        var id = Guid.NewGuid();
        var entity = CreateNotification(id);
        var request = new UpdateNotificationRequest(id, "Updated Subject", "Updated Body", NotificationPriority.High, Guid.NewGuid(), null, null);

        _notificationRepoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        _ruleValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        var result = await _service.UpdateAsync(request);

        result.IsSuccess.Should().BeTrue();
        entity.Subject.Should().Be("Updated Subject");
        entity.Body.Should().Be("Updated Body");
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrow_WhenNotFound()
    {
        var id = Guid.NewGuid();
        var request = new UpdateNotificationRequest(id, "Updated", "Body", NotificationPriority.Low, null, null, null);

        _notificationRepoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Notification?)null);

        var result = await _service.UpdateAsync(request);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not found");
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveNotification()
    {
        var id = Guid.NewGuid();
        var entity = CreateNotification(id);

        _notificationRepoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var result = await _service.DeleteAsync(id);

        result.IsSuccess.Should().BeTrue();
        entity.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFailure_WhenNotFound()
    {
        var id = Guid.NewGuid();
        _notificationRepoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Notification?)null);

        var result = await _service.DeleteAsync(id);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not found");
    }

    [Fact]
    public async Task CancelAsync_ShouldCancelDraft()
    {
        var id = Guid.NewGuid();
        var entity = CreateNotification(id);
        entity.Status = NotificationStatus.Draft;

        _notificationRepoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var result = await _service.CancelAsync(id);

        result.IsSuccess.Should().BeTrue();
        entity.Status.Should().Be(NotificationStatus.Cancelled);
    }

    [Fact]
    public async Task CancelAsync_ShouldFail_WhenAlreadySent()
    {
        var id = Guid.NewGuid();
        var entity = CreateNotification(id);
        entity.Status = NotificationStatus.Sent;

        _notificationRepoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var result = await _service.CancelAsync(id);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Cannot cancel");
    }
}
