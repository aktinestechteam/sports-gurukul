using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.BusinessRules;
using SportsGurukul.Application.Features.NotificationManagement.BusinessRules.Rules;

namespace SportsGurukul.Communication.Application.Tests.BusinessRules;

public class BusinessRuleValidatorTests
{
    private readonly Mock<ILogger<BusinessRuleValidator>> _loggerMock;
    private readonly BusinessRuleValidator _validator;
    private readonly List<Mock<IBusinessRule>> _ruleMocks;

    public BusinessRuleValidatorTests()
    {
        _loggerMock = new Mock<ILogger<BusinessRuleValidator>>();
        _ruleMocks = new List<Mock<IBusinessRule>>
        {
            new Mock<IBusinessRule>(),
            new Mock<IBusinessRule>(),
            new Mock<IBusinessRule>()
        };
        var rules = _ruleMocks.Select(m => m.Object);
        _validator = new BusinessRuleValidator(rules, _loggerMock.Object);
    }

    [Fact]
    public async Task ValidateAsync_WhenAllRulesPass_ReturnsSuccess()
    {
        foreach (var mock in _ruleMocks)
        {
            mock.Setup(r => r.ValidateAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<bool>.Success(true));
        }

        var result = await _validator.ValidateAsync(new object());

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateAsync_WhenAnyRuleFails_ReturnsFailure()
    {
        _ruleMocks[0].Setup(r => r.ValidateAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));
        _ruleMocks[1].Setup(r => r.ValidateAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Failure("Rule 2 failed"));
        _ruleMocks[2].Setup(r => r.ValidateAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        var result = await _validator.ValidateAsync(new object());

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Rule 2 failed");
    }

    [Fact]
    public async Task ValidateAsync_ReturnsAllErrorsFromAllRules()
    {
        _ruleMocks[0].Setup(r => r.ValidateAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Failure("Error 1"));
        _ruleMocks[1].Setup(r => r.ValidateAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Failure("Error 2"));
        _ruleMocks[2].Setup(r => r.ValidateAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Failure("Error 3"));

        var result = await _validator.ValidateAsync(new object());

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().HaveCount(3);
        result.Errors.Should().Contain("Error 1");
        result.Errors.Should().Contain("Error 2");
        result.Errors.Should().Contain("Error 3");
    }

    [Fact]
    public async Task ValidateAsync_RunsAllRulesEvenWhenPreviousFails()
    {
        _ruleMocks[0].Setup(r => r.ValidateAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Failure("Error 1"));
        _ruleMocks[1].Setup(r => r.ValidateAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Failure("Error 2"));
        _ruleMocks[2].Setup(r => r.ValidateAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Failure("Error 3"));

        await _validator.ValidateAsync(new object());

        foreach (var mock in _ruleMocks)
        {
            mock.Verify(r => r.ValidateAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }

    [Fact]
    public async Task ValidateAsync_WhenNoRules_ReturnsSuccess()
    {
        var emptyValidator = new BusinessRuleValidator(
            Enumerable.Empty<IBusinessRule>(),
            new Mock<ILogger<BusinessRuleValidator>>().Object);

        var result = await emptyValidator.ValidateAsync(new object());

        result.IsSuccess.Should().BeTrue();
    }
}
