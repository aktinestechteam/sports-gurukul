using MediatR;
using SportsGurukul.Application.Common.Interfaces.Finance.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;

namespace SportsGurukul.Application.Features.FinanceManagement.Commands.Payment;

public class RetryPaymentCommandHandler : IRequestHandler<RetryPaymentCommand, Result<PaymentDto>>
{
    private readonly IPaymentService _paymentService;

    public RetryPaymentCommandHandler(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    public async Task<Result<PaymentDto>> Handle(RetryPaymentCommand request, CancellationToken cancellationToken)
    {
        return await _paymentService.RetryPaymentAsync(request.PaymentId, cancellationToken);
    }
}
