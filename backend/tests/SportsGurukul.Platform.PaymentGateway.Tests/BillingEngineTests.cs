using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using SportsGurukul.Platform.PaymentGateway.Billing;
using SportsGurukul.Platform.PaymentGateway.Models;
using SportsGurukul.Platform.PaymentGateway.Tax;

namespace SportsGurukul.Platform.PaymentGateway.Tests;

public class BillingEngineTests
{
    private readonly IBillingService _billingService;

    public BillingEngineTests()
    {
        var taxEngine = new TaxEngine(NullLogger<TaxEngine>.Instance);
        _billingService = new BillingService(taxEngine, NullLogger<BillingService>.Instance);
    }

    [Fact]
    public async Task GenerateInvoice_CalculatesCorrectTotals()
    {
        var request = new InvoiceGenerationRequest
        {
            InvoiceNumber = "INV-001",
            CustomerId = "cust_001",
            LineItems =
            [
                new InvoiceLineItem { Description = "Item 1", Quantity = 2, UnitPrice = 500 },
                new InvoiceLineItem { Description = "Item 2", Quantity = 1, UnitPrice = 1000, TaxRate = 18, TaxName = "GST" }
            ],
            IssueDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(30),
            Currency = "INR"
        };

        var result = await _billingService.GenerateInvoiceAsync(request);

        result.InvoiceNumber.Should().Be("INV-001");
        result.SubTotal.Should().Be(2000);
        result.TaxTotal.Should().Be(180);
        result.Total.Should().Be(2180);
        result.TaxBreakdown.Should().HaveCount(1);
    }

    [Fact]
    public async Task GenerateInvoice_EmptyLineItems_ReturnsZeroTotal()
    {
        var request = new InvoiceGenerationRequest
        {
            InvoiceNumber = "INV-002",
            CustomerId = "cust_002",
            LineItems = [],
            Currency = "INR"
        };

        var result = await _billingService.GenerateInvoiceAsync(request);

        result.SubTotal.Should().Be(0);
        result.Total.Should().Be(0);
    }

    [Fact]
    public async Task GenerateInvoice_ZeroQuantity_CalculatesCorrectly()
    {
        var request = new InvoiceGenerationRequest
        {
            InvoiceNumber = "INV-ZERO",
            CustomerId = "cust_003",
            LineItems =
            [
                new InvoiceLineItem { Description = "Zero qty item", Quantity = 0, UnitPrice = 500 }
            ],
            Currency = "INR"
        };

        var result = await _billingService.GenerateInvoiceAsync(request);

        result.SubTotal.Should().Be(0);
        result.Total.Should().Be(0);
    }

    [Fact]
    public async Task GenerateInvoice_LargeQuantity_CalculatesCorrectly()
    {
        var request = new InvoiceGenerationRequest
        {
            InvoiceNumber = "INV-LARGE",
            CustomerId = "cust_004",
            LineItems =
            [
                new InvoiceLineItem { Description = "Bulk item", Quantity = 1000, UnitPrice = 10 }
            ],
            Currency = "INR"
        };

        var result = await _billingService.GenerateInvoiceAsync(request);

        result.SubTotal.Should().Be(10000);
        result.Total.Should().Be(10000);
    }

    [Fact]
    public async Task GenerateInvoice_MultipleTaxRates_AccumulatesCorrectly()
    {
        var request = new InvoiceGenerationRequest
        {
            InvoiceNumber = "INV-MULTI-TAX",
            CustomerId = "cust_005",
            LineItems =
            [
                new InvoiceLineItem { Description = "Item 5%", Quantity = 1, UnitPrice = 1000, TaxRate = 5, TaxName = "GST" },
                new InvoiceLineItem { Description = "Item 12%", Quantity = 2, UnitPrice = 500, TaxRate = 12, TaxName = "GST" },
                new InvoiceLineItem { Description = "No tax", Quantity = 1, UnitPrice = 2000 }
            ],
            Currency = "INR"
        };

        var result = await _billingService.GenerateInvoiceAsync(request);

        result.SubTotal.Should().Be(4000);
        result.TaxTotal.Should().Be(170);
        result.Total.Should().Be(4170);
        result.TaxBreakdown.Should().HaveCount(2);
    }

