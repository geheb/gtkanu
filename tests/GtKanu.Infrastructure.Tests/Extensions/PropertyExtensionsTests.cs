namespace GtKanu.Infrastructure.Tests.Extensions;

using System.Linq.Expressions;
using GtKanu.Infrastructure.Extensions;

public class PropertyExtensionsTests
{
    private class TestEntity
    {
        public string? Name { get; set; }
        public int Count { get; set; }
    }

    [Fact]
    public void SetValue_NewValue_ShouldSetAndReturnTrue()
    {
        var entity = new TestEntity { Name = "Old" };
        var result = entity.SetValue(e => e.Name, "New");
        result.Should().BeTrue();
        entity.Name.Should().Be("New");
    }

    [Fact]
    public void SetValue_SameValue_ShouldReturnFalse()
    {
        var entity = new TestEntity { Name = "Same" };
        var result = entity.SetValue(e => e.Name, "Same");
        result.Should().BeFalse();
    }

    [Fact]
    public void SetValue_NullToValue_ShouldSetAndReturnTrue()
    {
        var entity = new TestEntity { Name = null };
        var result = entity.SetValue(e => e.Name, "Value");
        result.Should().BeTrue();
        entity.Name.Should().Be("Value");
    }

    [Fact]
    public void SetValue_ValueToNull_ShouldSetAndReturnTrue()
    {
        var entity = new TestEntity { Name = "Value" };
        var result = entity.SetValue(e => e.Name, null);
        result.Should().BeTrue();
        entity.Name.Should().BeNull();
    }

    [Fact]
    public void SetValue_ValueType_NewValue_ShouldSetAndReturnTrue()
    {
        var entity = new TestEntity { Count = 1 };
        var result = entity.SetValue(e => e.Count, 2);
        result.Should().BeTrue();
        entity.Count.Should().Be(2);
    }

    [Fact]
    public void SetValue_ValueType_SameValue_ShouldReturnFalse()
    {
        var entity = new TestEntity { Count = 5 };
        var result = entity.SetValue(e => e.Count, 5);
        result.Should().BeFalse();
    }

    [Fact]
    public void SetValue_InvalidExpression_ShouldThrow()
    {
        var entity = new TestEntity();
        Action act = () => entity.SetValue((Expression<Func<TestEntity, string>>)(_ => "literal"), "value");
        act.Should().Throw<InvalidCastException>();
    }
}
