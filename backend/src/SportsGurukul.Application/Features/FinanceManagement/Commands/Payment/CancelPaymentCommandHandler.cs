using MediatR;
using SportsGurukul.Application.Common.Interfaces.Finance.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;

namespace SportsGurukul.Application.Features.FinanceManagement.Commands.Payment;

public class CancelPaymentCommandHandler : IRequestHandler<CancelPaymentCommand, Result<PaymentDto>>
{
    private readonly IPaymentService _paymentService;

    public CancelPaymentCommandHandler(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    public async Task<Result<PaymentDto>> Handle(CancelPaymentCommand request, CancellationToken cancellationToken)
    {
        return await _paymentService.CancelPaymentAsync(request.PaymentId, request.Reason, cancellationToken);
    }
}
