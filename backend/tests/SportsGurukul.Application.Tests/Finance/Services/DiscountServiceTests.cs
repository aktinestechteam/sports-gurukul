using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces.Finance;
using SportsGurukul.Application.Common.Interfaces.Finance.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.Services;
using SportsGurukul.Domain.Entities.Finance;
using SportsGurukul.Domain.Enums.Finance;

namespace SportsGurukul.Application.Tests.Finance.Services;

public class DiscountServiceTests
{
    private readonly Mock<ICouponRepository> _couponRepoMock;
    private readonly DiscountService _service;

    public DiscountServiceTests()
    {
        _couponRepoMock = new Mock<ICouponRepository>();
        _service = new DiscountService(_couponRepoMock.Object);
    }

    #region ApplyDiscountAsync

    [Fact]
    public async Task ApplyDiscountAsync_PercentageCoupon_ReturnsCalculatedDiscount()
    {
        var coupon = new Coupon
        {
            Code = "SAVE10",
            Type = DiscountType.Percentage,
            Value = 10m,
            IsActive = true,
            MinOrderAmount = null,
            MaxDiscountAmount = null
        };

        _couponRepoMock.Setup(r => r.GetByCodeAsync("SAVE10", It.IsAny<CancellationToken>()))
            .ReturnsAsync(coupon);

        var result = await _service.ApplyDiscountAsync(1000m, "SAVE10", null, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.DiscountName.Should().Be("SAVE10");
        result.Value.DiscountAmount.Should().Be(100m);
        result.Value.DiscountedTotal.Should().Be(900m);
    }

    [Fact]
    public async Task ApplyDiscountAsync_FlatCoupon_ReturnsFlatDiscount()
    {
        var coupon = new Coupon
        {
            Code = "FLAT50",
            Type = DiscountType.Flat,
            Value = 50m,
            IsActive = true
        };

        _couponRepoMock.Setup(r => r.GetByCodeAsync("FLAT50", It.IsAny<CancellationToken>()))
            .ReturnsAsync(coupon);

        var result = await _service.ApplyDiscountAsync(1000m, "FLAT50", null, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.DiscountAmount.Should().Be(50m);
        result.Value.DiscountedTotal.Should().Be(950m);
    }

    [Fact]
    public async Task ApplyDiscountAsync_CappedByMaxDiscountAmount()
    {
        var coupon = new Coupon
        {
            Code = "BIG50",
            Type = DiscountType.Percentage,
            Value = 50m,
            IsActive = true,
            MaxDiscountAmount = 200m
        };

        _couponRepoMock.Setup(r => r.GetByCodeAsync("BIG50", It.IsAny<CancellationToken>()))
            .ReturnsAsync(coupon);

        var result = await _service.ApplyDiscountAsync(1000m, "BIG50", null, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.DiscountAmount.Should().Be(200m);
        result.Value.DiscountedTotal.Should().Be(800m);
    }

    [Fact]
    public async Task ApplyDiscountAsync_CappedBySubTotal()
    {
        var coupon = new Coupon
        {
            Code = "OVER100",
            Type = DiscountType.Flat,
            Value = 200m,
            IsActive = true,
            MaxDiscountAmount = null
        };

        _couponRepoMock.Setup(r => r.GetByCodeAsync("OVER100", It.IsAny<CancellationToken>()))
            .ReturnsAsync(coupon);

        var result = await _service.ApplyDiscountAsync(150m, "OVER100", null, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.DiscountAmount.Should().Be(150m);
        result.Value.DiscountedTotal.Should().Be(0m);
    }

    [Fact]
    public async Task ApplyDiscountAsync_CouponNotFound_ReturnsFailure()
    {
        _couponRepoMock.Setup(r => r.GetByCodeAsync("INVALID", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Coupon?)null);

        var result = await _service.ApplyDiscountAsync(1000m, "INVALID", null, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Coupon not found");
    }

    [Fact]
    public async Task ApplyDiscountAsync_InactiveCoupon_ReturnsFailure()
    {
        var coupon = new Coupon { Code = "INACTIVE", IsActive = false };
        _couponRepoMock.Setup(r => r.GetByCodeAsync("INACTIVE", It.IsAny<CancellationToken>()))
            .ReturnsAsync(coupon);

        var result = await _service.ApplyDiscountAsync(1000m, "INACTIVE", null, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Coupon is not active");
    }

    [Fact]
    public async Task ApplyDiscountAsync_ZeroSubTotal_ReturnsZeroDiscount()
    {
        var coupon = new Coupon
        {
            Code = "ZERO",
            Type = DiscountType.Percentage,
            Value = 10m,
            IsActive = true
        };

        _couponRepoMock.Setup(r => r.GetByCodeAsync("ZERO", It.IsAny<CancellationToken>()))
            .ReturnsAsync(coupon);

        var result = await _service.ApplyDiscountAsync(0m, "ZERO", null, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.DiscountAmount.Should().Be(0m);
    }

    #endregion

    #region ApplyScholarshipAsync

    [Fact]
    public async Task ApplyScholarshipAsync_Returns25PercentDiscount()
    {
        var scholarshipId = Guid.NewGuid();

        var result = await _service.ApplyScholarshipAsync(1000m, scholarshipId, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.DiscountName.Should().Be("Scholarship");
        result.Value.DiscountAmount.Should().Be(250m);
        result.Value.DiscountedTotal.Should().Be(750m);
    }

    [Fact]
    public async Task ApplyScholarshipAsync_ZeroSubTotal_ReturnsZero()
    {
        var result = await _service.ApplyScholarshipAsync(0m, Guid.NewGuid(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.DiscountAmount.Should().Be(0m);
    }

    #endregion

    #region ApplyDiscountPolicyAsync

    [Fact]
    public async Task ApplyDiscountPolicyAsync_Returns10PercentDiscount()
    {
        var policyId = Guid.NewGuid();

        var result = await _service.ApplyDiscountPolicyAsync(2000m, policyId, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.DiscountName.Should().Be("Discount Policy");
        result.Value.DiscountAmount.Should().Be(200m);
        result.Value.DiscountedTotal.Should().Be(1800m);
    }

    [Fact]
    public async Task ApplyDiscountPolicyAsync_ZeroSubTotal_ReturnsZero()
    {
        var result = await _service.ApplyDiscountPolicyAsync(0m, Guid.NewGuid(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.DiscountAmount.Should().Be(0m);
    }

    #endregion
}
