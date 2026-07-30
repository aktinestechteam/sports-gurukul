using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces.Notification;
using SportsGurukul.Application.Features.NotificationManagement.BusinessRules;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;
using SportsGurukul.Domain.Entities.Notification;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Communication.Application.Tests.BusinessRules;

public class DuplicateCheckRuleTests
{
    private readonly Mock<INotificationRepository> _repositoryMock;
    private readonly Mock<ILogger<DuplicateCheckRule>> _loggerMock;
    private readonly DuplicateCheckRule _rule;

    public DuplicateCheckRuleTests()
    {
        _repositoryMock = new Mock<INotificationRepository>();
        _loggerMock = new Mock<ILogger<DuplicateCheckRule>>();
        _rule = new DuplicateCheckRule(_repositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task ValidateAsync_WhenNoDuplicateExists_ReturnsSuccess()
    {
        var request = CreateRequest(externalId: "ext-123");
        _repositoryMock
            .Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Notification, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Notification>());

        var result = await _rule.ValidateAsync(request);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateAsync_WhenDuplicateExternalIdExists_ReturnsFailure()
    {
        var request = CreateRequest(externalId: "ext-123");
        var existing = new List<Notification>
        {
            new() { ExternalId = "ext-123", Id = Guid.NewGuid() }
        };
        _repositoryMock
            .Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Notification, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var result = await _rule.ValidateAsync(request);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("ext-123");
    }

    [Fact]
    public async Task ValidateAsync_WhenNotCreateNotificationRequest_ReturnsSuccess()
    {
        var request = new object();

        var result = await _rule.ValidateAsync(request);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateAsync_WhenExternalIdIsNullAndBatchIdIsNull_ReturnsSuccess()
    {
        var request = CreateRequest(externalId: null);

        var result = await _rule.ValidateAsync(request);

        result.IsSuccess.Should().BeTrue();
        _repositoryMock.Verify(
            r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Notification, bool>>>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ValidateAsync_WhenExternalIdIsEmptyAndBatchIdIsNull_ReturnsSuccess()
    {
        var request = CreateRequest(externalId: "");

        var result = await _rule.ValidateAsync(request);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateAsync_WhenDifferentExternalId_ReturnsSuccess()
    {
        var request = CreateRequest(externalId: "ext-456");
        _repositoryMock
            .Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Notification, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Notification>());

        var result = await _rule.ValidateAsync(request);

        result.IsSuccess.Should().BeTrue();
    }

    private static CreateNotificationRequest CreateRequest(string? externalId = null) =>
        new(
            TemplateId: null,
            ChannelId: Guid.NewGuid(),
            ProviderId: null,
            Priority: NotificationPriority.Normal,
            Subject: "Test Subject",
            Body: "Test Body",
            SenderId: null,
            ScheduledAt: null,
            BatchId: null,
            CampaignId: null,
            ExternalId: externalId,
            Metadata: null,
            Recipients: new List<CreateRecipientRequest>
            {
                new(UserId: Guid.NewGuid(), ChannelType: "Email", DestinationAddress: "test@example.com", RecipientName: null)
            },
            Attachments: null
        );
}
