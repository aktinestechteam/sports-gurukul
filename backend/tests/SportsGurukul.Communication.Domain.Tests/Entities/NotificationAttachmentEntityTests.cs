using SportsGurukul.Domain.Entities.Notification;

namespace SportsGurukul.Communication.Domain.Tests.Entities;

public class NotificationAttachmentEntityTests
{
    [Fact]
    public void CreateAttachment_WithFileMetadata_ShouldSetPropertiesCorrectly()
    {
        var notificationId = Guid.NewGuid();
        var documentId = Guid.NewGuid();

        var attachment = new NotificationAttachment
        {
            Id = Guid.NewGuid(),
            NotificationId = notificationId,
            FileName = "invoice.pdf",
            FilePath = "/uploads/notifications/invoice_20260730.pdf",
            ContentType = "application/pdf",
            FileSize = 1024 * 50,
            StorageType = "local",
            DocumentId = documentId,
            CreatedAt = DateTime.UtcNow
        };

        attachment.NotificationId.Should().Be(notificationId);
        attachment.FileName.Should().Be("invoice.pdf");
        attachment.FilePath.Should().Be("/uploads/notifications/invoice_20260730.pdf");
        attachment.ContentType.Should().Be("application/pdf");
        attachment.FileSize.Should().Be(51200);
        attachment.StorageType.Should().Be("local");
        attachment.DocumentId.Should().Be(documentId);
    }

    [Fact]
    public void ContentType_ShouldSupportPdf()
    {
        var attachment = new NotificationAttachment
        {
            NotificationId = Guid.NewGuid(),
            FileName = "document.pdf",
            FilePath = "/path/to/document.pdf",
            ContentType = "application/pdf",
            FileSize = 2048
        };

        attachment.ContentType.Should().Be("application/pdf");
    }

    [Fact]
    public void ContentType_ShouldSupportImage()
    {
        var attachment = new NotificationAttachment
        {
            NotificationId = Guid.NewGuid(),
            FileName = "banner.png",
            FilePath = "/path/to/banner.png",
            ContentType = "image/png",
            FileSize = 1024 * 500
        };

        attachment.ContentType.Should().Be("image/png");
    }

    [Fact]
    public void ContentType_ShouldSupportHtml()
    {
        var attachment = new NotificationAttachment
        {
            NotificationId = Guid.NewGuid(),
            FileName = "template.html",
            FilePath = "/path/to/template.html",
            ContentType = "text/html",
            FileSize = 4096
        };

        attachment.ContentType.Should().Be("text/html");
    }

    [Fact]
    public void ContentType_ShouldSupportPlainText()
    {
        var attachment = new NotificationAttachment
        {
            NotificationId = Guid.NewGuid(),
            FileName = "notes.txt",
            FilePath = "/path/to/notes.txt",
            ContentType = "text/plain",
            FileSize = 512
        };

        attachment.ContentType.Should().Be("text/plain");
    }

    [Fact]
    public void FileSize_ShouldStoreSizeInBytes()
    {
        var attachment = new NotificationAttachment
        {
            NotificationId = Guid.NewGuid(),
            FileName = "large-file.zip",
            FilePath = "/path/to/large-file.zip",
            ContentType = "application/zip",
            FileSize = 10 * 1024 * 1024
        };

        attachment.FileSize.Should().Be(10 * 1024 * 1024);
    }

    [Fact]
    public void StorageType_ShouldDefaultToLocal()
    {
        var attachment = new NotificationAttachment
        {
            NotificationId = Guid.NewGuid(),
            FileName = "file.txt",
            FilePath = "/path/to/file.txt",
            ContentType = "text/plain",
            FileSize = 100
        };

        attachment.StorageType.Should().Be("local");
    }

    [Fact]
    public void StorageType_ShouldSupportS3()
    {
        var attachment = new NotificationAttachment
        {
            NotificationId = Guid.NewGuid(),
            FileName = "file.txt",
            FilePath = "s3://bucket/path/file.txt",
            ContentType = "text/plain",
            FileSize = 100,
            StorageType = "s3"
        };

        attachment.StorageType.Should().Be("s3");
    }

    [Fact]
    public void StorageType_ShouldSupportAzureBlob()
    {
        var attachment = new NotificationAttachment
        {
            NotificationId = Guid.NewGuid(),
            FileName = "file.txt",
            FilePath = "https://storage.blob.core.windows.net/container/file.txt",
            ContentType = "text/plain",
            FileSize = 100,
            StorageType = "azure"
        };

        attachment.StorageType.Should().Be("azure");
    }

    [Fact]
    public void DefaultContentType_ShouldBeEmpty()
    {
        var attachment = new NotificationAttachment
        {
            NotificationId = Guid.NewGuid(),
            FileName = "file.txt",
            FilePath = "/path/to/file.txt",
            FileSize = 100
        };

        attachment.ContentType.Should().BeEmpty();
    }

    [Fact]
    public void DocumentId_ShouldBeNull_WhenNotSet()
    {
        var attachment = new NotificationAttachment
        {
            NotificationId = Guid.NewGuid(),
            FileName = "file.txt",
            FilePath = "/path/to/file.txt",
            ContentType = "text/plain",
            FileSize = 100
        };

        attachment.DocumentId.Should().BeNull();
    }
}
