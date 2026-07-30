using MediatR;
using SportsGurukul.Application.Common.Interfaces.Finance.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;

namespace SportsGurukul.Application.Features.FinanceManagement.Commands.Payment;

public class RecordOfflinePaymentCommandHandler : IRequestHandler<RecordOfflinePaymentCommand, Result<PaymentDto>>
{
    private readonly IPaymentService _paymentService;

    public RecordOfflinePaymentCommandHandler(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    public async Task<Result<PaymentDto>> Handle(RecordOfflinePaymentCommand request, CancellationToken cancellationToken)
    {
        var recordRequest = new RecordOfflinePaymentRequest(
            request.InvoiceId,
            request.Amount,
            request.PaymentMethod,
            request.Reference,
            request.PaidAt,
            request.Description
        );
        return await _paymentService.RecordOfflinePaymentAsync(recordRequest, cancellationToken);
    }
}
