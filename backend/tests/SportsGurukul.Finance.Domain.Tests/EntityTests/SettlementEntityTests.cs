using SportsGurukul.Domain.Entities.Finance;
using SportsGurukul.Domain.Enums.Finance;
using SportsGurukul.Finance.Domain.Tests.Builders;

namespace SportsGurukul.Finance.Domain.Tests.Entities;

public class SettlementEntityTests
{
    [Fact]
    public void CreateSettlementBatch_HasCorrectInitialStatus()
    {
        var b = FinanceEntityBuilder.CreateSettlementBatch();
        b.BatchNumber.Should().StartWith("STL-");
        b.Status.Should().Be(SettlementStatus.Pending);
    }

    [Fact]
    public void CompletedSettlement_HasSettledAtDate()
    {
        var b = FinanceEntityBuilder.CreateSettlementBatch(status: SettlementStatus.Completed);
        b.SettledAt = DateTime.UtcNow;
        b.SettledAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void SettlementBatch_WithMultipleSettlements_HasCorrectTotal()
    {
        var batch = FinanceEntityBuilder.CreateSettlementBatch(total: 15000);
        batch.TotalAmount.Should().Be(15000);
    }

    [Fact]
    public void Settlement_ReferencesPaymentCorrectly()
    {
        var paymentId = Guid.NewGuid();
        var batchId = Guid.NewGuid();
        var s = FinanceEntityBuilder.CreateSettlement(batchId, paymentId);
        s.PaymentId.Should().Be(paymentId);
        s.SettlementBatchId.Should().Be(batchId);
    }
}
