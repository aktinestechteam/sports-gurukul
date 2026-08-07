using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces.AI;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.ModelRouting;
using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Tests.AI.ModelRouting;

public class ModelCandidateBuilder
{
    private readonly List<ModelCandidate> _candidates = new();

    public static ModelCandidate Create(
        string name,
        decimal? inputCost = null,
        decimal? outputCost = null,
        long? latency = null,
        bool supportsFunctionCalling = true,
        bool supportsVision = false,
        bool supportsJsonMode = true,
        int? contextWindow = 128000,
        bool supportsChat = true) =>
        new(
            Guid.NewGuid(), Guid.NewGuid(), name, "Provider", AIModelFamily.Gpt,
            contextWindow, 8192, inputCost, outputCost, "USD",
            supportsChat, supportsFunctionCalling, supportsVision, supportsJsonMode,
            60, latency);

    public ModelCandidateBuilder Add(ModelCandidate candidate)
    {
        _candidates.Add(candidate);
        return this;
    }

    public IReadOnlyList<ModelCandidate> Build() => _candidates;
}

public class CostBasedModelSelectionStrategyTests
{
    private readonly CostBasedModelSelectionStrategy _strategy = new();

    [Fact]
    public async Task SelectAsync_SelectsCheapestModel()
    {
        var expensive = ModelCandidateBuilder.Create("expensive", inputCost: 10m, outputCost: 20m, latency: 100);
        var cheap = ModelCandidateBuilder.Create("cheap", inputCost: 1m, outputCost: 2m, latency: 300);
        var candidates = new ModelCandidateBuilder().Add(expensive).Add(cheap).Build();
        var context = new ModelSelectionContext(null, null, null, AIRoutingStrategy.Cost, 1000, 1000, false, false, false, null, null, null, null);

        var result = await _strategy.SelectAsync(candidates, context);

        result.IsSuccess.Should().BeTrue();
        result.Value!.ModelName.Should().Be("cheap");
        result.Value.EstimatedCost.Should().BeLessThan(0.01m);
    }

    [Fact]
    public async Task SelectAsync_NoCandidateUnderMaxCost_Fails()
    {
        var candidates = new ModelCandidateBuilder()
            .Add(ModelCandidateBuilder.Create("a", inputCost: 10m, outputCost: 20m))
            .Build();
        var context = new ModelSelectionContext(null, null, null, AIRoutingStrategy.Cost, 1000, 1000, false, false, false, 0.001m, null, null, null);

        var result = await _strategy.SelectAsync(candidates, context);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("cost");
    }

    [Fact]
    public async Task SelectAsync_EmptyCandidates_Fails()
    {
        var context = new ModelSelectionContext(null, null, null, AIRoutingStrategy.Cost, null, null, false, false, false, null, null, null, null);

        var result = await _strategy.SelectAsync(new List<ModelCandidate>(), context);

        result.IsSuccess.Should().BeFalse();
    }
}

public class LatencyBasedModelSelectionStrategyTests
{
    private readonly LatencyBasedModelSelectionStrategy _strategy = new();

    [Fact]
    public async Task SelectAsync_SelectsFastestModel()
    {
        var fast = ModelCandidateBuilder.Create("fast", latency: 50);
        var slow = ModelCandidateBuilder.Create("slow", latency: 900);
        var candidates = new ModelCandidateBuilder().Add(slow).Add(fast).Build();
        var context = new ModelSelectionContext(null, null, null, AIRoutingStrategy.Speed, null, null, false, false, false, null, null, null, null);

        var result = await _strategy.SelectAsync(candidates, context);

        result.IsSuccess.Should().BeTrue();
        result.Value!.ModelName.Should().Be("fast");
        result.Value.EstimatedLatencyMs.Should().Be(50);
    }

    [Fact]
    public async Task SelectAsync_NoCandidateUnderMaxLatency_Fails()
    {
        var candidates = new ModelCandidateBuilder()
            .Add(ModelCandidateBuilder.Create("a", latency: 1000))
            .Build();
        var context = new ModelSelectionContext(null, null, null, AIRoutingStrategy.Speed, null, null, false, false, false, null, 100, null, null);

        var result = await _strategy.SelectAsync(candidates, context);

        result.IsSuccess.Should().BeFalse();
    }
}

public class CapabilityBasedModelSelectionStrategyTests
{
    private readonly CapabilityBasedModelSelectionStrategy _strategy = new();

    [Fact]
    public async Task SelectAsync_FiltersModelsMissingRequiredCapabilities()
    {
        var noVision = ModelCandidateBuilder.Create("no-vision", supportsVision: false, contextWindow: 10000);
        var hasVision = ModelCandidateBuilder.Create("has-vision", supportsVision: true, contextWindow: 20000);
        var candidates = new ModelCandidateBuilder().Add(noVision).Add(hasVision).Build();
        var context = new ModelSelectionContext(null, null, null, AIRoutingStrategy.Accuracy, null, null, false, true, false, null, null, null, null);

        var result = await _strategy.SelectAsync(candidates, context);

        result.IsSuccess.Should().BeTrue();
        result.Value!.ModelName.Should().Be("has-vision");
    }

    [Fact]
    public async Task SelectAsync_NoModelMeetsCapabilities_Fails()
    {
        var candidates = new ModelCandidateBuilder()
            .Add(ModelCandidateBuilder.Create("a", supportsVision: false))
            .Build();
        var context = new ModelSelectionContext(null, null, null, AIRoutingStrategy.Accuracy, null, null, false, true, false, null, null, null, null);

        var result = await _strategy.SelectAsync(candidates, context);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("capabilities");
    }
}

public class BalancedModelSelectionStrategyTests
{
    private readonly BalancedModelSelectionStrategy _strategy = new();

