using MediatR;
using SportsGurukul.Application.Common.Interfaces.Finance.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;

namespace SportsGurukul.Application.Features.FinanceManagement.Commands.Payment;

public class AuthorizePaymentCommandHandler : IRequestHandler<AuthorizePaymentCommand, Result<PaymentDto>>
{
    private readonly IPaymentService _paymentService;

    public AuthorizePaymentCommandHandler(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    public async Task<Result<PaymentDto>> Handle(AuthorizePaymentCommand request, CancellationToken cancellationToken)
    {
        return await _paymentService.AuthorizePaymentAsync(request.PaymentId, cancellationToken);
    }
}
