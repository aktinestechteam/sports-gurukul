using MediatR;
using SportsGurukul.Application.Common.Interfaces.Finance.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;

namespace SportsGurukul.Application.Features.FinanceManagement.Queries;

public class GetPaymentHistoryQueryHandler : IRequestHandler<GetPaymentHistoryQuery, Result<IReadOnlyList<PaymentDto>>>
{
    private readonly IPaymentService _paymentService;

    public GetPaymentHistoryQueryHandler(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    public async Task<Result<IReadOnlyList<PaymentDto>>> Handle(GetPaymentHistoryQuery request, CancellationToken cancellationToken)
    {
        return await _paymentService.GetPaymentHistoryAsync(request.InvoiceId, cancellationToken);
    }
}
