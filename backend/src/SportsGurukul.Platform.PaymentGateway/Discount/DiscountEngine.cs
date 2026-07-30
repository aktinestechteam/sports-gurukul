using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.PaymentGateway.Models;

namespace SportsGurukul.Platform.PaymentGateway.Discount;

public class DiscountEngine : IDiscountEngine
{
    private readonly ILogger<DiscountEngine> _logger;
    private readonly List<IDiscountHandler> _handlers;

    public DiscountEngine(
        IEnumerable<IDiscountHandler> handlers,
        ILogger<DiscountEngine> logger)
    {
        _handlers = handlers.OrderBy(h => h.Priority).ToList();
        _logger = logger;
    }

    public Task<DiscountResult> ApplyDiscountAsync(
        DiscountRequest request,
        CancellationToken cancellationToken = default)
    {
        var handler = _handlers.FirstOrDefault(h =>
            h.HandlerType.Equals(request.DiscountType, StringComparison.OrdinalIgnoreCase));

        if (handler is null)
        {
            _logger.LogWarning("No discount handler found for type {DiscountType}", request.DiscountType);
            return Task.FromResult(new DiscountResult
            {
                IsApplied = false,
                FinalAmount = request.OrderAmount,
                ErrorMessage = $"No handler for discount type '{request.DiscountType}'"
            });
        }

        return handler.ApplyAsync(request, cancellationToken);
    }

    public async Task<DiscountResult> ApplyCouponAsync(
        string couponCode,
        decimal orderAmount,
        string? customerId = null,
        CancellationToken cancellationToken = default)
    {
        var request = new DiscountRequest
        {
            DiscountType = "coupon",
            Code = couponCode,
            OrderAmount = orderAmount,
            CustomerId = customerId
        };

        return await ApplyDiscountAsync(request, cancellationToken);
    }

    public async Task<DiscountResult> ApplyScholarshipAsync(
        string scholarshipId,
        decimal orderAmount,
        string athleteId,
        CancellationToken cancellationToken = default)
    {
        var request = new DiscountRequest
        {
            DiscountType = "scholarship",
            Code = scholarshipId,
            OrderAmount = orderAmount,
            CustomerId = athleteId,
            Metadata = new Dictionary<string, string> { ["athlete_id"] = athleteId }
        };

        return await ApplyDiscountAsync(request, cancellationToken);
    }

    public async Task<DiscountResult> ApplyPromotionAsync(
        string promotionCode,
        decimal orderAmount,
        CancellationToken cancellationToken = default)
    {
        var request = new DiscountRequest
        {
            DiscountType = "promotion",
            Code = promotionCode,
            OrderAmount = orderAmount
        };

        return await ApplyDiscountAsync(request, cancellationToken);
    }

    public async Task<DiscountResult> ApplyStackableDiscountsAsync(
        List<DiscountRequest> requests,
        decimal orderAmount,
        CancellationToken cancellationToken = default)
    {
        var breakdown = new List<DiscountBreakdown>();
        var remainingAmount = orderAmount;
        var totalDiscount = 0m;
        var errors = new List<string>();

        var combinableRequests = FilterCombinableDiscounts(requests);

        foreach (var request in combinableRequests)
        {
            request.OrderAmount = remainingAmount;

            var result = await ApplyDiscountAsync(request, cancellationToken);
            if (result.IsApplied)
            {
                totalDiscount += result.DiscountAmount;
                remainingAmount = orderAmount - totalDiscount;
                breakdown.AddRange(result.Breakdown);
            }
            else if (result.ErrorMessage is not null)
            {
                errors.Add(result.ErrorMessage);
            }
        }

        return new DiscountResult
        {
            IsApplied = totalDiscount > 0,
            DiscountAmount = Math.Round(totalDiscount, 2),
            FinalAmount = Math.Round(orderAmount - totalDiscount, 2),
            Breakdown = breakdown,
            ErrorMessage = errors.Count > 0 ? string.Join("; ", errors) : null
        };
    }

    public Task<bool> ValidateCouponAsync(
        string couponCode,
        string? customerId = null,
        CancellationToken cancellationToken = default)
    {
        var handler = _handlers.FirstOrDefault(h =>
            h.HandlerType.Equals("coupon", StringComparison.OrdinalIgnoreCase));

        if (handler is null)
            return Task.FromResult(false);

        var request = new DiscountRequest
        {
            DiscountType = "coupon",
            Code = couponCode,
            CustomerId = customerId
        };

        return Task.FromResult(true);
    }

    public Task<bool> ValidatePromotionAsync(
        string promotionCode,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(!string.IsNullOrWhiteSpace(promotionCode));
    }

    private static List<DiscountRequest> FilterCombinableDiscounts(List<DiscountRequest> requests)
    {
        return requests
            .Where(r => !string.IsNullOrWhiteSpace(r.Code))
            .GroupBy(r => r.DiscountType)
            .Select(g => g.First())
            .ToList();
    }
}
