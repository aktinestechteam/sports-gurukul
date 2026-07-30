using SportsGurukul.Domain.Entities.Notification;

namespace SportsGurukul.Communication.Domain.Tests.Entities;

public class TemplateVersionEntityTests
{
    [Fact]
    public void CreateVersion_WithTemplateReference_ShouldSetPropertiesCorrectly()
    {
        var templateId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        var version = new TemplateVersion
        {
            Id = Guid.NewGuid(),
            TemplateId = templateId,
            VersionNumber = 2,
            SubjectTemplate = "Updated Subject",
            BodyTemplate = "Updated Body",
            ChangeNotes = "Fixed typo in greeting",
            PublishedAt = now
        };

        version.TemplateId.Should().Be(templateId);
        version.Template.Should().BeNull();
    }

    [Fact]
    public void CreateVersion_WithTemplateNavigation_ShouldSetNavigationProperty()
    {
        var template = new NotificationTemplate
        {
            Id = Guid.NewGuid(),
            Name = "Test Template"
        };
        var now = DateTime.UtcNow;

        var version = new TemplateVersion
        {
            TemplateId = template.Id,
            VersionNumber = 1,
            SubjectTemplate = "Subject",
            BodyTemplate = "Body",
            PublishedAt = now,
            Template = template
        };

        version.Template.Should().NotBeNull();
        version.Template.Should().Be(template);
        version.Template.Name.Should().Be("Test Template");
    }

    [Fact]
    public void VersionNumber_ShouldBeAssignable()
    {
        var version = new TemplateVersion();

        version.VersionNumber = 5;

        version.VersionNumber.Should().Be(5);
    }

    [Fact]
    public void VersionNumber_Default_ShouldBeZero()
    {
        var version = new TemplateVersion();

        version.VersionNumber.Should().Be(0);
    }

    [Fact]
    public void SubjectTemplate_ShouldStoreContent()
    {
        var version = new TemplateVersion
        {
            SubjectTemplate = "Hello {{Name}}, your order #{{OrderId}} is confirmed"
        };

        version.SubjectTemplate.Should().Be("Hello {{Name}}, your order #{{OrderId}} is confirmed");
    }

    [Fact]
    public void BodyTemplate_ShouldStoreContent()
    {
        var version = new TemplateVersion
        {
            BodyTemplate = "<p>Dear {{Name}},</p><p>Thank you for your order.</p>"
        };

        version.BodyTemplate.Should().Be("<p>Dear {{Name}},</p><p>Thank you for your order.</p>");
    }

    [Fact]
    public void ChangeNotes_ShouldStoreNotes()
    {
        var version = new TemplateVersion
        {
            ChangeNotes = "Updated pricing information in template"
        };

        version.ChangeNotes.Should().Be("Updated pricing information in template");
    }

    [Fact]
    public void ChangeNotes_ShouldBeNull_WhenNotSet()
    {
        var version = new TemplateVersion();

        version.ChangeNotes.Should().BeNull();
    }

    [Fact]
    public void PublishedAt_ShouldStoreTimestamp()
    {
        var now = DateTime.UtcNow;

        var version = new TemplateVersion { PublishedAt = now };

        version.PublishedAt.Should().Be(now);
    }

    [Fact]
    public void MultipleVersions_ShouldHaveDifferentVersionNumbers()
    {
        var templateId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        var version1 = new TemplateVersion
        {
            TemplateId = templateId,
            VersionNumber = 1,
            SubjectTemplate = "v1 Subject",
            BodyTemplate = "v1 Body",
            PublishedAt = now
        };

        var version2 = new TemplateVersion
        {
            TemplateId = templateId,
            VersionNumber = 2,
            SubjectTemplate = "v2 Subject",
            BodyTemplate = "v2 Body",
            PublishedAt = now.AddHours(1)
        };

        version1.VersionNumber.Should().Be(1);
        version2.VersionNumber.Should().Be(2);
        version1.VersionNumber.Should().BeLessThan(version2.VersionNumber);
    }
}
