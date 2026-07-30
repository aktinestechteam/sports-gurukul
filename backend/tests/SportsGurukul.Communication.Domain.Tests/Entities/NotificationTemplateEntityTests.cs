using SportsGurukul.Domain.Entities.Notification;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Communication.Domain.Tests.Entities;

public class NotificationTemplateEntityTests
{
    [Fact]
    public void CreateTemplate_WithAllProperties_ShouldSetPropertiesCorrectly()
    {
        var id = Guid.NewGuid();
        var now = DateTime.UtcNow;

        var template = new NotificationTemplate
        {
            Id = id,
            Name = "Welcome Email",
            Description = "Template for welcome emails",
            ChannelType = NotificationChannelType.Email,
            SubjectTemplate = "Welcome {{UserName}}!",
            BodyTemplate = "<h1>Welcome {{UserName}}</h1><p>Thank you for joining.</p>",
            IsActive = true,
            CurrentVersion = 1,
            CreatedAt = now
        };

        template.Id.Should().Be(id);
        template.Name.Should().Be("Welcome Email");
        template.Description.Should().Be("Template for welcome emails");
        template.ChannelType.Should().Be(NotificationChannelType.Email);
        template.SubjectTemplate.Should().Be("Welcome {{UserName}}!");
        template.BodyTemplate.Should().Be("<h1>Welcome {{UserName}}</h1><p>Thank you for joining.</p>");
        template.IsActive.Should().BeTrue();
        template.CurrentVersion.Should().Be(1);
        template.CreatedAt.Should().Be(now);
    }

    [Fact]
    public void DefaultIsActive_ShouldBeTrue()
    {
        var template = new NotificationTemplate();

        template.IsActive.Should().BeTrue();
    }

    [Fact]
    public void DefaultCurrentVersion_ShouldBeZero()
    {
        var template = new NotificationTemplate();

        template.CurrentVersion.Should().Be(0);
    }

    [Fact]
    public void Template_WithVariables_ShouldInitializeCollection()
    {
        var template = new NotificationTemplate();

        template.Variables.Should().NotBeNull();
        template.Variables.Should().BeEmpty();
        template.Variables.Should().BeAssignableTo<ICollection<TemplateVariable>>();
    }

    [Fact]
    public void Template_WithVersions_ShouldInitializeCollection()
    {
        var template = new NotificationTemplate();

        template.Versions.Should().NotBeNull();
        template.Versions.Should().BeEmpty();
        template.Versions.Should().BeAssignableTo<ICollection<TemplateVersion>>();
    }

    [Fact]
    public void Template_WithNotifications_ShouldInitializeCollection()
    {
        var template = new NotificationTemplate();

        template.Notifications.Should().NotBeNull();
        template.Notifications.Should().BeEmpty();
        template.Notifications.Should().BeAssignableTo<ICollection<Notification>>();
    }

    [Fact]
    public void AddVariable_ToTemplate_ShouldAddToCollection()
    {
        var template = new NotificationTemplate();
        var variable = new TemplateVariable
        {
            TemplateId = template.Id,
            Name = "UserName",
            Description = "The user's display name",
            IsRequired = true,
            DataType = "string"
        };

        template.Variables.Add(variable);

        template.Variables.Should().ContainSingle();
        template.Variables.Should().Contain(variable);
    }

    [Fact]
    public void AddVersion_ToTemplate_ShouldAddToCollection()
    {
        var template = new NotificationTemplate();
        var version = new TemplateVersion
        {
            TemplateId = template.Id,
            VersionNumber = 1,
            SubjectTemplate = "Version 1 Subject",
            BodyTemplate = "Version 1 Body",
            PublishedAt = DateTime.UtcNow
        };

        template.Versions.Add(version);

        template.Versions.Should().ContainSingle();
        template.Versions.Should().Contain(version);
    }

    [Fact]
    public void Description_ShouldBeNull_WhenNotSet()
    {
        var template = new NotificationTemplate();

        template.Description.Should().BeNull();
    }

    [Fact]
    public void ChannelType_ShouldDefaultToEmail()
    {
        var template = new NotificationTemplate();

        template.ChannelType.Should().Be(NotificationChannelType.Email);
    }
}
