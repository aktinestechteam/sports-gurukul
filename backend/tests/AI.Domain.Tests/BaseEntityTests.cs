using SportsGurukul.Domain.Common;

namespace AI.Domain.Tests;

public class BaseEntityTests
{
    private class TestEntity : BaseEntity
    {
        public string Value { get; set; } = string.Empty;
    }

    [Fact]
    public void NewEntity_DefaultValues()
    {
        var entity = new TestEntity();
        entity.Id.Should().Be(Guid.Empty);
        entity.CreatedAt.Should().Be(default);
        entity.UpdatedAt.Should().BeNull();
        entity.CreatedBy.Should().BeNull();
        entity.UpdatedBy.Should().BeNull();
        entity.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public void NewEntity_WithDefaults_HasEmptyValue()
    {
        var entity = new TestEntity();
        entity.Value.Should().BeEmpty();
    }

    [Fact]
    public void SetAllProperties_PersistsValues()
    {
        var id = Guid.NewGuid();
        var createdBy = Guid.NewGuid();
        var updatedBy = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var updatedAt = now.AddHours(1);

        var entity = new TestEntity
        {
            Id = id,
            CreatedAt = now,
            UpdatedAt = updatedAt,
            CreatedBy = createdBy,
            UpdatedBy = updatedBy,
            IsDeleted = true,
            Value = "hello"
        };

        entity.Id.Should().Be(id);
        entity.CreatedAt.Should().Be(now);
        entity.UpdatedAt.Should().Be(updatedAt);
        entity.CreatedBy.Should().Be(createdBy);
        entity.UpdatedBy.Should().Be(updatedBy);
        entity.IsDeleted.Should().BeTrue();
        entity.Value.Should().Be("hello");
    }
}
