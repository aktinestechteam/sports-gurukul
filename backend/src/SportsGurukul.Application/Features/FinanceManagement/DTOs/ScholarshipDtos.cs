using SportsGurukul.Domain.Enums.Finance;

namespace SportsGurukul.Application.Features.FinanceManagement.DTOs;

public record ScholarshipDto(
    Guid Id,
    Guid AthleteId,
    string? AthleteName,
    string? Name,
    string? Description,
    DiscountType Type,
    decimal Value,
    decimal? MaxAmount,
    DateTime? ValidFrom,
    DateTime? ValidTo,
    bool IsActive,
    DateTime CreatedAt
);

public record CreateScholarshipRequest(
    Guid AthleteId,
    string? Name,
    string? Description,
    DiscountType DiscountType,
    decimal Value,
    decimal? MaxAmount,
    DateTime? ValidFrom,
    DateTime? ValidTo
);

public record UpdateScholarshipRequest(
    string? Name,
    string? Description,
    decimal? Value,
    decimal? MaxAmount,
    DateTime? ValidFrom,
    DateTime? ValidTo
);
