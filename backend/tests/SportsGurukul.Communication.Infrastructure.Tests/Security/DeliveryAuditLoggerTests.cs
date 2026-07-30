using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces.Notification;
using SportsGurukul.Domain.Entities.Notification;
using SportsGurukul.Platform.Communication.Abstractions;
using SportsGurukul.Platform.Communication.Security;
using SportsGurukul.Communication.Infrastructure.Tests.Fixtures;

namespace SportsGurukul.Communication.Infrastructure.Tests.Security;

public class DeliveryAuditLoggerTests
{
    private readonly Mock<IAuditRepository> _auditRepo;
    private readonly DataMasker _dataMasker;
    private readonly Mock<ILogger<DeliveryAuditLogger>> _logger;
    private readonly DeliveryAuditLogger _auditLogger;

    public DeliveryAuditLoggerTests()
    {
        _auditRepo = new Mock<IAuditRepository>();
        _dataMasker = new DataMasker();
        _logger = new Mock<ILogger<DeliveryAuditLogger>>();
        _auditLogger = new DeliveryAuditLogger(_auditRepo.Object, _dataMasker, _logger.Object);
    }

    [Fact]
    public async Task LogDispatch_RecordsDeliveryAudit()
    {
        var notification = TestDataFactory.CreateNotification();
        var recipient = TestDataFactory.CreateRecipient();
        var result = new DeliveryResultBuilder().Success("msg-1").Build();

        _auditRepo.Setup(r => r.AddAsync(It.IsAny<NotificationAudit>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((NotificationAudit a, CancellationToken _) => a);

        await _auditLogger.LogDispatch(notification, recipient, result, CancellationToken.None);

        _auditRepo.Verify(r => r.AddAsync(
            It.Is<NotificationAudit>(a =>
                a.EntityType == "NotificationDelivery" &&
                a.EntityId == notification.Id &&
                a.Action == "DispatchSuccess"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task LogDispatch_RecordsFailureAudit()
    {
        var notification = TestDataFactory.CreateNotification();
        var recipient = TestDataFactory.CreateRecipient();
        var result = new DeliveryResultBuilder().Failure("Network error", "TIMEOUT").Build();

        _auditRepo.Setup(r => r.AddAsync(It.IsAny<NotificationAudit>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((NotificationAudit a, CancellationToken _) => a);

        await _auditLogger.LogDispatch(notification, recipient, result, CancellationToken.None);

        _auditRepo.Verify(r => r.AddAsync(
            It.Is<NotificationAudit>(a =>
                a.Action == "DispatchFailed" &&
                a.NewValue!.Contains("Network error")),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task LogDispatch_IncludesMaskedRecipientAddress()
    {
        var notification = TestDataFactory.CreateNotification();
        var recipient = TestDataFactory.CreateRecipient(destinationAddress: "john.doe@example.com");
        var result = new DeliveryResultBuilder().Success().Build();

        _auditRepo.Setup(r => r.AddAsync(It.IsAny<NotificationAudit>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((NotificationAudit a, CancellationToken _) => a);

        await _auditLogger.LogDispatch(notification, recipient, result, CancellationToken.None);

        _auditRepo.Verify(r => r.AddAsync(
            It.Is<NotificationAudit>(a =>
                a.NewValue!.Contains("j***e@example.com") &&
                !a.NewValue!.Contains("john.doe@example.com")),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task LogDispatch_HandlesException_Gracefully()
    {
        var notification = TestDataFactory.CreateNotification();
        var recipient = TestDataFactory.CreateRecipient();
        var result = new DeliveryResultBuilder().Success().Build();

        _auditRepo.Setup(r => r.AddAsync(It.IsAny<NotificationAudit>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("DB error"));

        await _auditLogger.LogDispatch(notification, recipient, result, CancellationToken.None);

        _logger.Verify(l => l.Log(
            LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("audit log")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }

    [Fact]
    public async Task LogQueueAction_RecordsQueueAudit()
    {
        var notificationId = Guid.NewGuid();

        _auditRepo.Setup(r => r.AddAsync(It.IsAny<NotificationAudit>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((NotificationAudit a, CancellationToken _) => a);

        await _auditLogger.LogQueueAction(notificationId, "Enqueued", "Priority: High", CancellationToken.None);

        _auditRepo.Verify(r => r.AddAsync(
            It.Is<NotificationAudit>(a =>
                a.EntityType == "NotificationQueue" &&
                a.EntityId == notificationId &&
                a.Action == "Enqueued" &&
                a.NewValue == "Priority: High"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task LogQueueAction_HandlesNullDetails()
    {
        var notificationId = Guid.NewGuid();

        _auditRepo.Setup(r => r.AddAsync(It.IsAny<NotificationAudit>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((NotificationAudit a, CancellationToken _) => a);

        await _auditLogger.LogQueueAction(notificationId, "Dequeued", null, CancellationToken.None);

        _auditRepo.Verify(r => r.AddAsync(
            It.Is<NotificationAudit>(a =>
                a.EntityType == "NotificationQueue" &&
                a.Action == "Dequeued" &&
                a.NewValue == null),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task LogDispatch_IncludesDurationMs()
    {
        var notification = TestDataFactory.CreateNotification();
        var recipient = TestDataFactory.CreateRecipient();
        var result = new DeliveryResultBuilder().Success("msg-1").WithDurationMs(250).Build();

        _auditRepo.Setup(r => r.AddAsync(It.IsAny<NotificationAudit>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((NotificationAudit a, CancellationToken _) => a);

        await _auditLogger.LogDispatch(notification, recipient, result, CancellationToken.None);

        _auditRepo.Verify(r => r.AddAsync(
            It.Is<NotificationAudit>(a =>
                a.NewValue!.Contains("250")),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task LogDispatch_CreatesAuditWithTimestamp()
    {
        var notification = TestDataFactory.CreateNotification();
        var recipient = TestDataFactory.CreateRecipient();
        var result = new DeliveryResultBuilder().Success().Build();
        var beforeTest = DateTime.UtcNow;

        _auditRepo.Setup(r => r.AddAsync(It.IsAny<NotificationAudit>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((NotificationAudit a, CancellationToken _) => a);

        await _auditLogger.LogDispatch(notification, recipient, result, CancellationToken.None);

        _auditRepo.Verify(r => r.AddAsync(
            It.Is<NotificationAudit>(a => a.ChangedAt >= beforeTest),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
