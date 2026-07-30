using FluentAssertions;
using SportsGurukul.Application.Common.Interfaces.Finance.Services;
using SportsGurukul.Application.Features.FinanceManagement.Services;

namespace SportsGurukul.Application.Tests.Finance.Services;

public class TaxCalculationServiceTests
{
    private readonly TaxCalculationService _service;

    public TaxCalculationServiceTests()
    {
        _service = new TaxCalculationService();
    }

    #region CalculateTaxAsync

    [Fact]
    public async Task CalculateTaxAsync_GST18_Returns18Percent()
    {
        var result = await _service.CalculateTaxAsync(1000m, "GST18", CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(180m);
    }

    [Fact]
    public async Task CalculateTaxAsync_GST12_Returns12Percent()
    {
        var result = await _service.CalculateTaxAsync(1000m, "GST12", CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(120m);
    }

    [Fact]
    public async Task CalculateTaxAsync_GST5_Returns5Percent()
    {
        var result = await _service.CalculateTaxAsync(2000m, "GST5", CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(100m);
    }

    [Fact]
    public async Task CalculateTaxAsync_IGST18_Returns18Percent()
    {
        var result = await _service.CalculateTaxAsync(500m, "IGST18", CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(90m);
    }

    [Fact]
    public async Task CalculateTaxAsync_NIL_ReturnsZero()
    {
        var result = await _service.CalculateTaxAsync(1000m, "NIL", CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(0m);
    }

    [Fact]
    public async Task CalculateTaxAsync_UnknownCode_ReturnsFailure()
    {
        var result = await _service.CalculateTaxAsync(1000m, "UNKNOWN", CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Unknown tax code: UNKNOWN");
    }

    [Fact]
    public async Task CalculateTaxAsync_ZeroAmount_ReturnsZero()
    {
        var result = await _service.CalculateTaxAsync(0m, "GST18", CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(0m);
    }

    [Fact]
    public async Task CalculateTaxAsync_CaseInsensitiveCode_WorksCorrectly()
    {
        var result = await _service.CalculateTaxAsync(1000m, "gst18", CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(180m);
    }

    #endregion

    #region CalculateInvoiceTaxesAsync

    [Fact]
    public async Task CalculateInvoiceTaxesAsync_DefaultCurrency_ReturnsGST18()
    {
        var result = await _service.CalculateInvoiceTaxesAsync(1000m, "INR", CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value[0].TaxName.Should().Be("GST 18%");
        result.Value[0].TaxCode.Should().Be("GST18");
        result.Value[0].TaxRate.Should().Be(0.18m);
        result.Value[0].TaxAmount.Should().Be(180m);
    }

    [Fact]
    public async Task CalculateInvoiceTaxesAsync_NullCurrency_ReturnsDefaultGST18()
    {
        var result = await _service.CalculateInvoiceTaxesAsync(500m, null, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value[0].TaxAmount.Should().Be(90m);
    }

    [Fact]
    public async Task CalculateInvoiceTaxesAsync_ZeroSubTotal_ReturnsZeroTax()
    {
        var result = await _service.CalculateInvoiceTaxesAsync(0m, "INR", CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value[0].TaxAmount.Should().Be(0m);
    }

    [Fact]
    public async Task CalculateInvoiceTaxesAsync_ReturnsReadOnlyList()
    {
        var result = await _service.CalculateInvoiceTaxesAsync(100m, "USD", CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeAssignableTo<IReadOnlyList<TaxLineItem>>();
    }

    [Fact]
    public async Task CalculateInvoiceTaxesAsync_LargeAmount_CalculatesCorrectly()
    {
        var result = await _service.CalculateInvoiceTaxesAsync(1000000m, "INR", CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value[0].TaxAmount.Should().Be(180000m);
    }

    #endregion
}
