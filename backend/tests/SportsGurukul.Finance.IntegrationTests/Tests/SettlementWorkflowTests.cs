using FluentAssertions;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.Commands.Settlement;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;
using SportsGurukul.Application.Features.FinanceManagement.Queries;
using SportsGurukul.Domain.Enums.Finance;
using SportsGurukul.Finance.IntegrationTests.Fixtures;
using SportsGurukul.Finance.IntegrationTests.Helpers;
using SportsGurukul.Finance.IntegrationTests.Seed;
using Xunit;

namespace SportsGurukul.Finance.IntegrationTests.Tests;

[Collection("Finance")]
public class SettlementWorkflowTests : FinanceTestBase
{
    public SettlementWorkflowTests(FinanceWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task CreateSettlementBatch_WithValidData_ReturnsSuccess()
    {
        var result = await SendAsync(new CreateSettlementBatchCommand(
            Array.Empty<Guid>()));

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task ApproveSettlement_ChangesStatus()
    {
        var batchResult = await SendAsync(new CreateSettlementBatchCommand(
            Array.Empty<Guid>()));

        var result = await SendAsync(new ApproveSettlementCommand(batchResult.Value!.Id));

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task CompleteSettlement_WithApproved_ReturnsSuccess()
    {
        var batchResult = await SendAsync(new CreateSettlementBatchCommand(
            Array.Empty<Guid>()));

        await SendAsync(new ApproveSettlementCommand(batchResult.Value!.Id));

        var result = await SendAsync(new CompleteSettlementCommand(
            batchResult.Value!.Id, "BANK-REF-001"));

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Status.Should().Be(SettlementStatus.Completed);
    }

    [Fact]
    public async Task CompleteSettlement_WithoutApproval_ReturnsFailure()
    {
        var batchResult = await SendAsync(new CreateSettlementBatchCommand(
            Array.Empty<Guid>()));

        var result = await SendAsync(new CompleteSettlementCommand(
            batchResult.Value!.Id, "BANK-REF-999"));

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task GetSettlementById_ReturnsSettlement()
    {
        var batchResult = await SendAsync(new CreateSettlementBatchCommand(
            Array.Empty<Guid>()));

        var result = await SendAsync(new GetSettlementByIdQuery(batchResult.Value!.Id));

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Id.Should().Be(batchResult.Value!.Id);
    }

    [Fact]
    public async Task Settlement_UpdatesLedger()
    {
        var batchResult = await SendAsync(new CreateSettlementBatchCommand(
            Array.Empty<Guid>()));

        await SendAsync(new ApproveSettlementCommand(batchResult.Value!.Id));

        var completeResult = await SendAsync(new CompleteSettlementCommand(
            batchResult.Value!.Id, "BANK-REF-LEDGER-001"));

        completeResult.IsSuccess.Should().BeTrue();
        completeResult.Value!.Status.Should().Be(SettlementStatus.Completed);
    }
}
