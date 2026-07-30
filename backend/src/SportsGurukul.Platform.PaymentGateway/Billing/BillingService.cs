using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.PaymentGateway.Models;
using SportsGurukul.Platform.PaymentGateway.Tax;

namespace SportsGurukul.Platform.PaymentGateway.Billing;

public class BillingService : IBillingService
{
    private readonly ITaxEngine _taxEngine;
    private readonly ILogger<BillingService> _logger;

    public BillingService(ITaxEngine taxEngine, ILogger<BillingService> logger)
    {
        _taxEngine = taxEngine;
        _logger = logger;
    }

    public Task<InvoiceResult> GenerateInvoiceAsync(
        InvoiceGenerationRequest request,
        CancellationToken cancellationToken = default)
    {
        var subTotal = request.LineItems.Sum(i => i.Quantity * i.UnitPrice);
        var taxTotal = 0m;
        var taxBreakdown = new List<TaxBreakdown>();
        var discountTotal = 0m;

        foreach (var item in request.LineItems)
        {
            if (item.TaxRate.HasValue && item.TaxRate > 0)
            {
                var itemTax = item.Quantity * item.UnitPrice * item.TaxRate.Value / 100;
                taxTotal += itemTax;
                taxBreakdown.Add(new TaxBreakdown
                {
                    Name = item.TaxName ?? $"Tax {item.TaxRate:P}",
                    Rate = item.TaxRate.Value,
                    Amount = itemTax,
                    Type = "GST"
                });
            }
        }

        var total = subTotal + taxTotal - discountTotal;

        return Task.FromResult(new InvoiceResult
        {
            InvoiceNumber = request.InvoiceNumber,
            SubTotal = subTotal,
            TaxTotal = taxTotal,
            DiscountTotal = discountTotal,
            Total = total,
            AmountDue = total,
            Currency = request.Currency,
            TaxBreakdown = taxBreakdown,
            GeneratedAt = DateTime.UtcNow
        });
    }

    public async Task<InvoiceResult> GenerateInvoiceWithTaxAsync(
        InvoiceGenerationRequest request,
        string? customerGstin,
        string? customerState,
        CancellationToken cancellationToken = default)
    {
        var subTotal = request.LineItems.Sum(i => i.Quantity * i.UnitPrice);
        var totalTax = 0m;
        var taxBreakdown = new List<TaxBreakdown>();

        foreach (var item in request.LineItems)
        {
            var itemTotal = item.Quantity * item.UnitPrice;
            var hsnCode = item.HsnCode ?? "0000";

            if (!string.IsNullOrEmpty(customerState))
            {
                var taxResult = await _taxEngine.CalculateGstAsync(
                    itemTotal,
                    hsnCode,
                    customerState,
                    "Gujarat",
                    false,
                    cancellationToken);

                totalTax += taxResult.TotalTax;
                foreach (var gst in taxResult.Breakdown)
                {
                    taxBreakdown.Add(new TaxBreakdown
                    {
                        Name = gst.Component,
                        Rate = gst.Rate,
                        Amount = gst.Amount,
                        Type = "GST"
                    });
                }
            }
        }

        var total = subTotal + totalTax;

        return new InvoiceResult
        {
            InvoiceNumber = request.InvoiceNumber,
            SubTotal = subTotal,
            TaxTotal = totalTax,
            DiscountTotal = 0,
            Total = total,
            AmountDue = total,
            Currency = request.Currency,
            TaxBreakdown = taxBreakdown,
            GeneratedAt = DateTime.UtcNow
        };
    }

