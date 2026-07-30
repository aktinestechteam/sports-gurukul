using MediatR;
using SportsGurukul.Application.Common.Interfaces.Finance.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;

namespace SportsGurukul.Application.Features.FinanceManagement.Commands.Payment;

public class CapturePaymentCommandHandler : IRequestHandler<CapturePaymentCommand, Result<PaymentDto>>
{
    private readonly IPaymentService _paymentService;

    public CapturePaymentCommandHandler(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    public async Task<Result<PaymentDto>> Handle(CapturePaymentCommand request, CancellationToken cancellationToken)
    {
        return await _paymentService.CapturePaymentAsync(request.PaymentId, cancellationToken);
    }
}
