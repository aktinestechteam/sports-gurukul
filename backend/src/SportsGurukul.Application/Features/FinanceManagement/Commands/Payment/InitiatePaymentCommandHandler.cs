using MediatR;
using SportsGurukul.Application.Common.Interfaces.Finance.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;

namespace SportsGurukul.Application.Features.FinanceManagement.Commands.Payment;

public class InitiatePaymentCommandHandler : IRequestHandler<InitiatePaymentCommand, Result<PaymentDto>>
{
    private readonly IPaymentService _paymentService;

    public InitiatePaymentCommandHandler(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    public async Task<Result<PaymentDto>> Handle(InitiatePaymentCommand request, CancellationToken cancellationToken)
    {
        var initiateRequest = new InitiatePaymentRequest(
            request.InvoiceId,
            request.Amount,
            request.PaymentMethod,
            request.IdempotencyKey,
            request.Description
        );
        return await _paymentService.InitiatePaymentAsync(initiateRequest, cancellationToken);
    }
}
