using System.Linq.Expressions;
using SportsGurukul.Application.Common.Interfaces.Notification;
using SportsGurukul.Domain.Entities.Notification;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Communication.Infrastructure.Tests.Repositories;

public class TemplateRepositoryTests
{
    private static int _counter;

    private static NotificationTemplate CreateTemplate(
        NotificationChannelType channel = NotificationChannelType.Email,
        bool isActive = true)
    {
        _counter++;
        return new NotificationTemplate
        {
            Id = Guid.NewGuid(),
            Name = $"Template{_counter:D3}",
            ChannelType = channel,
            SubjectTemplate = "Hello {{name}}",
            BodyTemplate = "Welcome {{name}}!",
            IsActive = isActive,
            CurrentVersion = 1,
            CreatedAt = DateTime.UtcNow
        };
    }

    private readonly Mock<ITemplateRepository> _mock;
    private readonly List<NotificationTemplate> _templates;

    public TemplateRepositoryTests()
    {
        _templates =
        [
            CreateTemplate(),
            CreateTemplate(NotificationChannelType.SMS),
            CreateTemplate(NotificationChannelType.Email)
        ];
        _mock = CreateMockWithBaseSetup(_templates);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnTemplateWithVersions_WhenFound()
    {
        var expected = _templates[0];
        expected.Versions.Add(new TemplateVersion
        {
            TemplateId = expected.Id,
            VersionNumber = 1,
            SubjectTemplate = expected.SubjectTemplate,
            BodyTemplate = expected.BodyTemplate,
            PublishedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        });
        _mock.Setup(r => r.GetWithVersionsAsync(expected.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);
        var result = await _mock.Object.GetWithVersionsAsync(expected.Id);
        result.Should().Be(expected);
        result!.Versions.Should().ContainSingle();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_ForUnknown()
    {
        var result = await _mock.Object.GetByIdAsync(Guid.NewGuid());
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnTemplates()
    {
        var result = await _mock.Object.GetAllAsync();
        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task AddAsync_ShouldAddTemplate()
    {
        var template = CreateTemplate();
        var result = await _mock.Object.AddAsync(template);
        result.Should().Be(template);
        _mock.Verify(r => r.AddAsync(template, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public void UpdateAsync_ShouldUpdateTemplate()
    {
        var template = _templates[0];
        _mock.Object.Update(template);
        _mock.Verify(r => r.Update(template), Times.Once);
    }

    [Fact]
    public void DeleteAsync_ShouldSoftDeleteTemplate()
    {
        var template = _templates[0];
        _mock.Object.Remove(template);
        _mock.Verify(r => r.Remove(template), Times.Once);
    }

    [Fact]
    public async Task GetByNameAsync_ShouldReturnTemplate_WhenFound()
    {
        var expected = _templates[0];
        _mock.Setup(r => r.GetByNameAsync(expected.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);
        var result = await _mock.Object.GetByNameAsync(expected.Name);
        result.Should().Be(expected);
    }

    [Fact]
    public async Task GetByNameAsync_ShouldReturnNull_WhenNotFound()
    {
        _mock.Setup(r => r.GetByNameAsync("NONEXISTENT", It.IsAny<CancellationToken>()))
            .ReturnsAsync((NotificationTemplate?)null);
        var result = await _mock.Object.GetByNameAsync("NONEXISTENT");
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByChannelAsync_ShouldReturnTemplatesByChannel()
    {
        var emailTemplates = _templates.Where(t => t.ChannelType == NotificationChannelType.Email).ToList();
        _mock.Setup(r => r.GetByChannelAsync(NotificationChannelType.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(emailTemplates);
        var result = await _mock.Object.GetByChannelAsync(NotificationChannelType.Email);
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(t => t.ChannelType.Should().Be(NotificationChannelType.Email));
    }

    [Fact]
    public async Task GetActiveTemplatesAsync_ShouldReturnActiveOnly()
    {
        var active = _templates.Where(t => t.IsActive).ToList();
        _mock.Setup(r => r.GetActiveTemplatesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(active);
        var result = await _mock.Object.GetActiveTemplatesAsync();
        result.Should().NotBeEmpty();
        result.Should().AllSatisfy(t => t.IsActive.Should().BeTrue());
    }

    private static Mock<ITemplateRepository> CreateMockWithBaseSetup(List<NotificationTemplate> data)
    {
        var mock = new Mock<ITemplateRepository>();

        mock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid id, CancellationToken _) => data.FirstOrDefault(e => e.Id == id));

        mock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(data);

        mock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<NotificationTemplate, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<NotificationTemplate, bool>> predicate, CancellationToken _) =>
                data.AsQueryable().Where(predicate).ToList());

        mock.Setup(r => r.AddAsync(It.IsAny<NotificationTemplate>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((NotificationTemplate entity, CancellationToken _) => entity);

        mock.Setup(r => r.CountAsync(It.IsAny<Expression<Func<NotificationTemplate, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<NotificationTemplate, bool>>? predicate, CancellationToken _) =>
                predicate == null ? data.Count : data.AsQueryable().Count(predicate));

        mock.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<NotificationTemplate, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<NotificationTemplate, bool>> predicate, CancellationToken _) =>
                data.AsQueryable().Any(predicate));

        return mock;
    }
}
