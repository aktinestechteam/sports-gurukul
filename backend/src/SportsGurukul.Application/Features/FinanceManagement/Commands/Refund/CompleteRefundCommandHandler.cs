using MediatR;
using SportsGurukul.Application.Common.Interfaces.Finance.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;

namespace SportsGurukul.Application.Features.FinanceManagement.Commands.Refund;

public class CompleteRefundCommandHandler : IRequestHandler<CompleteRefundCommand, Result<RefundDto>>
{
    private readonly IRefundService _refundService;

    public CompleteRefundCommandHandler(IRefundService refundService)
    {
        _refundService = refundService;
    }

    public async Task<Result<RefundDto>> Handle(CompleteRefundCommand request, CancellationToken cancellationToken)
    {
        return await _refundService.CompleteRefundAsync(request.RefundId, request.GatewayReference, cancellationToken);
    }
}
