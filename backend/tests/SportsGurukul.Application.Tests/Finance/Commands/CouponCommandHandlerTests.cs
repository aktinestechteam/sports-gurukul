using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces.Finance.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.Commands.Coupon;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;
using SportsGurukul.Domain.Enums.Finance;

namespace SportsGurukul.Application.Tests.Finance.Commands;

public class CreateCouponCommandHandlerTests
{
    private readonly Mock<ICouponService> _serviceMock;
    private readonly CreateCouponCommandHandler _handler;

    public CreateCouponCommandHandlerTests()
    {
        _serviceMock = new Mock<ICouponService>();
        _handler = new CreateCouponCommandHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsSuccess()
    {
        var couponId = Guid.NewGuid();
        var command = new CreateCouponCommand("SAVE10", "10% off", DiscountType.Percentage, 10m, 100m, 500m, 100, DateTime.UtcNow, DateTime.UtcNow.AddMonths(1));
        var expected = Result<CouponDto>.Success(new CouponDto(couponId, "SAVE10", "10% off", DiscountType.Percentage, 10m, 100m, 500m, 100, 0, DateTime.UtcNow, DateTime.UtcNow.AddMonths(1), true, DateTime.UtcNow));
        _serviceMock.Setup(s => s.CreateCouponAsync(It.IsAny<CreateCouponRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _serviceMock.Verify(s => s.CreateCouponAsync(It.IsAny<CreateCouponRequest>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ServiceFailure_ReturnsFailure()
    {
        var command = new CreateCouponCommand("", null, DiscountType.Flat, 0m, null, null, null, null, null);
        _serviceMock.Setup(s => s.CreateCouponAsync(It.IsAny<CreateCouponRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<CouponDto>.Failure("Creation failed"));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Creation failed");
    }
}

public class ApplyCouponCommandHandlerTests
{
    private readonly Mock<ICouponService> _serviceMock;
    private readonly ApplyCouponCommandHandler _handler;

    public ApplyCouponCommandHandlerTests()
    {
        _serviceMock = new Mock<ICouponService>();
        _handler = new ApplyCouponCommandHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsSuccess()
    {
        var couponId = Guid.NewGuid();
        var command = new ApplyCouponCommand("SAVE10", "user-001", 500m);
        var expected = Result<CouponDto>.Success(new CouponDto(couponId, "SAVE10", null, DiscountType.Percentage, 10m, null, 500m, null, 1, null, null, true, DateTime.UtcNow));
        _serviceMock.Setup(s => s.ApplyCouponAsync(It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<decimal>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _serviceMock.Verify(s => s.ApplyCouponAsync(It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<decimal>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ServiceFailure_ReturnsFailure()
    {
        var command = new ApplyCouponCommand("INVALID", null, 100m);
        _serviceMock.Setup(s => s.ApplyCouponAsync(It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<decimal>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<CouponDto>.Failure("Invalid coupon"));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Invalid coupon");
    }
}
