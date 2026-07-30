using SportsGurukul.Domain.Entities.Finance;
using SportsGurukul.Domain.Enums.Finance;
using SportsGurukul.Finance.Domain.Tests.Builders;

namespace SportsGurukul.Finance.Domain.Tests.Entities;

public class PaymentEntityTests
{
    [Fact]
    public void CreatePayment_HasCorrectInitialState()
    {
        var p = FinanceEntityBuilder.CreatePayment();
        p.Id.Should().NotBeEmpty();
        p.PaymentReference.Should().StartWith("PAY-");
        p.Status.Should().Be(PaymentStatus.Pending);
        p.Currency.Should().Be("INR");
    }

    [Fact]
    public void CapturedPayment_StatusChangesCorrectly()
    {
        var p = FinanceEntityBuilder.CreatePayment(status: PaymentStatus.Captured);
        p.Status.Should().Be(PaymentStatus.Captured);
    }

    [Fact]
    public void FailedPayment_HasFailureReason()
    {
        var p = FinanceEntityBuilder.CreatePayment(status: PaymentStatus.Failed);
        p.FailureReason = "Insufficient funds";
        p.Status.Should().Be(PaymentStatus.Failed);
        p.FailureReason.Should().Be("Insufficient funds");
    }

    [Fact]
    public void RefundedPayment_CannotBeRefundedAgain()
    {
        var p = FinanceEntityBuilder.CreatePayment(status: PaymentStatus.Refunded);
        p.Status.Should().Be(PaymentStatus.Refunded);
    }

    [Fact]
    public void Payment_WithIdempotencyKey_IsMarkedIdempotent()
    {
        var p = FinanceEntityBuilder.CreatePayment();
        p.IdempotencyKey = "idem-key-001";
        p.IsIdempotent = true;
        p.IsIdempotent.Should().BeTrue();
        p.IdempotencyKey.Should().Be("idem-key-001");
    }

    [Fact]
    public void Payment_WithGatewayReference_StoresCorrectly()
    {
        var p = FinanceEntityBuilder.CreatePayment();
        var gatewayTxn = new GatewayTransaction
        {
            GatewayId = Guid.NewGuid(),
            PaymentId = p.Id,
            TransactionId = "gtxn_001",
            Status = "completed",
            TransactedAt = DateTime.UtcNow
        };
        p.GatewayTransactions.Add(gatewayTxn);
        p.GatewayTransactions.Should().ContainSingle();
    }
}
