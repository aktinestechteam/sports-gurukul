using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;

namespace SportsGurukul.Application.Common.Interfaces.Finance.Services;

public interface IRefundService
{
    Task<Result<RefundDto>> RequestRefundAsync(RequestRefundRequest request, CancellationToken cancellationToken = default);
    Task<Result<RefundDto>> ApproveRefundAsync(Guid refundId, string approvedBy, CancellationToken cancellationToken = default);
    Task<Result<RefundDto>> RejectRefundAsync(Guid refundId, string reason, CancellationToken cancellationToken = default);
    Task<Result<RefundDto>> CompleteRefundAsync(Guid refundId, string? gatewayReference = null, CancellationToken cancellationToken = default);
    Task<Result<string>> GenerateRefundNumberAsync(CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<RefundDto>>> GetRefundHistoryAsync(Guid paymentId, CancellationToken cancellationToken = default);
}
