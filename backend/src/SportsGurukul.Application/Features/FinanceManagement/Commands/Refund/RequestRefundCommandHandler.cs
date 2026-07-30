using MediatR;
using SportsGurukul.Application.Common.Interfaces.Finance.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;

namespace SportsGurukul.Application.Features.FinanceManagement.Commands.Refund;

public class RequestRefundCommandHandler : IRequestHandler<RequestRefundCommand, Result<RefundDto>>
{
    private readonly IRefundService _refundService;

    public RequestRefundCommandHandler(IRefundService refundService)
    {
        _refundService = refundService;
    }

    public async Task<Result<RefundDto>> Handle(RequestRefundCommand request, CancellationToken cancellationToken)
    {
        var refundRequest = new RequestRefundRequest(request.PaymentId, request.Amount, request.Reason, request.Items);
        return await _refundService.RequestRefundAsync(refundRequest, cancellationToken);
    }
}
