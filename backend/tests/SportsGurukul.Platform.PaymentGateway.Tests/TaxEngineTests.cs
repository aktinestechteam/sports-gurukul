using Microsoft.Extensions.Logging.Abstractions;
using SportsGurukul.Platform.PaymentGateway.Tax;

namespace SportsGurukul.Platform.PaymentGateway.Tests;

public class TaxEngineTests
{
    private readonly ITaxEngine _taxEngine;

    public TaxEngineTests()
    {
        _taxEngine = new TaxEngine(NullLogger<TaxEngine>.Instance);
    }

    [Fact]
    public async Task CalculateGst_IntraState_ReturnsCgstSgst()
    {
        var result = await _taxEngine.CalculateGstAsync(1000, "9991", "Maharashtra", "Maharashtra");

        Assert.Equal(180, result.TotalTax); // 18% of 1000
        Assert.Equal(2, result.Breakdown.Count);
        Assert.Contains(result.Breakdown, b => b.Component == "CGST");
        Assert.Contains(result.Breakdown, b => b.Component == "SGST");
        Assert.Equal(90, result.Breakdown[0].Amount); // CGST = 9%
        Assert.Equal(90, result.Breakdown[1].Amount); // SGST = 9%
    }

    [Fact]
    public async Task CalculateGst_InterState_ReturnsIgst()
    {
        var result = await _taxEngine.CalculateGstAsync(1000, "9991", "Maharashtra", "Gujarat");

        Assert.Equal(180, result.TotalTax); // 18% of 1000
        Assert.Single(result.Breakdown);
        Assert.Equal("IGST", result.Breakdown[0].Component);
        Assert.Equal(180, result.Breakdown[0].Amount);
    }

    [Theory]
    [InlineData("6101", 5)]
    [InlineData("8504", 12)]
    [InlineData("9991", 18)]
    [InlineData("9506", 12)]
    [InlineData("0000", 18)]
    public async Task CalculateGst_CorrectRateByHsn(string hsnCode, decimal expectedRate)
    {
        var result = await _taxEngine.CalculateGstAsync(100, hsnCode, "Maharashtra", "Gujarat");

        var igst = result.Breakdown[0];
        Assert.Equal(expectedRate, igst.Rate);
    }

    [Fact]
    public async Task IsTaxExempt_ExemptHsn_ReturnsTrue()
    {
        var result = await _taxEngine.IsTaxExemptAsync("0001");
        Assert.True(result);
    }

    [Fact]
    public async Task IsTaxExempt_NonExemptHsn_ReturnsFalse()
    {
        var result = await _taxEngine.IsTaxExemptAsync("9991");
        Assert.False(result);
    }

    [Fact]
    public async Task IsTaxExempt_EducationalCustomer_ReturnsTrue()
    {
        var result = await _taxEngine.IsTaxExemptAsync("9991", "educational");
        Assert.True(result);
    }

    [Fact]
    public async Task CalculateTaxAsync_ExemptHsn_ReturnsZeroTax()
    {
        var request = new TaxCalculationRequest
        {
            TaxableAmount = 1000,
            HsnCode = "0001",
            CustomerState = "Maharashtra",
            BusinessState = "Maharashtra"
        };

        var result = await _taxEngine.CalculateTaxAsync(request);
        Assert.True(result.IsExempt);
        Assert.Equal(0, result.TotalTax);
    }

    [Fact]
    public async Task GetApplicableTaxRate_ByHsn()
    {
        var rate = await _taxEngine.GetApplicableTaxRateAsync("8504", "Maharashtra", "Gujarat");
        Assert.Equal(12, rate);
    }

    [Fact]
    public async Task GetApplicableTaxRate_ExemptHsn_ReturnsZero()
    {
        var rate = await _taxEngine.GetApplicableTaxRateAsync("0001", "Maharashtra", "Gujarat");
        Assert.Equal(0, rate);
    }

    [Fact]
    public async Task CalculateGstBreakdown_IntraState()
    {
        var result = await _taxEngine.CalculateGstBreakdownAsync(1000, 18, "intra_state");
        Assert.Equal(180, result.Amount);
        Assert.Contains("CGST", result.Component);
    }

    [Fact]
    public async Task CalculateGstBreakdown_InterState()
    {
        var result = await _taxEngine.CalculateGstBreakdownAsync(1000, 18, "inter_state");
        Assert.Equal(180, result.Amount);
        Assert.Equal("IGST", result.Component);
    }

    [Theory]
    [InlineData(100, 5, 5)]
    [InlineData(200, 12, 24)]
    [InlineData(500, 18, 90)]
    [InlineData(1000, 18, 180)]
    public async Task CalculateGst_CorrectAmounts(decimal amount, decimal rate, decimal expectedTax)
    {
        // Use a rate that maps to the expected HSN code
        var hsnCode = rate switch
        {
            5 => "6101",
            12 => "8504",
            18 => "9991",
            28 => "9999",
            _ => "9991"
        };

        var result = await _taxEngine.CalculateGstAsync(amount, hsnCode, "Maharashtra", "Gujarat");
        Assert.Equal(expectedTax, result.TotalTax);
    }
}
