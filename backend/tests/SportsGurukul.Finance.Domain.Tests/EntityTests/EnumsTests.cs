using SportsGurukul.Domain.Enums.Finance;

namespace SportsGurukul.Finance.Domain.Tests.Entities;

public class EnumsTests
{
    [Fact]
    public void PaymentStatus_HasAllExpectedValues()
    {
        Enum.GetValues<PaymentStatus>().Should().HaveCount(6);
        ((int)PaymentStatus.Pending).Should().Be(0);
        ((int)PaymentStatus.Authorized).Should().Be(1);
        ((int)PaymentStatus.Captured).Should().Be(2);
        ((int)PaymentStatus.Failed).Should().Be(3);
        ((int)PaymentStatus.Cancelled).Should().Be(4);
        ((int)PaymentStatus.Refunded).Should().Be(5);
    }

    [Fact]
    public void InvoiceStatus_HasAllExpectedValues()
    {
        Enum.GetValues<InvoiceStatus>().Should().HaveCount(6);
        ((int)InvoiceStatus.Draft).Should().Be(0);
        ((int)InvoiceStatus.Issued).Should().Be(1);
        ((int)InvoiceStatus.PartiallyPaid).Should().Be(2);
        ((int)InvoiceStatus.Paid).Should().Be(3);
        ((int)InvoiceStatus.Cancelled).Should().Be(4);
        ((int)InvoiceStatus.Overdue).Should().Be(5);
    }

    [Fact]
    public void RefundStatus_HasAllExpectedValues()
    {
        Enum.GetValues<RefundStatus>().Should().HaveCount(4);
        ((int)RefundStatus.Requested).Should().Be(0);
        ((int)RefundStatus.Approved).Should().Be(1);
        ((int)RefundStatus.Rejected).Should().Be(2);
        ((int)RefundStatus.Completed).Should().Be(3);
    }

    [Fact]
    public void SettlementStatus_HasAllExpectedValues()
    {
        Enum.GetValues<SettlementStatus>().Should().HaveCount(4);
        ((int)SettlementStatus.Pending).Should().Be(0);
        ((int)SettlementStatus.InProgress).Should().Be(1);
        ((int)SettlementStatus.Completed).Should().Be(2);
        ((int)SettlementStatus.Failed).Should().Be(3);
    }

    [Fact]
    public void DiscountType_HasExpectedValues()
    {
        Enum.GetValues<DiscountType>().Should().HaveCount(2);
        ((int)DiscountType.Percentage).Should().Be(0);
        ((int)DiscountType.Flat).Should().Be(1);
    }

    [Fact]
    public void TransactionType_HasExpectedValues()
    {
        Enum.GetValues<TransactionType>().Should().HaveCount(5);
        ((int)TransactionType.Debit).Should().Be(0);
        ((int)TransactionType.Credit).Should().Be(1);
        ((int)TransactionType.Refund).Should().Be(2);
        ((int)TransactionType.Fee).Should().Be(3);
        ((int)TransactionType.Adjustment).Should().Be(4);
    }

    [Fact]
    public void PaymentMethod_HasExpectedValues()
    {
        Enum.GetValues<PaymentMethod>().Should().HaveCount(7);
    }

    [Fact]
    public void LedgerType_HasExpectedValues()
    {
        Enum.GetValues<LedgerType>().Should().HaveCount(5);
    }
}
