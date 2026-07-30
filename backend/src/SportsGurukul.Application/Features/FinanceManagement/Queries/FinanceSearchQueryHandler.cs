using MediatR;
using SportsGurukul.Application.Common.Interfaces.Finance;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.FinanceManagement.Queries;

public class FinanceSearchQueryHandler : IRequestHandler<FinanceSearchQuery, Result<FinanceSearchResultDto>>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IRefundRepository _refundRepository;

    public FinanceSearchQueryHandler(IInvoiceRepository invoiceRepository, IPaymentRepository paymentRepository, IRefundRepository refundRepository)
    {
        _invoiceRepository = invoiceRepository;
        _paymentRepository = paymentRepository;
        _refundRepository = refundRepository;
    }

    public async Task<Result<FinanceSearchResultDto>> Handle(FinanceSearchQuery request, CancellationToken cancellationToken)
    {
        var invoices = await _invoiceRepository.GetAllAsync(cancellationToken);
        var payments = await _paymentRepository.GetAllAsync(cancellationToken);
        var refunds = await _refundRepository.GetAllAsync(cancellationToken);

        var invoiceHits = invoices
            .Where(i => string.IsNullOrEmpty(request.SearchTerm) || i.InvoiceNumber.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase))
            .Select(i => new InvoiceSearchHit(i.Id, i.InvoiceNumber ?? string.Empty, i.Total, i.Status.ToString(), i.CreatedAt))
            .ToList();

        var paymentHits = payments
            .Where(p => string.IsNullOrEmpty(request.SearchTerm) || p.PaymentReference.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase))
            .Select(p => new PaymentSearchHit(p.Id, p.PaymentReference, p.Amount, p.Status.ToString(), p.CreatedAt))
            .ToList();

        var refundHits = refunds
            .Where(r => string.IsNullOrEmpty(request.SearchTerm) || r.RefundNumber.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase))
            .Select(r => new RefundSearchHit(r.Id, r.RefundNumber, r.TotalAmount, r.Status.ToString(), r.CreatedAt))
            .ToList();

        var result = new FinanceSearchResultDto(invoiceHits, paymentHits, refundHits, invoiceHits.Count + paymentHits.Count + refundHits.Count);
        return Result<FinanceSearchResultDto>.Success(result);
    }
}
