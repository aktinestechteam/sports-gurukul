using System.Diagnostics;
using FluentAssertions;
using SportsGurukul.Application.Features.FinanceManagement.Commands.Invoice;
using SportsGurukul.Application.Features.FinanceManagement.Commands.Payment;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;
using SportsGurukul.Application.Features.FinanceManagement.Queries;
using SportsGurukul.Domain.Enums.Finance;
using SportsGurukul.Finance.IntegrationTests.Fixtures;
using SportsGurukul.Finance.IntegrationTests.Helpers;
using SportsGurukul.Finance.IntegrationTests.Seed;
using Xunit;

namespace SportsGurukul.Finance.IntegrationTests.Tests;

[Collection("Finance")]
public class PerformanceBenchmarks : FinanceTestBase
{
    public PerformanceBenchmarks(FinanceWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task Invoice_Creation_Under300ms()
    {
        var lineItems = new List<CreateInvoiceLineItemDto>
        {
            new("Coaching Fee", "Coaching", null, 1, 1000m, null)
        };
        var command = new CreateInvoiceCommand(
            "Performance Test Invoice", null, "INR",
            FinanceTestIds.AthleteUserId, null, lineItems, null, null);

        var stopwatch = Stopwatch.StartNew();
        var result = await SendAsync(command);
        stopwatch.Stop();

        result.IsSuccess.Should().BeTrue();
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(300);
    }

    [Fact]
    public async Task Payment_Creation_Under300ms()
    {
        var lineItems = new List<CreateInvoiceLineItemDto>
        {
            new("Coaching Fee", "Coaching", null, 1, 5000m, null)
        };
        var invoiceCommand = new CreateInvoiceCommand(
            "Payment Performance Test", null, "INR",
            FinanceTestIds.AthleteUserId, null, lineItems, null, null);
        var invoiceResult = await SendAsync(invoiceCommand);
        invoiceResult.IsSuccess.Should().BeTrue();

        var paymentCommand = new InitiatePaymentCommand(
            invoiceResult.Value!.Id, 5000m, PaymentMethod.UPI, null, "Performance payment");

        var stopwatch = Stopwatch.StartNew();
        var paymentResult = await SendAsync(paymentCommand);
        stopwatch.Stop();

        paymentResult.IsSuccess.Should().BeTrue();
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(300);
    }

    [Fact]
    public async Task GetProviders_Under100ms()
    {
        var stopwatch = Stopwatch.StartNew();
        var response = await GetAsync(AdminClient, "api/v1/payments/providers");
        stopwatch.Stop();

        response.IsSuccessStatusCode.Should().BeTrue();
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(100);
    }

    [Fact]
    public async Task Dashboard_Under500ms()
    {
        var query = new GetFinanceDashboardQuery();

        var stopwatch = Stopwatch.StartNew();
        var result = await SendAsync(query);
        stopwatch.Stop();

        result.IsSuccess.Should().BeTrue();
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(500);
    }

    [Fact]
    public async Task Report_Under1Second()
    {
        var query = new GetRevenueQuery(null, null);

        var stopwatch = Stopwatch.StartNew();
        var result = await SendAsync(query);
        stopwatch.Stop();

        result.IsSuccess.Should().BeTrue();
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(1000);
    }

    [Fact]
    public async Task BulkInvoiceCreation_Under2Seconds()
    {
        var lineItems = new List<CreateInvoiceLineItemDto>
        {
            new("Coaching Fee", "Coaching", null, 1, 1000m, null)
        };

        var stopwatch = Stopwatch.StartNew();
        for (var i = 0; i < 10; i++)
        {
            var command = new CreateInvoiceCommand(
                $"Bulk Invoice {i}", null, "INR",
                FinanceTestIds.AthleteUserId, null, lineItems, null, null);
            var result = await SendAsync(command);
            result.IsSuccess.Should().BeTrue();
        }
        stopwatch.Stop();

        stopwatch.ElapsedMilliseconds.Should().BeLessThan(2000);
    }
}
