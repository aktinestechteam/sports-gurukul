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

public class TemplateServiceTests
{
    private readonly Mock<ITemplateRepository> _templateRepoMock;
    private readonly Mock<IBusinessRuleValidator> _ruleValidatorMock;
    private readonly Mock<ITemplateRenderer> _templateRendererMock;
    private readonly Mock<ILogger<TemplateService>> _loggerMock;
    private readonly TemplateService _service;

    public TemplateServiceTests()
    {
        _templateRepoMock = new Mock<ITemplateRepository>();
        _ruleValidatorMock = new Mock<IBusinessRuleValidator>();
        _templateRendererMock = new Mock<ITemplateRenderer>();
        _loggerMock = new Mock<ILogger<TemplateService>>();
        _service = new TemplateService(
            _templateRepoMock.Object,
            _ruleValidatorMock.Object,
            _templateRendererMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateTemplate()
    {
        var request = new CreateTemplateRequest(
            "Welcome Email", "Welcome template", NotificationChannelType.Email,
            "Welcome {{name}}", "Hello {{name}}",
            [new CreateTemplateVariableRequest("name", "User name", true, null, "string")]);

        _ruleValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        _templateRepoMock.Setup(r => r.GetByNameAsync(request.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync((NotificationTemplate?)null);

        NotificationTemplate? addedEntity = null;
        _templateRepoMock.Setup(r => r.AddAsync(It.IsAny<NotificationTemplate>(), It.IsAny<CancellationToken>()))
            .Callback<NotificationTemplate, CancellationToken>((e, _) => addedEntity = e)
            .ReturnsAsync((NotificationTemplate e, CancellationToken _) => e);

        var result = await _service.CreateAsync(request);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Name.Should().Be("Welcome Email");
        result.Value.CurrentVersion.Should().Be(1);
        addedEntity.Should().NotBeNull();
        addedEntity!.Versions.Should().HaveCount(1);
        addedEntity.Variables.Should().HaveCount(1);
    }

    [Fact]
    public async Task CreateAsync_ShouldFail_WhenNameExists()
    {
        var request = new CreateTemplateRequest("Welcome Email", "Desc", NotificationChannelType.Email, "Subj", "Body", null);

        _ruleValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        _templateRepoMock.Setup(r => r.GetByNameAsync("Welcome Email", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new NotificationTemplate { Id = Guid.NewGuid(), Name = "Welcome Email", SubjectTemplate = "Subj", BodyTemplate = "Body", ChannelType = NotificationChannelType.Email });

        var result = await _service.CreateAsync(request);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("already exists");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnTemplate()
    {
        var id = Guid.NewGuid();
        var entity = new NotificationTemplate
        {
            Id = id,
            Name = "Welcome",
            SubjectTemplate = "Subj",
            BodyTemplate = "Body",
            ChannelType = NotificationChannelType.Email,
            IsActive = true,
            CurrentVersion = 2,
            CreatedAt = DateTime.UtcNow,
            Versions =
            [
                new TemplateVersion { Id = Guid.NewGuid(), TemplateId = id, VersionNumber = 1, SubjectTemplate = "Subj v1", BodyTemplate = "Body v1", ChangeNotes = "Initial", PublishedAt = DateTime.UtcNow.AddDays(-1) },
                new TemplateVersion { Id = Guid.NewGuid(), TemplateId = id, VersionNumber = 2, SubjectTemplate = "Subj v2", BodyTemplate = "Body v2", ChangeNotes = "Updated", PublishedAt = DateTime.UtcNow },
            ]
        };

        _templateRepoMock.Setup(r => r.GetWithVersionsAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var result = await _service.GetByIdAsync(id);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Id.Should().Be(id);
        result.Value.Name.Should().Be("Welcome");
        result.Value.CurrentVersion.Should().Be(2);
        result.Value.Versions.Should().HaveCount(2);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateTemplateContent()
    {
        var id = Guid.NewGuid();
        var entity = new NotificationTemplate
        {
            Id = id,
            Name = "Old Name",
            SubjectTemplate = "Old Subj",
            BodyTemplate = "Old Body",
            ChannelType = NotificationChannelType.Email,
            IsActive = true,
            CurrentVersion = 1,
            CreatedAt = DateTime.UtcNow,
        };

        var request = new UpdateTemplateRequest(id, "New Name", "New Desc", "New Subj", "New Body", null);

        _templateRepoMock.Setup(r => r.GetWithVersionsAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        _ruleValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        var result = await _service.UpdateAsync(request);

        result.IsSuccess.Should().BeTrue();
        entity.Name.Should().Be("New Name");
        entity.Description.Should().Be("New Desc");
        entity.SubjectTemplate.Should().Be("New Subj");
        entity.BodyTemplate.Should().Be("New Body");
    }

    [Fact]
    public async Task CreateVersionAsync_ShouldCreateNewVersion()
    {
        var id = Guid.NewGuid();
        var entity = new NotificationTemplate
        {
            Id = id,
            Name = "Welcome",
            SubjectTemplate = "Old Subj",
            BodyTemplate = "Old Body",
            ChannelType = NotificationChannelType.Email,
            IsActive = true,
            CurrentVersion = 1,
            CreatedAt = DateTime.UtcNow,
            Versions = [new TemplateVersion { Id = Guid.NewGuid(), TemplateId = id, VersionNumber = 1, SubjectTemplate = "Old Subj", BodyTemplate = "Old Body", ChangeNotes = "Initial", PublishedAt = DateTime.UtcNow }]
        };

        var request = new CreateTemplateVersionRequest(id, "New Subj", "New Body", "Updated content");

        _templateRepoMock.Setup(r => r.GetWithVersionsAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var result = await _service.CreateVersionAsync(request);

        result.IsSuccess.Should().BeTrue();
        result.Value!.VersionNumber.Should().Be(2);
        entity.CurrentVersion.Should().Be(2);
        entity.SubjectTemplate.Should().Be("New Subj");
        entity.BodyTemplate.Should().Be("New Body");
        entity.Versions.Should().HaveCount(2);
    }

    [Fact]
    public async Task PublishAsync_ShouldPublishDraftTemplate()
    {
        var id = Guid.NewGuid();
        var entity = new NotificationTemplate
        {
            Id = id,
            Name = "Welcome",
            SubjectTemplate = "Subj",
            BodyTemplate = "Body",
            ChannelType = NotificationChannelType.Email,
            IsActive = false,
            CurrentVersion = 1,
            CreatedAt = DateTime.UtcNow,
            Versions = []
        };

        _templateRepoMock.Setup(r => r.GetWithVersionsAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var result = await _service.PublishAsync(id);

        result.IsSuccess.Should().BeTrue();
        entity.IsActive.Should().BeTrue();
        entity.CurrentVersion.Should().Be(2);
        entity.Versions.Should().HaveCount(1);
    }

    [Fact]
    public async Task ArchiveAsync_ShouldArchivePublishedTemplate()
    {
        var id = Guid.NewGuid();
        var entity = new NotificationTemplate
        {
            Id = id,
            Name = "Active",
            SubjectTemplate = "Subj",
            BodyTemplate = "Body",
            ChannelType = NotificationChannelType.Email,
            IsActive = true,
            CurrentVersion = 1,
            CreatedAt = DateTime.UtcNow,
        };

        _templateRepoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var result = await _service.ArchiveAsync(id);

        result.IsSuccess.Should().BeTrue();
        entity.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task ArchiveAsync_ShouldFail_WhenNotFound()
    {
        var id = Guid.NewGuid();
        _templateRepoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((NotificationTemplate?)null);

        var result = await _service.ArchiveAsync(id);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not found");
    }
}
