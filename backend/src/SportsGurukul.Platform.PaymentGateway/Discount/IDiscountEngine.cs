using SportsGurukul.Platform.PaymentGateway.Models;

namespace SportsGurukul.Platform.PaymentGateway.Discount;

public interface IDiscountEngine
{
    Task<DiscountResult> ApplyDiscountAsync(
        DiscountRequest request,
        CancellationToken cancellationToken = default);

    Task<DiscountResult> ApplyCouponAsync(
        string couponCode,
        decimal orderAmount,
        string? customerId = null,
        CancellationToken cancellationToken = default);

    Task<DiscountResult> ApplyScholarshipAsync(
        string scholarshipId,
        decimal orderAmount,
        string athleteId,
        CancellationToken cancellationToken = default);

    Task<DiscountResult> ApplyPromotionAsync(
        string promotionCode,
        decimal orderAmount,
        CancellationToken cancellationToken = default);

    Task<DiscountResult> ApplyStackableDiscountsAsync(
        List<DiscountRequest> requests,
        decimal orderAmount,
        CancellationToken cancellationToken = default);

    Task<bool> ValidateCouponAsync(
        string couponCode,
        string? customerId = null,
        CancellationToken cancellationToken = default);

    Task<bool> ValidatePromotionAsync(
        string promotionCode,
        CancellationToken cancellationToken = default);
}

public class DiscountRequest
{
    public string DiscountType { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public decimal OrderAmount { get; set; }
    public string? CustomerId { get; set; }
    public Dictionary<string, string>? Metadata { get; set; }
}

public class DiscountResult
{
    public bool IsApplied { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal FinalAmount { get; set; }
    public List<DiscountBreakdown> Breakdown { get; set; } = [];
    public string? ErrorMessage { get; set; }
}

public class CouponDiscountHandler : IDiscountHandler
{
    public string HandlerType => "coupon";
    public int Priority => 10;

    public async Task<DiscountResult> ApplyAsync(
        DiscountRequest request,
        CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;
        return new DiscountResult { IsApplied = false };
    }
}

public class ScholarshipDiscountHandler : IDiscountHandler
{
    public string HandlerType => "scholarship";
    public int Priority => 20;

    public async Task<DiscountResult> ApplyAsync(
        DiscountRequest request,
        CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;
        return new DiscountResult { IsApplied = false };
    }
}

public class PromotionDiscountHandler : IDiscountHandler
{
    public string HandlerType => "promotion";
    public int Priority => 30;

    public async Task<DiscountResult> ApplyAsync(
        DiscountRequest request,
        CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;
        return new DiscountResult { IsApplied = false };
    }
}

public interface IDiscountHandler
{
    string HandlerType { get; }
    int Priority { get; }
    Task<DiscountResult> ApplyAsync(DiscountRequest request, CancellationToken cancellationToken = default);
}

public class StackableDiscountRule
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsCombinable { get; set; } = true;
    public int ApplyOrder { get; set; }
    public decimal? MaxDiscountPercentage { get; set; }
    public decimal? MaxDiscountAmount { get; set; }
    public Func<List<DiscountResult>, DiscountResult, bool>? ConflictResolver { get; set; }
}
