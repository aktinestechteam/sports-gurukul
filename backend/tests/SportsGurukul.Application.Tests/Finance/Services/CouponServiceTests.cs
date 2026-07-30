using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces.Finance;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;
using SportsGurukul.Application.Features.FinanceManagement.Services;
using SportsGurukul.Domain.Entities.Finance;
using SportsGurukul.Domain.Enums.Finance;

namespace SportsGurukul.Application.Tests.Finance.Services;

public class CouponServiceTests
{
    private readonly Mock<ICouponRepository> _couponRepoMock;
    private readonly CouponService _service;

    public CouponServiceTests()
    {
        _couponRepoMock = new Mock<ICouponRepository>();
        _service = new CouponService(_couponRepoMock.Object);
    }

    #region CreateCouponAsync

    [Fact]
    public async Task CreateCouponAsync_NewCode_ReturnsCoupon()
    {
        var request = new CreateCouponRequest("SAVE10", "Save 10%", DiscountType.Percentage, 10m, 500m, 100m, 100, DateTime.UtcNow, DateTime.UtcNow.AddDays(30));

        _couponRepoMock.Setup(r => r.GetByCodeAsync("SAVE10", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Coupon?)null);
        _couponRepoMock.Setup(r => r.AddAsync(It.IsAny<Coupon>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Coupon c, CancellationToken _) => c);

        var result = await _service.CreateCouponAsync(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Code.Should().Be("SAVE10");
        _couponRepoMock.Verify(r => r.AddAsync(It.IsAny<Coupon>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateCouponAsync_CodeUppercased_ReturnsUppercased()
    {
        var request = new CreateCouponRequest("save20", null, DiscountType.Flat, 20m, null, null, null, null, null);

        _couponRepoMock.Setup(r => r.GetByCodeAsync("SAVE20", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Coupon?)null);
        Coupon? captured = null;
        _couponRepoMock.Setup(r => r.AddAsync(It.IsAny<Coupon>(), It.IsAny<CancellationToken>()))
            .Callback((Coupon c, CancellationToken _) => captured = c)
            .ReturnsAsync((Coupon c, CancellationToken _) => c);

        await _service.CreateCouponAsync(request, CancellationToken.None);

        captured.Should().NotBeNull();
        captured!.Code.Should().Be("SAVE20");
    }

    [Fact]
    public async Task CreateCouponAsync_DuplicateCode_ReturnsFailure()
    {
        var request = new CreateCouponRequest("SAVE10", null, DiscountType.Percentage, 10m, null, null, null, null, null);

        _couponRepoMock.Setup(r => r.GetByCodeAsync("SAVE10", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Coupon { Code = "SAVE10" });

        var result = await _service.CreateCouponAsync(request, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Coupon code already exists");
    }

    [Fact]
    public async Task CreateCouponAsync_DefaultDates_WhenNotProvided()
    {
        var request = new CreateCouponRequest("TEST", null, DiscountType.Percentage, 10m, null, null, null, null, null);

        _couponRepoMock.Setup(r => r.GetByCodeAsync("TEST", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Coupon?)null);
        Coupon? captured = null;
        _couponRepoMock.Setup(r => r.AddAsync(It.IsAny<Coupon>(), It.IsAny<CancellationToken>()))
            .Callback((Coupon c, CancellationToken _) => captured = c)
            .ReturnsAsync((Coupon c, CancellationToken _) => c);

        await _service.CreateCouponAsync(request, CancellationToken.None);

        captured.Should().NotBeNull();
        captured!.ValidFrom.Date.Should().Be(DateTime.UtcNow.Date);
        captured.ValidTo.Date.Should().Be(DateTime.UtcNow.AddDays(30).Date);
    }

    #endregion

    #region UpdateCouponAsync

    [Fact]
    public async Task UpdateCouponAsync_ValidCoupon_ReturnsUpdated()
    {
        var couponId = Guid.NewGuid();
        var coupon = new Coupon
        {
            Id = couponId,
            Value = 10m,
            MinOrderAmount = 100m,
            MaxDiscountAmount = 500m,
            MaxUsage = 100,
            ValidFrom = DateTime.UtcNow.AddDays(-30),
            ValidTo = DateTime.UtcNow.AddDays(30)
        };

        _couponRepoMock.Setup(r => r.GetByIdAsync(couponId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(coupon);

        var request = new UpdateCouponRequest("Updated", 20m, 200m, 1000m, 50, DateTime.UtcNow, DateTime.UtcNow.AddMonths(1));
        var result = await _service.UpdateCouponAsync(couponId, request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _couponRepoMock.Verify(r => r.Update(coupon), Times.Once);
    }

    [Fact]
    public async Task UpdateCouponAsync_PartialUpdate_OnlyUpdatesProvidedFields()
    {
        var couponId = Guid.NewGuid();
        var coupon = new Coupon
        {
            Id = couponId,
            Value = 10m,
            MinOrderAmount = 100m,
            MaxDiscountAmount = 500m,
            MaxUsage = 100,
            ValidFrom = DateTime.UtcNow.AddDays(-30),
            ValidTo = DateTime.UtcNow.AddDays(30)
        };

        _couponRepoMock.Setup(r => r.GetByIdAsync(couponId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(coupon);

        var request = new UpdateCouponRequest(null, 25m, null, null, null, null, null);
        var result = await _service.UpdateCouponAsync(couponId, request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        coupon.Value.Should().Be(25m);
        coupon.MinOrderAmount.Should().Be(100m);
        coupon.MaxDiscountAmount.Should().Be(500m);
    }

    [Fact]
    public async Task UpdateCouponAsync_NotFound_ReturnsFailure()
    {
        _couponRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Coupon?)null);

        var request = new UpdateCouponRequest(null, null, null, null, null, null, null);
        var result = await _service.UpdateCouponAsync(Guid.NewGuid(), request, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Coupon not found");
    }

    #endregion

    #region ApplyCouponAsync

    [Fact]
    public async Task ApplyCouponAsync_ValidCoupon_IncrementsUsage()
    {
        var coupon = new Coupon
        {
            Id = Guid.NewGuid(),
            Code = "SAVE10",
            Type = DiscountType.Percentage,
            Value = 10m,
            IsActive = true,
            ValidFrom = DateTime.UtcNow.AddDays(-30),
            ValidTo = DateTime.UtcNow.AddDays(30),
            CurrentUsage = 5,
            MaxUsage = 100,
            MinOrderAmount = 100
        };

        _couponRepoMock.Setup(r => r.GetByCodeAsync("SAVE10", It.IsAny<CancellationToken>()))
            .ReturnsAsync(coupon);
        _couponRepoMock.Setup(r => r.GetByCodeWithUsagesAsync("SAVE10", It.IsAny<CancellationToken>()))
            .ReturnsAsync(coupon);

        var result = await _service.ApplyCouponAsync("SAVE10", "user-1", 1000m, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        coupon.CurrentUsage.Should().Be(6);
        _couponRepoMock.Verify(r => r.Update(coupon), Times.Once);
    }

    [Fact]
    public async Task ApplyCouponAsync_InvalidCoupon_ReturnsFailure()
    {
        _couponRepoMock.Setup(r => r.GetByCodeAsync("INVALID", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Coupon?)null);

        var result = await _service.ApplyCouponAsync("INVALID", null, 1000m, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }

    #endregion

    #region ExpireCouponAsync

    [Fact]
    public async Task ExpireCouponAsync_ValidCoupon_Deactivates()
    {
        var couponId = Guid.NewGuid();
        var coupon = new Coupon { Id = couponId, IsActive = true };

        _couponRepoMock.Setup(r => r.GetByIdAsync(couponId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(coupon);

        var result = await _service.ExpireCouponAsync(couponId, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        coupon.IsActive.Should().BeFalse();
        _couponRepoMock.Verify(r => r.Update(coupon), Times.Once);
    }

    [Fact]
    public async Task ExpireCouponAsync_NotFound_ReturnsFailure()
    {
        _couponRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Coupon?)null);

        var result = await _service.ExpireCouponAsync(Guid.NewGuid(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Coupon not found");
    }

    #endregion

    #region ValidateCouponAsync

    [Fact]
    public async Task ValidateCouponAsync_ValidCoupon_ReturnsTrue()
    {
        var coupon = new Coupon
        {
            Code = "SAVE10",
            IsActive = true,
            ValidFrom = DateTime.UtcNow.AddDays(-30),
            ValidTo = DateTime.UtcNow.AddDays(30),
            CurrentUsage = 5,
            MaxUsage = 100,
            MinOrderAmount = 100
        };

        _couponRepoMock.Setup(r => r.GetByCodeAsync("SAVE10", It.IsAny<CancellationToken>()))
            .ReturnsAsync(coupon);

        var result = await _service.ValidateCouponAsync("SAVE10", null, 500m, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateCouponAsync_NotFound_ReturnsFailure()
    {
        _couponRepoMock.Setup(r => r.GetByCodeAsync("NONE", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Coupon?)null);

        var result = await _service.ValidateCouponAsync("NONE", null, 100m, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Coupon not found");
    }

    [Fact]
    public async Task ValidateCouponAsync_NotActive_ReturnsFailure()
    {
        var coupon = new Coupon { Code = "INACTIVE", IsActive = false };
        _couponRepoMock.Setup(r => r.GetByCodeAsync("INACTIVE", It.IsAny<CancellationToken>()))
            .ReturnsAsync(coupon);

        var result = await _service.ValidateCouponAsync("INACTIVE", null, 100m, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Coupon is not active");
    }

    [Fact]
    public async Task ValidateCouponAsync_NotYetValid_ReturnsFailure()
    {
        var coupon = new Coupon
        {
            Code = "FUTURE",
            IsActive = true,
            ValidFrom = DateTime.UtcNow.AddDays(10),
            ValidTo = DateTime.UtcNow.AddDays(30)
        };
        _couponRepoMock.Setup(r => r.GetByCodeAsync("FUTURE", It.IsAny<CancellationToken>()))
            .ReturnsAsync(coupon);

        var result = await _service.ValidateCouponAsync("FUTURE", null, 100m, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Coupon is not yet valid");
    }

    [Fact]
    public async Task ValidateCouponAsync_Expired_ReturnsFailure()
    {
        var coupon = new Coupon
        {
            Code = "EXPIRED",
            IsActive = true,
            ValidFrom = DateTime.UtcNow.AddDays(-60),
            ValidTo = DateTime.UtcNow.AddDays(-1)
        };
        _couponRepoMock.Setup(r => r.GetByCodeAsync("EXPIRED", It.IsAny<CancellationToken>()))
            .ReturnsAsync(coupon);

        var result = await _service.ValidateCouponAsync("EXPIRED", null, 100m, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Coupon has expired");
    }

    [Fact]
    public async Task ValidateCouponAsync_UsageExceeded_ReturnsFailure()
    {
        var coupon = new Coupon
        {
            Code = "MAXED",
            IsActive = true,
            ValidFrom = DateTime.UtcNow.AddDays(-30),
            ValidTo = DateTime.UtcNow.AddDays(30),
            CurrentUsage = 100,
            MaxUsage = 100
        };
        _couponRepoMock.Setup(r => r.GetByCodeAsync("MAXED", It.IsAny<CancellationToken>()))
            .ReturnsAsync(coupon);

        var result = await _service.ValidateCouponAsync("MAXED", null, 500m, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Coupon usage limit exceeded");
    }

    [Fact]
    public async Task ValidateCouponAsync_MinOrderNotMet_ReturnsFailure()
    {
        var coupon = new Coupon
        {
            Code = "MINFAIL",
            IsActive = true,
            ValidFrom = DateTime.UtcNow.AddDays(-30),
            ValidTo = DateTime.UtcNow.AddDays(30),
            MinOrderAmount = 500,
            CurrentUsage = 0,
            MaxUsage = 100
        };
        _couponRepoMock.Setup(r => r.GetByCodeAsync("MINFAIL", It.IsAny<CancellationToken>()))
            .ReturnsAsync(coupon);

        var result = await _service.ValidateCouponAsync("MINFAIL", null, 100m, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Minimum order amount of 500 not met");
    }

    [Fact]
    public async Task ValidateCouponAsync_NoMaxUsage_DoesNotCheckUsage()
    {
        var coupon = new Coupon
        {
            Code = "UNLIMITED",
            IsActive = true,
            ValidFrom = DateTime.UtcNow.AddDays(-30),
            ValidTo = DateTime.UtcNow.AddDays(30),
            CurrentUsage = 999,
            MaxUsage = null,
            MinOrderAmount = null
        };
        _couponRepoMock.Setup(r => r.GetByCodeAsync("UNLIMITED", It.IsAny<CancellationToken>()))
            .ReturnsAsync(coupon);

        var result = await _service.ValidateCouponAsync("UNLIMITED", null, 100m, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateCouponAsync_NoMinOrder_SkipsMinCheck()
    {
        var coupon = new Coupon
        {
            Code = "NOMIN",
            IsActive = true,
            ValidFrom = DateTime.UtcNow.AddDays(-30),
            ValidTo = DateTime.UtcNow.AddDays(30),
            MinOrderAmount = null,
            CurrentUsage = 0
        };
        _couponRepoMock.Setup(r => r.GetByCodeAsync("NOMIN", It.IsAny<CancellationToken>()))
            .ReturnsAsync(coupon);

        var result = await _service.ValidateCouponAsync("NOMIN", null, 1m, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    #endregion

    #region GetByCodeAsync

    [Fact]
    public async Task GetByCodeAsync_Exists_ReturnsCoupon()
    {
        _couponRepoMock.Setup(r => r.GetByCodeAsync("SAVE10", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Coupon { Code = "SAVE10", IsActive = true });

        var result = await _service.GetByCodeAsync("SAVE10", CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Code.Should().Be("SAVE10");
    }

    [Fact]
    public async Task GetByCodeAsync_NotFound_ReturnsFailure()
    {
        _couponRepoMock.Setup(r => r.GetByCodeAsync("NONE", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Coupon?)null);

        var result = await _service.GetByCodeAsync("NONE", CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Coupon not found");
    }

    #endregion
}
