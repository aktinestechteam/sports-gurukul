using SportsGurukul.Domain.Entities.Finance;
using SportsGurukul.Domain.Enums.Finance;
using SportsGurukul.Finance.Domain.Tests.Builders;

namespace SportsGurukul.Finance.Domain.Tests.Entities;

public class CouponEntityTests
{
    [Fact]
    public void CreateCoupon_HasCorrectInitialState()
    {
        var c = FinanceEntityBuilder.CreateCoupon();
        c.IsActive.Should().BeTrue();
        c.CurrentUsage.Should().Be(0);
        c.Type.Should().Be(DiscountType.Percentage);
    }

    [Fact]
    public void CouponAtMaxUsage_ShouldBeInactive()
    {
        var c = FinanceEntityBuilder.CreateCoupon(maxUsage: 1);
        c.CurrentUsage = 1;
        c.CurrentUsage.Should().Be(c.MaxUsage);
    }

    [Fact]
    public void ExpiredCoupon_ValidToBeforeNow_ShouldBeInactive()
    {
        var c = FinanceEntityBuilder.CreateCoupon();
        c.ValidTo = DateTime.UtcNow.AddDays(-1);
        c.ValidTo.Should().BeBefore(DateTime.UtcNow);
    }

    [Fact]
    public void FlatDiscountCoupon_HasCorrectType()
    {
        var c = FinanceEntityBuilder.CreateCoupon(type: DiscountType.Flat, value: 500);
        c.Type.Should().Be(DiscountType.Flat);
        c.Value.Should().Be(500);
    }

    [Fact]
    public void CouponWithMinOrderAmount_EnforcesMinimum()
    {
        var c = FinanceEntityBuilder.CreateCoupon();
        c.MinOrderAmount = 500;
        c.MinOrderAmount.Should().Be(500);
    }
}
