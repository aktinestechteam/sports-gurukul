using SportsGurukul.Domain.Enums.Finance;

namespace SportsGurukul.Application.Features.FinanceManagement.DTOs;

public record CouponDto(
    Guid Id,
    string Code,
    string? Description,
    DiscountType Type,
    decimal Value,
    decimal? MinimumOrderAmount,
    decimal? MaximumDiscountAmount,
    int? MaxUsages,
    int CurrentUsages,
    DateTime? ValidFrom,
    DateTime? ValidTo,
    bool IsActive,
    DateTime CreatedAt
);

public record CreateCouponRequest(
    string Code,
    string? Description,
    DiscountType DiscountType,
    decimal Value,
    decimal? MinimumOrderAmount,
    decimal? MaximumDiscountAmount,
    int? MaxUsages,
    DateTime? ValidFrom,
    DateTime? ValidTo
);

public record UpdateCouponRequest(
    string? Description,
    decimal? Value,
    decimal? MinimumOrderAmount,
    decimal? MaximumDiscountAmount,
    int? MaxUsages,
    DateTime? ValidFrom,
    DateTime? ValidTo
);
