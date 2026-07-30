using MediatR;
using SportsGurukul.Application.Common.Interfaces.Finance.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;

namespace SportsGurukul.Application.Features.FinanceManagement.Queries;

public class GetRefundHistoryQueryHandler : IRequestHandler<GetRefundHistoryQuery, Result<IReadOnlyList<RefundDto>>>
{
    private readonly IRefundService _refundService;

    public GetRefundHistoryQueryHandler(IRefundService refundService)
    {
        _refundService = refundService;
    }

    public async Task<Result<IReadOnlyList<RefundDto>>> Handle(GetRefundHistoryQuery request, CancellationToken cancellationToken)
    {
        return await _refundService.GetRefundHistoryAsync(request.PaymentId, cancellationToken);
    }
}