    [Fact]
    public async Task SelectAsync_ReturnsEligibleModel()
    {
        var candidates = new ModelCandidateBuilder()
            .Add(ModelCandidateBuilder.Create("a", inputCost: 5m, outputCost: 10m, latency: 500))
            .Add(ModelCandidateBuilder.Create("b", inputCost: 2m, outputCost: 4m, latency: 200))
            .Build();
        var context = new ModelSelectionContext(null, null, null, AIRoutingStrategy.Balanced, 1000, 1000, false, false, false, null, null, null, null);

        var result = await _strategy.SelectAsync(candidates, context);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task SelectAsync_NoModelUnderConstraints_Fails()
    {
        var candidates = new ModelCandidateBuilder()
            .Add(ModelCandidateBuilder.Create("a", inputCost: 5m, outputCost: 10m, latency: 500))
            .Build();
        var context = new ModelSelectionContext(null, null, null, AIRoutingStrategy.Balanced, 1000, 1000, false, false, false, 0.0001m, 50, null, null);

        var result = await _strategy.SelectAsync(candidates, context);

        result.IsSuccess.Should().BeFalse();
    }
}

public class ModelRoutingServiceTests
{
    private readonly Mock<IModelAvailabilityService> _availabilityMock = new();
    private readonly Mock<IFallbackStrategy> _fallbackMock = new();
    private readonly Mock<IAIProviderRepository> _providerMock = new();
    private readonly Mock<ILogger<ModelRoutingService>> _loggerMock = new();

    private ModelRoutingService BuildService(params IModelSelectionStrategy[] strategies)
        => new(strategies, _availabilityMock.Object, _fallbackMock.Object, _providerMock.Object, _loggerMock.Object);

    [Fact]
    public async Task SelectModelAsync_NoCandidates_ReturnsFailure()
    {
        _availabilityMock.Setup(a => a.GetAvailableCandidatesAsync(It.IsAny<ModelSelectionContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ModelCandidate>());
        var service = BuildService(new BalancedModelSelectionStrategy());
        var context = new ModelSelectionContext(null, null, null, AIRoutingStrategy.Balanced, null, null, false, false, false, null, null, null, null);

        var result = await service.SelectModelAsync(context);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("available");
    }

    [Fact]
    public async Task SelectModelAsync_UsesRequestedStrategy()
    {
        var candidates = new ModelCandidateBuilder()
            .Add(ModelCandidateBuilder.Create("a", inputCost: 1m, outputCost: 2m))
            .Build();
        _availabilityMock.Setup(a => a.GetAvailableCandidatesAsync(It.IsAny<ModelSelectionContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(candidates);
        var service = BuildService(new CostBasedModelSelectionStrategy(), new BalancedModelSelectionStrategy());
        var context = new ModelSelectionContext(null, null, null, AIRoutingStrategy.Cost, 1000, 1000, false, false, false, null, null, null, null);

        var result = await service.SelectModelAsync(context);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Reason.Should().Contain("cost");
    }

    [Fact]
    public async Task SelectModelAsync_UnknownStrategy_FallsBackToBalanced()
    {
        var candidates = new ModelCandidateBuilder()
            .Add(ModelCandidateBuilder.Create("a", inputCost: 1m, outputCost: 2m))
            .Build();
        _availabilityMock.Setup(a => a.GetAvailableCandidatesAsync(It.IsAny<ModelSelectionContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(candidates);
        var service = BuildService(new BalancedModelSelectionStrategy());
        var context = new ModelSelectionContext(null, null, null, AIRoutingStrategy.Manual, 1000, 1000, false, false, false, null, null, null, null);

        var result = await service.SelectModelAsync(context);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Reason.Should().Contain("balanced");
    }

    [Fact]
    public async Task ResolveFallbackChainAsync_ReturnsPrioritizedChain()
    {
        var first = ModelCandidateBuilder.Create("first");
        var second = ModelCandidateBuilder.Create("second");
        var candidates = new ModelCandidateBuilder().Add(second).Add(first).Build();
        _availabilityMock.Setup(a => a.GetAvailableCandidatesAsync(It.IsAny<ModelSelectionContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(candidates);
        _fallbackMock.Setup(f => f.ResolveFallbackChainAsync(It.IsAny<IReadOnlyList<ModelCandidate>>(), It.IsAny<ModelSelectionContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { first.ModelId, second.ModelId });
        var service = BuildService();
        var context = new ModelSelectionContext(null, null, null, AIRoutingStrategy.Fallback, null, null, false, false, false, null, null, null, null);

        var result = await service.ResolveFallbackChainAsync(context);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Should().HaveCount(2);
        result.Value![0].ModelId.Should().Be(first.ModelId);
    }

    [Fact]
    public async Task IsModelAvailableAsync_ReturnsAvailability()
    {
        _availabilityMock.Setup(a => a.IsAvailableAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        var service = BuildService();

        var result = await service.IsModelAvailableAsync(Guid.NewGuid());

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
    }
}

public class FallbackStrategyTests
{
    private readonly FallbackStrategy _strategy = new();

    [Fact]
    public async Task ResolveFallbackChainAsync_PrefersConfiguredFallbacksThenRest()
    {
        var preferred = ModelCandidateBuilder.Create("preferred");
        var other = ModelCandidateBuilder.Create("other");
        var candidates = new ModelCandidateBuilder().Add(other).Add(preferred).Build();
        var context = new ModelSelectionContext(null, null, null, AIRoutingStrategy.Fallback, null, null, false, false, false, null, null, null, new[] { preferred.ModelId });

        var chain = await _strategy.ResolveFallbackChainAsync(candidates, context);

        chain.Should().HaveCount(2);
        chain[0].Should().Be(preferred.ModelId);
        chain[1].Should().Be(other.ModelId);
    }
}
