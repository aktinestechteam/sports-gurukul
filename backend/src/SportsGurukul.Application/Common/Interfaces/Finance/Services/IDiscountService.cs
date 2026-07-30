using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;

namespace SportsGurukul.Application.Common.Interfaces.Finance.Services;

public interface IDiscountService
{
    Task<Result<DiscountResult>> ApplyDiscountAsync(decimal subTotal, string couponCode, string? userId = null, CancellationToken cancellationToken = default);
    Task<Result<DiscountResult>> ApplyScholarshipAsync(decimal subTotal, Guid scholarshipId, CancellationToken cancellationToken = default);
    Task<Result<DiscountResult>> ApplyDiscountPolicyAsync(decimal subTotal, Guid policyId, CancellationToken cancellationToken = default);
}

public record DiscountResult(string DiscountName, decimal DiscountAmount, decimal DiscountedTotal);
