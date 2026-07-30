using SportsGurukul.Platform.Communication.Security;

namespace SportsGurukul.Communication.Infrastructure.Tests.Security;

public class DataMaskerTests
{
    private readonly DataMasker _masker = new();

    [Fact]
    public void MaskEmail_PreservesDomain()
    {
        var result = _masker.MaskEmail("john.doe@example.com");
        result.Should().Be("j***e@example.com");
    }

    [Fact]
    public void MaskEmail_MasksLocalPart()
    {
        var result = _masker.MaskEmail("alice@test.org");
        result.Should().Be("a***e@test.org");
    }

    [Fact]
    public void MaskEmail_HandlesShortLocalPart()
    {
        var result = _masker.MaskEmail("ab@test.com");
        result.Should().Be("a***@test.com");
    }

    [Fact]
    public void MaskEmail_ReturnsWholeEmail_WhenNoAtSign()
    {
        var result = _masker.MaskEmail("invalid-email");
        result.Should().Be("invalid-email");
    }

    [Fact]
    public void MaskPhone_PreservesLastFour()
    {
        var result = _masker.MaskPhone("+919876543210");
        result.Should().EndWith("3210");
        result.Length.Should().Be("+919876543210".Length);
    }

    [Fact]
    public void MaskPhone_MasksAllButLastFour()
    {
        var result = _masker.MaskPhone("9876543210");
        result.Should().Be("******3210");
    }

    [Fact]
    public void MaskPhone_ReturnsOriginal_WhenTooShort()
    {
        var result = _masker.MaskPhone("123");
        result.Should().Be("123");
    }

    [Fact]
    public void MaskSensitiveValue_MasksContent()
    {
        var result = _masker.MaskSensitiveValue("secret123", 4);
        result.Should().Be("*****t123");
    }

    [Fact]
    public void MaskSensitiveValue_MasksAll_WhenShorterThanVisibleChars()
    {
        var result = _masker.MaskSensitiveValue("ab", 4);
        result.Should().Be("**");
    }

    [Fact]
    public void MaskSensitiveValue_ReturnsAsterisks_ForZeroVisibleChars()
    {
        var result = _masker.MaskSensitiveValue("password", 0);
        result.Should().Be("********");
    }

    [Fact]
    public void MaskAsync_ReturnsNonSensitiveDataUnchanged()
    {
        var email = _masker.MaskEmail("public@info.com");
        email.Should().Be("p***c@info.com");
    }

    [Fact]
    public void MaskEmail_ReturnsEmptyString_ForNullInput()
    {
        var result = _masker.MaskEmail(null);
        result.Should().Be(string.Empty);
    }

    [Fact]
    public void MaskEmail_ReturnsEmptyString_ForEmptyInput()
    {
        var result = _masker.MaskEmail(string.Empty);
        result.Should().Be(string.Empty);
    }

    [Fact]
    public void MaskPhone_ReturnsEmptyString_ForNullInput()
    {
        var result = _masker.MaskPhone(null);
        result.Should().Be(string.Empty);
    }

    [Fact]
    public void MaskSensitiveValue_ReturnsEmptyString_ForNullInput()
    {
        var result = _masker.MaskSensitiveValue(null);
        result.Should().Be(string.Empty);
    }

    [Fact]
    public void MaskSensitiveValue_ReturnsEmptyString_ForEmptyInput()
    {
        var result = _masker.MaskSensitiveValue(string.Empty);
        result.Should().Be(string.Empty);
    }

    [Fact]
    public void MaskJsonSensitiveFields_MasksSpecifiedFields()
    {
        var json = """{"password":"secret123","name":"John","token":"abc123"}""";
        var result = _masker.MaskJsonSensitiveFields(json, "password", "token");
        result.Should().Contain("*****t123");
        result.Should().Contain("**c123");
        result.Should().Contain("John");
    }

    [Fact]
    public void MaskJsonSensitiveFields_ReturnsOriginal_ForEmptyInput()
    {
        var result = _masker.MaskJsonSensitiveFields(string.Empty, "password");
        result.Should().Be(string.Empty);
    }

    [Fact]
    public void MaskDictionary_MasksSensitiveKeys()
    {
        var data = new Dictionary<string, string>
        {
            ["email"] = "test@example.com",
            ["password"] = "mysecret",
            ["name"] = "John"
        };

        var result = _masker.MaskDictionary(data, "password");

        result["email"].Should().Be("test@example.com");
        result["password"].Should().Be("****cret");
        result["name"].Should().Be("John");
    }

    [Fact]
    public void MaskDictionary_ReturnsOriginal_WhenNoSensitiveKeysMatch()
    {
        var data = new Dictionary<string, string>
        {
            ["key1"] = "value1",
            ["key2"] = "value2"
        };

        var result = _masker.MaskDictionary(data, "password");

        result["key1"].Should().Be("value1");
        result["key2"].Should().Be("value2");
    }

    [Fact]
    public void MaskEmail_HandlesSingleCharacterLocalPart()
    {
        var result = _masker.MaskEmail("a@b.com");
        result.Should().Be("a@b.com");
    }

    [Fact]
    public void MaskPhone_HandlesNullInput()
    {
        var result = _masker.MaskPhone(null);
        result.Should().Be(string.Empty);
    }
}
