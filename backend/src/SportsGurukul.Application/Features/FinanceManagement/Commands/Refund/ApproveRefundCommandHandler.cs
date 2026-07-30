using MediatR;
using SportsGurukul.Application.Common.Interfaces.Finance.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;

namespace SportsGurukul.Application.Features.FinanceManagement.Commands.Refund;

public class ApproveRefundCommandHandler : IRequestHandler<ApproveRefundCommand, Result<RefundDto>>
{
    private readonly IRefundService _refundService;

    public ApproveRefundCommandHandler(IRefundService refundService)
    {
        _refundService = refundService;
    }

    public async Task<Result<RefundDto>> Handle(ApproveRefundCommand request, CancellationToken cancellationToken)
    {
        return await _refundService.ApproveRefundAsync(request.RefundId, request.ApprovedBy, cancellationToken);
    }
}
