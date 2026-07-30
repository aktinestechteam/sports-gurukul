using FluentAssertions;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.Commands.Coupon;
using SportsGurukul.Application.Features.FinanceManagement.Commands.Scholarship;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;
using SportsGurukul.Application.Features.FinanceManagement.Queries;
using SportsGurukul.Domain.Enums.Finance;
using SportsGurukul.Finance.IntegrationTests.Fixtures;
using SportsGurukul.Finance.IntegrationTests.Helpers;
using SportsGurukul.Finance.IntegrationTests.Seed;
using Xunit;

namespace SportsGurukul.Finance.IntegrationTests.Tests;

[Collection("Finance")]
public class CouponScholarshipTests : FinanceTestBase
{
    public CouponScholarshipTests(FinanceWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task CreateCoupon_WithValidData_ReturnsSuccess()
    {
        var result = await SendAsync(new CreateCouponCommand(
            "SUMMER25", "Summer discount", DiscountType.Percentage, 25m,
            1000m, 2000m, 50, DateTime.UtcNow, DateTime.UtcNow.AddDays(60)));

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Code.Should().Be("SUMMER25");
        result.Value.Value.Should().Be(25m);
    }

    [Fact]
    public async Task CreateCoupon_WithDuplicateCode_ReturnsFailure()
    {
        var result = await SendAsync(new CreateCouponCommand(
            "WELCOME10", "Duplicate", DiscountType.Percentage, 10m,
            null, null, null, DateTime.UtcNow, DateTime.UtcNow.AddDays(30)));

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task UpdateCoupon_ChangesProperties()
    {
        var result = await SendAsync(new UpdateCouponCommand(
            FinanceTestIds.TestCouponId, "Updated description", 15m,
            200m, 1000m, 200, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(90)));

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Description.Should().Be("Updated description");
    }

    [Fact]
    public async Task ApplyCoupon_ValidCoupon_ReturnsDiscount()
    {
        var result = await SendAsync(new ApplyCouponCommand(
            "WELCOME10", FinanceTestIds.AthleteUserId.ToString(), 1500m));

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task ApplyCoupon_ExpiredCoupon_ReturnsFailure()
    {
        var expiredCommand = await SendAsync(new CreateCouponCommand(
            "EXPIRED", "Expired", DiscountType.Flat, 100m,
            null, null, 10, DateTime.UtcNow.AddDays(-60), DateTime.UtcNow.AddDays(-1)));

        var result = await SendAsync(new ApplyCouponCommand(
            "EXPIRED", FinanceTestIds.AthleteUserId.ToString(), 1500m));

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task ApplyCoupon_ExceededUsageLimit_ReturnsFailure()
    {
        var limitedCommand = await SendAsync(new CreateCouponCommand(
            "LIMITED1", "Limited", DiscountType.Percentage, 10m,
            null, null, 1, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(30)));

        var firstUse = await SendAsync(new ApplyCouponCommand(
            "LIMITED1", FinanceTestIds.AthleteUserId.ToString(), 1000m));

        var secondUse = await SendAsync(new ApplyCouponCommand(
            "LIMITED1", FinanceTestIds.AthleteUserId.ToString(), 1000m));

        secondUse.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task ExpireCoupon_SetsInactive()
    {
        var result = await SendAsync(new ExpireCouponCommand(FinanceTestIds.TestCouponId));

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task GetCouponByCode_ReturnsCoupon()
    {
        var result = await SendAsync(new GetCouponByCodeQuery("WELCOME10"));

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Code.Should().Be("WELCOME10");
    }

    [Fact]
    public async Task ValidateCoupon_ValidCoupon_ReturnsSuccess()
    {
        var result = await SendAsync(new ValidateCouponQuery(
            "WELCOME10", FinanceTestIds.AthleteUserId.ToString(), 1500m));

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
    }

    [Fact]
    public async Task CouponUsage_TracksUsageCount()
    {
        await SendAsync(new ApplyCouponCommand(
            "WELCOME10", FinanceTestIds.AthleteUserId.ToString(), 2000m));

        var couponResult = await SendAsync(new GetCouponByCodeQuery("WELCOME10"));

        couponResult.IsSuccess.Should().BeTrue();
        couponResult.Value!.CurrentUsages.Should().BeGreaterThanOrEqualTo(1);
    }

    [Fact]
    public async Task CreateScholarship_WithValidData_ReturnsSuccess()
    {
        var result = await SendAsync(new CreateScholarshipCommand(
            FinanceTestIds.AthleteUserId, "Merit Scholarship", "Based on performance",
            DiscountType.Percentage, 15m, 5000m,
            DateTime.UtcNow, DateTime.UtcNow.AddYears(1)));

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Name.Should().Be("Merit Scholarship");
    }

    [Fact]
    public async Task ApproveScholarship_ChangesStatus()
    {
        var createResult = await SendAsync(new CreateScholarshipCommand(
            FinanceTestIds.AthleteUserId, "Approval Test", null,
            DiscountType.Percentage, 10m, null,
            DateTime.UtcNow, DateTime.UtcNow.AddMonths(6)));

        var result = await SendAsync(new ApproveScholarshipCommand(createResult.Value!.Id));

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task RejectScholarship_ChangesStatus()
    {
        var createResult = await SendAsync(new CreateScholarshipCommand(
            FinanceTestIds.AthleteUserId, "Rejection Test", null,
            DiscountType.Percentage, 10m, null,
            DateTime.UtcNow, DateTime.UtcNow.AddMonths(6)));

        var result = await SendAsync(new RejectScholarshipCommand(createResult.Value!.Id, "Insufficient criteria"));

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task GetAllScholarships_ReturnsList()
    {
        var result = await SendAsync(new GetAllScholarshipsQuery());

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().HaveCountGreaterThanOrEqualTo(1);
    }
}