    public Task<LateFeeResult> CalculateLateFeeAsync(
        decimal outstandingAmount,
        DateTime dueDate,
        DateTime asOfDate,
        decimal? lateFeeRate = null,
        decimal? penaltyRate = null,
        int? gracePeriodDays = null,
        CancellationToken cancellationToken = default)
    {
        var graceDays = gracePeriodDays ?? 0;
        var effectiveDueDate = dueDate.AddDays(graceDays);

        if (asOfDate <= effectiveDueDate)
        {
            return Task.FromResult(new LateFeeResult
            {
                LateFee = 0,
                PenaltyAmount = 0,
                DaysOverdue = 0,
                TotalDue = outstandingAmount,
                CalculatedAt = DateTime.UtcNow
            });
        }

        var daysOverdue = (asOfDate - effectiveDueDate).Days;
        var effectiveLateFeeRate = lateFeeRate ?? 0.02m;
        var effectivePenaltyRate = penaltyRate ?? 0.05m;

        var lateFee = outstandingAmount * effectiveLateFeeRate / 100 * daysOverdue;
        var penalty = outstandingAmount * effectivePenaltyRate / 100;

        var totalDue = outstandingAmount + lateFee + penalty;

        return Task.FromResult(new LateFeeResult
        {
            LateFee = Math.Round(lateFee, 2),
            PenaltyAmount = Math.Round(penalty, 2),
            DaysOverdue = daysOverdue,
            TotalDue = Math.Round(totalDue, 2),
            CalculatedAt = DateTime.UtcNow
        });
    }

    public Task<List<InstallmentSchedule>> GenerateInstallmentPlanAsync(
        decimal totalAmount,
        int numberOfInstallments,
        DateTime startDate,
        string frequency = "monthly",
        decimal? interestRate = null,
        CancellationToken cancellationToken = default)
    {
        var schedule = new List<InstallmentSchedule>();
        var baseInstallment = Math.Round(totalAmount / numberOfInstallments, 2);
        var remainder = totalAmount - (baseInstallment * numberOfInstallments);

        var currentDate = startDate;

        for (int i = 1; i <= numberOfInstallments; i++)
        {
            var amount = baseInstallment;

            if (interestRate.HasValue && interestRate > 0)
            {
                var outstandingPrincipal = totalAmount - (baseInstallment * (i - 1));
                var interestAmount = outstandingPrincipal * (interestRate.Value / 100 / 12);
                amount += Math.Round(interestAmount, 2);
            }

            if (i == numberOfInstallments)
                amount += remainder;

            schedule.Add(new InstallmentSchedule
            {
                InstallmentNumber = i,
                Amount = Math.Round(amount, 2),
                DueDate = currentDate,
                Status = "pending"
            });

            currentDate = frequency.ToLowerInvariant() switch
            {
                "weekly" => currentDate.AddDays(7),
                "biweekly" => currentDate.AddDays(14),
                "monthly" => currentDate.AddMonths(1),
                "quarterly" => currentDate.AddMonths(3),
                "halfyearly" => currentDate.AddMonths(6),
                "yearly" => currentDate.AddYears(1),
                _ => currentDate.AddMonths(1)
            };
        }

        return Task.FromResult(schedule);
    }

    public Task<decimal> CalculatePenaltyAsync(
        decimal outstandingAmount,
        int daysOverdue,
        decimal penaltyRate,
        decimal? maxPenalty = null,
        CancellationToken cancellationToken = default)
    {
        var penalty = outstandingAmount * penaltyRate / 100 * daysOverdue;

        if (maxPenalty.HasValue)
            penalty = Math.Min(penalty, maxPenalty.Value);

        return Task.FromResult(Math.Round(penalty, 2));
    }

    public bool IsWithinGracePeriod(DateTime dueDate, DateTime asOfDate, int gracePeriodDays)
    {
        return asOfDate <= dueDate.AddDays(gracePeriodDays);
    }

    public DateTime CalculateNextBillingDate(DateTime currentDate, string frequency, int interval = 1)
    {
        return frequency.ToLowerInvariant() switch
        {
            "daily" => currentDate.AddDays(interval),
            "weekly" => currentDate.AddDays(7 * interval),
            "biweekly" => currentDate.AddDays(14 * interval),
            "monthly" => currentDate.AddMonths(interval),
            "quarterly" => currentDate.AddMonths(3 * interval),
            "halfyearly" => currentDate.AddMonths(6 * interval),
            "yearly" => currentDate.AddYears(interval),
            _ => currentDate.AddMonths(interval)
        };
    }
}
