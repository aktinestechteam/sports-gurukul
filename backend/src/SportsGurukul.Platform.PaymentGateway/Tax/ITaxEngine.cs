using SportsGurukul.Platform.PaymentGateway.Models;

namespace SportsGurukul.Platform.PaymentGateway.Tax;

public interface ITaxEngine
{
    Task<TaxCalculationResult> CalculateTaxAsync(
        TaxCalculationRequest request,
        CancellationToken cancellationToken = default);

    Task<TaxCalculationResult> CalculateGstAsync(
        decimal taxableAmount,
        string hsnCode,
        string customerState,
        string businessState,
        bool isReverseCharge = false,
        CancellationToken cancellationToken = default);

    Task<GstBreakdown> CalculateGstBreakdownAsync(
        decimal taxableAmount,
        decimal gstRate,
        string supplyType,
        CancellationToken cancellationToken = default);

    Task<bool> IsTaxExemptAsync(
        string hsnCode,
        string? customerType = null,
        CancellationToken cancellationToken = default);

    Task<decimal> GetApplicableTaxRateAsync(
        string hsnCode,
        string customerState,
        string businessState,
        CancellationToken cancellationToken = default);
}

public class TaxCalculationRequest
{
    public decimal TaxableAmount { get; set; }
    public string HsnCode { get; set; } = string.Empty;
    public string CustomerState { get; set; } = string.Empty;
    public string BusinessState { get; set; } = string.Empty;
    public string? CustomerGstin { get; set; }
    public string? BusinessGstin { get; set; }
    public decimal? CustomTaxRate { get; set; }
    public bool IsReverseCharge { get; set; }
    public Dictionary<string, string>? Metadata { get; set; }
}

public class TaxCalculationResult
{
    public decimal TaxableAmount { get; set; }
    public decimal TotalTax { get; set; }
    public List<GstBreakdown> Breakdown { get; set; } = [];
    public string TaxType { get; set; } = "GST";
    public decimal EffectiveRate { get; set; }
    public bool IsReverseCharge { get; set; }
    public bool IsExempt { get; set; }
}

public class GstBreakdown
{
    public string Component { get; set; } = string.Empty;
    public decimal Rate { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
}
