using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Common.Interfaces.Finance.Services;

public interface ITaxCalculationService
{
    Task<Result<decimal>> CalculateTaxAsync(decimal amount, string taxCode, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<TaxLineItem>>> CalculateInvoiceTaxesAsync(decimal subTotal, string? currency = null, CancellationToken cancellationToken = default);
}

public record TaxLineItem(string TaxName, string TaxCode, decimal TaxRate, decimal TaxAmount);