    [Fact]
    public async Task GenerateInvoiceWithTax_CalculatesGst()
    {
        var request = new InvoiceGenerationRequest
        {
            InvoiceNumber = "INV-GST-001",
            CustomerId = "cust_001",
            LineItems =
            [
                new InvoiceLineItem { Description = "Service", Quantity = 1, UnitPrice = 1000, HsnCode = "9991" }
            ],
            Currency = "INR"
        };

        var result = await _billingService.GenerateInvoiceWithTaxAsync(request, "27AAAAA0000A1Z5", "Maharashtra");

        result.SubTotal.Should().Be(1000);
        result.TaxTotal.Should().BeGreaterThan(0);
        result.TaxBreakdown.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GenerateInvoiceWithTax_NullState_ReturnsNoTax()
    {
        var request = new InvoiceGenerationRequest
        {
            InvoiceNumber = "INV-NOSTATE",
            CustomerId = "cust_002",
            LineItems =
            [
                new InvoiceLineItem { Description = "Service", Quantity = 1, UnitPrice = 1000, HsnCode = "9991" }
            ],
            Currency = "INR"
        };

        var result = await _billingService.GenerateInvoiceWithTaxAsync(request, null, null);

        result.SubTotal.Should().Be(1000);
        result.TaxTotal.Should().Be(0);
        result.Total.Should().Be(1000);
    }

    [Fact]
    public async Task CalculateLateFee_WithinGracePeriod_ReturnsZero()
    {
        var dueDate = DateTime.UtcNow.AddDays(-5);
        var asOfDate = DateTime.UtcNow;

        var result = await _billingService.CalculateLateFeeAsync(1000, dueDate, asOfDate, gracePeriodDays: 10);

        result.LateFee.Should().Be(0);
        result.PenaltyAmount.Should().Be(0);
        result.DaysOverdue.Should().Be(0);
    }

    [Fact]
    public async Task CalculateLateFee_ExactlyAtGracePeriod_ReturnsZero()
    {
        var dueDate = DateTime.UtcNow.AddDays(-10);
        var asOfDate = DateTime.UtcNow;

        var result = await _billingService.CalculateLateFeeAsync(1000, dueDate, asOfDate, gracePeriodDays: 10);

        result.LateFee.Should().Be(0);
        result.DaysOverdue.Should().Be(0);
    }

    [Fact]
    public async Task CalculateLateFee_Overdue_ReturnsFee()
    {
        var dueDate = DateTime.UtcNow.AddDays(-20);
        var asOfDate = DateTime.UtcNow;

        var result = await _billingService.CalculateLateFeeAsync(1000, dueDate, asOfDate, lateFeeRate: 0.5m, penaltyRate: 2);

        result.LateFee.Should().BeGreaterThan(0);
        result.PenaltyAmount.Should().BeGreaterThan(0);
        result.DaysOverdue.Should().BeGreaterThan(0);
        result.TotalDue.Should().BeGreaterThan(1000);
    }

    [Fact]
    public async Task CalculateLateFee_ZeroOutstanding_ReturnsZero()
    {
        var dueDate = DateTime.UtcNow.AddDays(-30);
        var asOfDate = DateTime.UtcNow;

        var result = await _billingService.CalculateLateFeeAsync(0, dueDate, asOfDate, lateFeeRate: 0.5m, penaltyRate: 2);

        result.LateFee.Should().Be(0);
        result.PenaltyAmount.Should().Be(0);
        result.DaysOverdue.Should().Be(30);
        result.TotalDue.Should().Be(0);
    }

    [Fact]
    public async Task CalculateLateFee_DefaultRates_AppliesCorrectly()
    {
        var dueDate = DateTime.UtcNow.AddDays(-10);
        var asOfDate = DateTime.UtcNow;

        var result = await _billingService.CalculateLateFeeAsync(10000, dueDate, asOfDate);

        result.DaysOverdue.Should().Be(10);
        result.LateFee.Should().BeGreaterThan(0);
        result.PenaltyAmount.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GenerateInstallmentPlan_CreatesCorrectSchedules()
    {
        var result = await _billingService.GenerateInstallmentPlanAsync(12000, 6, DateTime.UtcNow, "monthly");

        result.Should().HaveCount(6);
        result[0].InstallmentNumber.Should().Be(1);
        result[^1].InstallmentNumber.Should().Be(6);
        result[0].Status.Should().Be("pending");
    }

    [Fact]
    public async Task GenerateInstallmentPlan_WithInterest()
    {
        var result = await _billingService.GenerateInstallmentPlanAsync(12000, 12, DateTime.UtcNow, "monthly", 12);

        result.Should().HaveCount(12);
        var totalAmount = result.Sum(r => r.Amount);
        totalAmount.Should().BeGreaterThan(12000);
    }

    [Fact]
    public async Task GenerateInstallmentPlan_SingleInstallment_ReturnsOneSchedule()
    {
        var result = await _billingService.GenerateInstallmentPlanAsync(5000, 1, DateTime.UtcNow, "monthly");

        result.Should().HaveCount(1);
        result[0].InstallmentNumber.Should().Be(1);
        result[0].Amount.Should().Be(5000);
    }

    [Fact]
    public async Task GenerateInstallmentPlan_BiweeklyFrequency_CreatesSchedule()
    {
        var startDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        var result = await _billingService.GenerateInstallmentPlanAsync(4000, 4, startDate, "biweekly");

        result.Should().HaveCount(4);
        result[0].DueDate.Should().Be(startDate);
        result[1].DueDate.Should().Be(startDate.AddDays(14));
        result[2].DueDate.Should().Be(startDate.AddDays(28));
        result[3].DueDate.Should().Be(startDate.AddDays(42));
    }

    [Fact]
    public async Task GenerateInstallmentPlan_QuarterlyFrequency_CreatesSchedule()
    {
        var startDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        var result = await _billingService.GenerateInstallmentPlanAsync(12000, 4, startDate, "quarterly");

        result.Should().HaveCount(4);
        result[0].DueDate.Should().Be(startDate);
        result[1].DueDate.Should().Be(startDate.AddMonths(3));
        result[2].DueDate.Should().Be(startDate.AddMonths(6));
        result[3].DueDate.Should().Be(startDate.AddMonths(9));
    }

    [Fact]
    public async Task GenerateInstallmentPlan_ZeroInterest_AmountsMatchTotal()
    {
        var result = await _billingService.GenerateInstallmentPlanAsync(1000, 5, DateTime.UtcNow, "monthly", 0);

        result.Should().HaveCount(5);
        var total = result.Sum(r => r.Amount);
        total.Should().Be(1000);
    }

    [Fact]
    public async Task CalculatePenalty_AppliesCorrectly()
    {
        var result = await _billingService.CalculatePenaltyAsync(1000, 30, 0.1m);

        result.Should().Be(30);
    }

    [Fact]
    public async Task CalculatePenalty_WithMaxCap()
    {
        var result = await _billingService.CalculatePenaltyAsync(10000, 100, 0.5m, 200);

        result.Should().Be(200);
    }

    [Fact]
    public async Task CalculatePenalty_ZeroRate_ReturnsZero()
    {
        var result = await _billingService.CalculatePenaltyAsync(5000, 30, 0);

        result.Should().Be(0);
    }

    [Fact]
    public async Task CalculatePenalty_ZeroOutstanding_ReturnsZero()
    {
        var result = await _billingService.CalculatePenaltyAsync(0, 30, 0.1m);

        result.Should().Be(0);
    }

    [Fact]
    public void IsWithinGracePeriod_ReturnsCorrectly()
    {
        var dueDate = DateTime.UtcNow.AddDays(-3);
        _billingService.IsWithinGracePeriod(dueDate, DateTime.UtcNow, 5).Should().BeTrue();
        _billingService.IsWithinGracePeriod(dueDate, DateTime.UtcNow, 2).Should().BeFalse();
    }

    [Theory]
    [InlineData("monthly")]
    [InlineData("weekly")]
    [InlineData("yearly")]
    public void CalculateNextBillingDate_ReturnsCorrectDate(string frequency)
    {
        var currentDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var nextDate = _billingService.CalculateNextBillingDate(currentDate, frequency);

        var expected = frequency switch
        {
            "monthly" => currentDate.AddMonths(1),
            "weekly" => currentDate.AddDays(7),
            "yearly" => currentDate.AddYears(1),
            _ => currentDate.AddMonths(1)
        };

        nextDate.Should().Be(expected);
    }

    [Theory]
    [InlineData("daily")]
    [InlineData("biweekly")]
    [InlineData("quarterly")]
    [InlineData("halfyearly")]
    public void CalculateNextBillingDate_VariousFrequencies(string frequency)
    {
        var currentDate = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc);
        var nextDate = _billingService.CalculateNextBillingDate(currentDate, frequency);

        var expected = frequency switch
        {
            "daily" => currentDate.AddDays(1),
            "biweekly" => currentDate.AddDays(14),
            "quarterly" => currentDate.AddMonths(3),
            "halfyearly" => currentDate.AddMonths(6),
            _ => currentDate.AddMonths(1)
        };

        nextDate.Should().Be(expected);
    }

    [Fact]
    public void CalculateNextBillingDate_WithInterval_ReturnsCorrectDate()
    {
        var currentDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        var result = _billingService.CalculateNextBillingDate(currentDate, "monthly", 3);

        result.Should().Be(currentDate.AddMonths(3));
    }

    [Fact]
    public void CalculateNextBillingDate_UnknownFrequency_DefaultsToMonthly()
    {
        var currentDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        var result = _billingService.CalculateNextBillingDate(currentDate, "unknown_freq");

        result.Should().Be(currentDate.AddMonths(1));
    }

    [Fact]
    public async Task GenerateInvoice_ConsecutiveCalls_ReturnIndependentResults()
    {
        var request1 = new InvoiceGenerationRequest
        {
            InvoiceNumber = "INV-CONSEC-1",
            CustomerId = "cust_010",
            LineItems = [new InvoiceLineItem { Description = "Item A", Quantity = 1, UnitPrice = 500 }],
            Currency = "INR"
        };

        var request2 = new InvoiceGenerationRequest
        {
            InvoiceNumber = "INV-CONSEC-2",
            CustomerId = "cust_010",
            LineItems = [new InvoiceLineItem { Description = "Item B", Quantity = 3, UnitPrice = 200 }],
            Currency = "INR"
        };

        var result1 = await _billingService.GenerateInvoiceAsync(request1);
        var result2 = await _billingService.GenerateInvoiceAsync(request2);

        result1.SubTotal.Should().Be(500);
        result2.SubTotal.Should().Be(600);
        result1.InvoiceNumber.Should().Be("INV-CONSEC-1");
        result2.InvoiceNumber.Should().Be("INV-CONSEC-2");
    }

    [Fact]
    public async Task GenerateInvoiceWithTax_UsesCorrectGstRate()
    {
        var request = new InvoiceGenerationRequest
        {
            InvoiceNumber = "INV-GST",
            CustomerId = "cust_011",
            LineItems =
            [
                new InvoiceLineItem { Description = "Taxable service", Quantity = 1, UnitPrice = 1000, HsnCode = "8504" }
            ],
            Currency = "INR"
        };

        var result = await _billingService.GenerateInvoiceWithTaxAsync(request, "27AAAAA0000A1Z5", "Maharashtra");

        result.SubTotal.Should().Be(1000);
        result.TaxTotal.Should().Be(120);
        result.TaxBreakdown.Should().HaveCount(1);
        result.TaxBreakdown[0].Name.Should().Be("IGST");
    }
}
