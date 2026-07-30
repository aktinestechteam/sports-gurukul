using SportsGurukul.Domain.Entities.Finance;
using SportsGurukul.Domain.Enums.Finance;
using SportsGurukul.Finance.Domain.Tests.Builders;

namespace SportsGurukul.Finance.Domain.Tests.Entities;

public class InvoiceEntityTests
{
    [Fact]
    public void CreateInvoice_DefaultValues_AreSetCorrectly()
    {
        var inv = FinanceEntityBuilder.CreateInvoice();
        inv.Id.Should().NotBeEmpty();
        inv.InvoiceNumber.Should().StartWith("INV-");
        inv.Status.Should().Be(InvoiceStatus.Draft);
        inv.Currency.Should().Be("INR");
    }

    [Fact]
    public void CreateInvoice_WithLineItems_CalculatesTotalCorrectly()
    {
        var inv = FinanceEntityBuilder.CreateInvoice(total: 5000);
        var item = FinanceEntityBuilder.CreateInvoiceItem(inv.Id, unitPrice: 2000, quantity: 2);
        inv.Items.Add(item);
        item.TotalAmount.Should().Be(4000);
    }

    [Fact]
    public void FullyPaidInvoice_StatusCanBePaid()
    {
        var inv = FinanceEntityBuilder.CreateInvoice(total: 5000, paidAmount: 5900, amountDue: 0, status: InvoiceStatus.Paid);
        inv.Status.Should().Be(InvoiceStatus.Paid);
        inv.AmountDue.Should().Be(0);
        inv.AmountPaid.Should().Be(5900);
    }

    [Fact]
    public void OverdueInvoice_DueDateInPast_ShouldBeOverdueWhenIssued()
    {
        var inv = FinanceEntityBuilder.CreateInvoice(status: InvoiceStatus.Issued, total: 3000);
        inv.DueDate = DateTime.UtcNow.AddDays(-5);
        inv.DueDate.Should().BeBefore(DateTime.UtcNow);
        inv.Status.Should().Be(InvoiceStatus.Issued);
    }

    [Fact]
    public void CancelledInvoice_CannotBePaid()
    {
        var inv = FinanceEntityBuilder.CreateInvoice(status: InvoiceStatus.Cancelled);
        inv.Status.Should().Be(InvoiceStatus.Cancelled);
    }

    [Fact]
    public void Invoice_WithDiscount_HasCorrectDiscountTotal()
    {
        var inv = FinanceEntityBuilder.CreateInvoice(total: 5000);
        var disc = FinanceEntityBuilder.CreateInvoiceDiscount(inv.Id, amount: 500);
        inv.Discounts.Add(disc);
        inv.DiscountTotal.Should().Be(500);
    }

    [Fact]
    public void Invoice_WithTaxes_HasCorrectTaxTotal()
    {
        var inv = FinanceEntityBuilder.CreateInvoice(total: 10000);
        var tax = FinanceEntityBuilder.CreateInvoiceTax(inv.Id, amount: 1800);
        inv.Taxes.Add(tax);
        inv.TaxTotal.Should().Be(1800);
    }
}
