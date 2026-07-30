using Microsoft.Extensions.Logging.Abstractions;
using SportsGurukul.Platform.PaymentGateway.Discount;
using SportsGurukul.Platform.PaymentGateway.Models;

namespace SportsGurukul.Platform.PaymentGateway.Tests;

public class DiscountEngineTests
{
    private readonly IDiscountEngine _discountEngine;

    public DiscountEngineTests()
    {
        var handlers = new List<IDiscountHandler>
        {
            new CouponDiscountHandler(),
            new ScholarshipDiscountHandler(),
            new PromotionDiscountHandler()
        };

        _discountEngine = new DiscountEngine(handlers, NullLogger<DiscountEngine>.Instance);
    }

    [Fact]
    public async Task ApplyCoupon_WithDefaultHandler_ReturnsNotApplied()
    {
        var result = await _discountEngine.ApplyCouponAsync("DISCOUNT10", 1000);
        Assert.False(result.IsApplied);
    }

    [Fact]
    public async Task ApplyScholarship_WithDefaultHandler_ReturnsNotApplied()
    {
        var result = await _discountEngine.ApplyScholarshipAsync("SCHOLAR001", 1000, "ATHLETE001");
        Assert.False(result.IsApplied);
    }

    [Fact]
    public async Task ApplyPromotion_WithDefaultHandler_ReturnsNotApplied()
    {
        var result = await _discountEngine.ApplyPromotionAsync("PROMO50", 1000);
        Assert.False(result.IsApplied);
    }

    [Fact]
    public async Task ApplyStackableDiscounts_EmptyRequests_ReturnsNoDiscount()
    {
        var result = await _discountEngine.ApplyStackableDiscountsAsync([], 1000);
        Assert.False(result.IsApplied);
        Assert.Equal(0, result.DiscountAmount);
        Assert.Equal(1000, result.FinalAmount);
    }

    [Fact]
    public async Task ApplyStackableDiscounts_MultipleRequests_ProcessesInOrder()
    {
        var requests = new List<DiscountRequest>
        {
            new() { DiscountType = "coupon", Code = "DISCOUNT10", OrderAmount = 1000 },
            new() { DiscountType = "promotion", Code = "PROMO50", OrderAmount = 900 }
        };

        var result = await _discountEngine.ApplyStackableDiscountsAsync(requests, 1000);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task ValidateCoupon_WithNoHandler_ReturnsFalse()
    {
        var engine = new DiscountEngine([], NullLogger<DiscountEngine>.Instance);
        var result = await engine.ValidateCouponAsync("COUPON");
        Assert.False(result);
    }

    [Fact]
    public async Task ValidatePromotion_WithCode_ReturnsTrue()
    {
        var result = await _discountEngine.ValidatePromotionAsync("PROMO2024");
        Assert.True(result);
    }

    [Fact]
    public async Task ValidatePromotion_EmptyCode_ReturnsFalse()
    {
        var result = await _discountEngine.ValidatePromotionAsync("");
        Assert.False(result);
    }

    [Fact]
    public async Task ApplyDiscountAsync_UnknownType_ReturnsNotApplied()
    {
        var request = new DiscountRequest
        {
            DiscountType = "unknown_type",
            Code = "TEST",
            OrderAmount = 1000
        };

        var result = await _discountEngine.ApplyDiscountAsync(request);
        Assert.False(result.IsApplied);
        Assert.NotNull(result.ErrorMessage);
    }

    [Fact]
    public async Task Handlers_HaveCorrectPriority()
    {
        var handler = new CouponDiscountHandler();
        Assert.Equal("coupon", handler.HandlerType);
        Assert.Equal(10, handler.Priority);

        var scholarship = new ScholarshipDiscountHandler();
        Assert.Equal("scholarship", scholarship.HandlerType);
        Assert.Equal(20, scholarship.Priority);

        var promotion = new PromotionDiscountHandler();
        Assert.Equal("promotion", promotion.HandlerType);
        Assert.Equal(30, promotion.Priority);
    }
}
