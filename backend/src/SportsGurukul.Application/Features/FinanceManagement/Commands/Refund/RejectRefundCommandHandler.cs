using MediatR;
using SportsGurukul.Application.Common.Interfaces.Finance.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;

namespace SportsGurukul.Application.Features.FinanceManagement.Commands.Refund;

public class RejectRefundCommandHandler : IRequestHandler<RejectRefundCommand, Result<RefundDto>>
{
    private readonly IRefundService _refundService;

    public RejectRefundCommandHandler(IRefundService refundService)
    {
        _refundService = refundService;
    }

    public async Task<Result<RefundDto>> Handle(RejectRefundCommand request, CancellationToken cancellationToken)
    {
        return await _refundService.RejectRefundAsync(request.RefundId, request.Reason, cancellationToken);
    }
}
