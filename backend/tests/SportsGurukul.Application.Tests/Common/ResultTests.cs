using FluentAssertions;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Tests.Common;

public class ResultTests
{
    [Fact]
    public void Success_SetsIsTrueAndValue()
    {
        var result = Result<string>.Success("hello");

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("hello");
        result.Error.Should().BeNull();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Failure_SingleError_SetsIsFalseAndError()
    {
        var result = Result<string>.Failure("something went wrong");

        result.IsSuccess.Should().BeFalse();
        result.Value.Should().BeNull();
        result.Error.Should().Be("something went wrong");
        result.Errors.Should().HaveCount(1);
        result.Errors.Should().Contain("something went wrong");
    }

    [Fact]
    public void Failure_MultipleErrors_SetsFirstAsError()
    {
        var errors = new[] { "first error", "second error", "third error" };
        var result = Result<string>.Failure(errors);

        result.IsSuccess.Should().BeFalse();
        result.Value.Should().BeNull();
        result.Error.Should().Be("first error");
        result.Errors.Should().HaveCount(3);
    }

    [Fact]
    public void Failure_EmptyErrorsList_SetsNullError()
    {
        var result = Result<string>.Failure(Array.Empty<string>());

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().BeNull();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Success_IntValue_Works()
    {
        var result = Result<int>.Success(42);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(42);
    }

    [Fact]
    public void Failure_ListOverload_ContainsAllErrors()
    {
        var errors = new List<string> { "err1", "err2" };
        var result = Result<int>.Failure(errors);

        result.Errors.Should().BeEquivalentTo(errors);
    }
}
