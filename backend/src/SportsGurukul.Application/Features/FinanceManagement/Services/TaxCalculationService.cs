using SportsGurukul.Application.Common.Interfaces.Finance.Services;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.FinanceManagement.Services;

public class TaxCalculationService : ITaxCalculationService
{
    // Standard tax rates: GST 18% for most services
    private static readonly Dictionary<string, (string Name, decimal Rate)> TaxRates = new(StringComparer.OrdinalIgnoreCase)
    {
        { "GST18", ("GST 18%", 0.18m) },
        { "GST12", ("GST 12%", 0.12m) },
        { "GST5", ("GST 5%", 0.05m) },
        { "IGST18", ("IGST 18%", 0.18m) },
        { "NIL", ("No Tax", 0m) },
    };

    public async Task<Result<decimal>> CalculateTaxAsync(decimal amount, string taxCode, CancellationToken cancellationToken)
    {
        if (!TaxRates.TryGetValue(taxCode, out var rate))
            return Result<decimal>.Failure($"Unknown tax code: {taxCode}");

        var taxAmount = amount * rate.Rate;
        return Result<decimal>.Success(taxAmount);
    }

    public async Task<Result<IReadOnlyList<TaxLineItem>>> CalculateInvoiceTaxesAsync(decimal subTotal, string? currency, CancellationToken cancellationToken)
    {
        // Default: apply 18% GST
        var rate = TaxRates["GST18"];
        var taxAmount = subTotal * rate.Rate;

        var items = new List<TaxLineItem>
        {
            new(rate.Name, "GST18", rate.Rate, taxAmount)
        };

        return Result<IReadOnlyList<TaxLineItem>>.Success(items);
    }
}
