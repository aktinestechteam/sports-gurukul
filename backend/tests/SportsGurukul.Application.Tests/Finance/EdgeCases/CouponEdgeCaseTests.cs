using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces.Finance.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.Commands.Coupon;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;
using SportsGurukul.Domain.Enums.Finance;

namespace SportsGurukul.Application.Tests.Finance.EdgeCases;

public class ApplyCouponEdgeCaseTests
{
    private readonly Mock<ICouponService> _couponServiceMock;
    private readonly ApplyCouponCommandHandler _handler;

    public ApplyCouponEdgeCaseTests()
    {
        _couponServiceMock = new Mock<ICouponService>();
        _handler = new ApplyCouponCommandHandler(_couponServiceMock.Object);
    }

    [Fact]
    public async Task ApplyCoupon_ExpiredCoupon_ShouldFail()
    {
        var command = new ApplyCouponCommand("EXPIRED10", "user-001", 1000m);

        _couponServiceMock.Setup(s => s.ApplyCouponAsync("EXPIRED10", "user-001", 1000m, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<CouponDto>.Failure("Coupon has expired"));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Coupon has expired");
        _couponServiceMock.Verify(s => s.ApplyCouponAsync("EXPIRED10", "user-001", 1000m, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ApplyCoupon_UsageLimitReached_ShouldFail()
    {
        var command = new ApplyCouponCommand("MAXED20", "user-002", 500m);

        _couponServiceMock.Setup(s => s.ApplyCouponAsync("MAXED20", "user-002", 500m, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<CouponDto>.Failure("Coupon usage limit has been reached"));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Coupon usage limit has been reached");
        _couponServiceMock.Verify(s => s.ApplyCouponAsync("MAXED20", "user-002", 500m, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ApplyCoupon_MinimumOrderAmountNotMet_ShouldFail()
    {
        var command = new ApplyCouponCommand("MIN500", null, 200m);

        _couponServiceMock.Setup(s => s.ApplyCouponAsync("MIN500", null, 200m, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<CouponDto>.Failure("Minimum order amount of 500 is not met"));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Minimum order amount of 500 is not met");
        _couponServiceMock.Verify(s => s.ApplyCouponAsync("MIN500", null, 200m, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ApplyCoupon_NotYetValid_ShouldFail()
    {
        var command = new ApplyCouponCommand("FUTURE30", "user-003", 1000m);

        _couponServiceMock.Setup(s => s.ApplyCouponAsync("FUTURE30", "user-003", 1000m, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<CouponDto>.Failure("Coupon is not yet valid"));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Coupon is not yet valid");
        _couponServiceMock.Verify(s => s.ApplyCouponAsync("FUTURE30", "user-003", 1000m, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ApplyCoupon_InactiveCoupon_ShouldFail()
    {
        var command = new ApplyCouponCommand("INACTIVE", "user-004", 500m);

        _couponServiceMock.Setup(s => s.ApplyCouponAsync("INACTIVE", "user-004", 500m, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<CouponDto>.Failure("Coupon is not active"));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Coupon is not active");
        _couponServiceMock.Verify(s => s.ApplyCouponAsync("INACTIVE", "user-004", 500m, It.IsAny<CancellationToken>()), Times.Once);
    }
}

public class CreateCouponEdgeCaseTests
{
    private readonly Mock<ICouponService> _couponServiceMock;
    private readonly CreateCouponCommandHandler _handler;

    public CreateCouponEdgeCaseTests()
    {
        _couponServiceMock = new Mock<ICouponService>();
        _handler = new CreateCouponCommandHandler(_couponServiceMock.Object);
    }

    [Fact]
    public async Task CreateCoupon_DuplicateCode_ShouldFail()
    {
        var command = new CreateCouponCommand("SAVE10", null, DiscountType.Percentage, 10m, null, null, null, null, null);

        _couponServiceMock.Setup(s => s.CreateCouponAsync(It.IsAny<CreateCouponRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<CouponDto>.Failure("Coupon code already exists"));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Coupon code already exists");
        _couponServiceMock.Verify(s => s.CreateCouponAsync(It.Is<CreateCouponRequest>(r => r.Code == "SAVE10"), It.IsAny<CancellationToken>()), Times.Once);
    }
}
