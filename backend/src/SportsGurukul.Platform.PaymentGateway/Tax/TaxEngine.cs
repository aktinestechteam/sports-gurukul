using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.PaymentGateway.Models;

namespace SportsGurukul.Platform.PaymentGateway.Tax;

public class TaxEngine : ITaxEngine
{
    private readonly ILogger<TaxEngine> _logger;
    private static readonly Dictionary<string, decimal> GstRatesByHsn = new()
    {
        ["0000"] = 18m,
        ["6101"] = 5m,
        ["6201"] = 5m,
        ["6301"] = 5m,
        ["6401"] = 5m,
        ["6402"] = 5m,
        ["6403"] = 18m,
        ["6404"] = 18m,
        ["6405"] = 18m,
        ["8504"] = 12m,
        ["8517"] = 18m,
        ["8523"] = 18m,
        ["8542"] = 18m,
        ["8801"] = 5m,
        ["8802"] = 5m,
        ["8803"] = 12m,
        ["8804"] = 12m,
        ["9503"] = 12m,
        ["9504"] = 18m,
        ["9506"] = 12m,
        ["9507"] = 5m,
        ["9701"] = 12m,
        ["9702"] = 12m,
        ["9703"] = 12m,
        ["9704"] = 12m,
        ["9705"] = 12m,
        ["9706"] = 12m,
        ["9991"] = 18m,
        ["9992"] = 18m,
        ["9993"] = 18m,
        ["9994"] = 18m,
        ["9995"] = 18m,
        ["9996"] = 18m,
        ["9997"] = 18m,
        ["9998"] = 18m,
        ["9999"] = 18m,
    };

    private static readonly HashSet<string> ExemptHsnCodes = new(StringComparer.OrdinalIgnoreCase)
    {
        "0001", "0002", "0003"
    };

    public TaxEngine(ILogger<TaxEngine> logger)
    {
        _logger = logger;
    }

    public async Task<TaxCalculationResult> CalculateTaxAsync(
        TaxCalculationRequest request,
        CancellationToken cancellationToken = default)
    {
        if (await IsTaxExemptAsync(request.HsnCode, cancellationToken: cancellationToken))
        {
            return new TaxCalculationResult
            {
                TaxableAmount = request.TaxableAmount,
                TotalTax = 0,
                TaxType = "GST",
                EffectiveRate = 0,
                IsExempt = true
            };
        }

        return await CalculateGstAsync(
            request.TaxableAmount,
            request.HsnCode,
            request.CustomerState,
            request.BusinessState,
            request.IsReverseCharge,
            cancellationToken);
    }

    public Task<TaxCalculationResult> CalculateGstAsync(
        decimal taxableAmount,
        string hsnCode,
        string customerState,
        string businessState,
        bool isReverseCharge = false,
        CancellationToken cancellationToken = default)
    {
        var gstRate = GetGstRate(hsnCode);
        var isIntraState = string.Equals(customerState, businessState, StringComparison.OrdinalIgnoreCase);

        var breakdown = isIntraState
            ? CalculateIntraStateGst(taxableAmount, gstRate)
            : CalculateInterStateGst(taxableAmount, gstRate);

        var result = new TaxCalculationResult
        {
            TaxableAmount = taxableAmount,
            TotalTax = breakdown.Sum(b => b.Amount),
            Breakdown = breakdown,
            TaxType = "GST",
            EffectiveRate = gstRate,
            IsReverseCharge = isReverseCharge,
            IsExempt = false
        };

        _logger.LogDebug(
            "GST calculated: Amount={Amount}, Rate={Rate}%, HSN={Hsn}, Type={Type}",
            taxableAmount, gstRate, hsnCode, isIntraState ? "Intra-State" : "Inter-State");

        return Task.FromResult(result);
    }

    public Task<GstBreakdown> CalculateGstBreakdownAsync(
        decimal taxableAmount,
        decimal gstRate,
        string supplyType,
        CancellationToken cancellationToken = default)
    {
        var isIntraState = supplyType?.Equals("intra_state", StringComparison.OrdinalIgnoreCase) ?? true;
        var gstAmount = Math.Round(taxableAmount * gstRate / 100, 2);

        if (isIntraState)
        {
            var halfGst = Math.Round(gstAmount / 2, 2);
            return Task.FromResult(new GstBreakdown
            {
                Component = "CGST + SGST",
                Rate = gstRate,
                Amount = gstAmount,
                Description = $"CGST @ {gstRate / 2}% + SGST @ {gstRate / 2}%"
            });
        }

        return Task.FromResult(new GstBreakdown
        {
            Component = "IGST",
            Rate = gstRate,
            Amount = gstAmount,
            Description = $"IGST @ {gstRate}%"
        });
    }

    public Task<bool> IsTaxExemptAsync(
        string hsnCode,
        string? customerType = null,
        CancellationToken cancellationToken = default)
    {
        if (ExemptHsnCodes.Contains(hsnCode))
            return Task.FromResult(true);

        if (customerType?.Equals("educational", StringComparison.OrdinalIgnoreCase) == true)
            return Task.FromResult(true);

        return Task.FromResult(false);
    }

    public async Task<decimal> GetApplicableTaxRateAsync(
        string hsnCode,
        string customerState,
        string businessState,
        CancellationToken cancellationToken = default)
    {
        if (await IsTaxExemptAsync(hsnCode, cancellationToken: cancellationToken))
            return 0;

        return GetGstRate(hsnCode);
    }

    private static List<GstBreakdown> CalculateIntraStateGst(decimal taxableAmount, decimal totalRate)
    {
        var halfRate = totalRate / 2;
        var cgst = Math.Round(taxableAmount * halfRate / 100, 2);
        var sgst = Math.Round(taxableAmount * halfRate / 100, 2);

        return
        [
            new GstBreakdown { Component = "CGST", Rate = halfRate, Amount = cgst, Description = $"Central GST @ {halfRate}%" },
            new GstBreakdown { Component = "SGST", Rate = halfRate, Amount = sgst, Description = $"State GST @ {halfRate}%" }
        ];
    }

    private static List<GstBreakdown> CalculateInterStateGst(decimal taxableAmount, decimal totalRate)
    {
        var igst = Math.Round(taxableAmount * totalRate / 100, 2);

        return
        [
            new GstBreakdown { Component = "IGST", Rate = totalRate, Amount = igst, Description = $"Integrated GST @ {totalRate}%" }
        ];
    }

    private static decimal GetGstRate(string hsnCode)
    {
        if (string.IsNullOrWhiteSpace(hsnCode)) return 18;
        var code = hsnCode.Trim();
        return GstRatesByHsn.TryGetValue(code, out var rate) ? rate : 18;
    }
}
