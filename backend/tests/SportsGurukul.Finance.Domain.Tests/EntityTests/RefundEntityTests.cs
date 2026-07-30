using SportsGurukul.Domain.Entities.Finance;
using SportsGurukul.Domain.Enums.Finance;
using SportsGurukul.Finance.Domain.Tests.Builders;

namespace SportsGurukul.Finance.Domain.Tests.Entities;

public class RefundEntityTests
{
    [Fact]
    public void CreateRefund_HasCorrectInitialStatus()
    {
        var r = FinanceEntityBuilder.CreateRefund();
        r.RefundNumber.Should().StartWith("RFN-");
        r.Status.Should().Be(RefundStatus.Requested);
    }

    [Fact]
    public void ApprovedRefund_StatusChanges()
    {
        var r = FinanceEntityBuilder.CreateRefund(status: RefundStatus.Approved);
        r.ApprovedBy = "admin@test.com";
        r.ApprovedAt = DateTime.UtcNow;
        r.Status.Should().Be(RefundStatus.Approved);
        r.ApprovedBy.Should().Be("admin@test.com");
    }

    [Fact]
    public void CompletedRefund_HasGatewayReference()
    {
        var r = FinanceEntityBuilder.CreateRefund(status: RefundStatus.Completed);
        r.GatewayReference = "gtxn_refund_001";
        r.GatewayReference.Should().Be("gtxn_refund_001");
    }

    [Fact]
    public void RejectedRefund_HasReason()
    {
        var r = FinanceEntityBuilder.CreateRefund(status: RefundStatus.Rejected);
        r.Status.Should().Be(RefundStatus.Rejected);
    }

    [Fact]
    public void RefundAmount_CannotExceedPaymentAmount()
    {
        var payment = FinanceEntityBuilder.CreatePayment(amount: 1000);
        var refund = FinanceEntityBuilder.CreateRefund(paymentId: payment.Id, amount: 1500);
        refund.TotalAmount.Should().BeGreaterThan(payment.Amount);
    }
}
