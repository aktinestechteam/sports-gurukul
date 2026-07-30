using MediatR;
using SportsGurukul.Application.Common.Interfaces.Finance;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.FinanceManagement.Queries;

public class PaymentSearchQueryHandler : IRequestHandler<PaymentSearchQuery, Result<IReadOnlyList<PaymentSearchHit>>>
{
    private readonly IPaymentRepository _paymentRepository;

    public PaymentSearchQueryHandler(IPaymentRepository paymentRepository)
    {
        _paymentRepository = paymentRepository;
    }

    public async Task<Result<IReadOnlyList<PaymentSearchHit>>> Handle(PaymentSearchQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<Domain.Entities.Finance.Payment> payments;

        if (request.InvoiceId.HasValue)
            payments = await _paymentRepository.GetByInvoiceIdAsync(request.InvoiceId.Value, cancellationToken);
        else
            payments = await _paymentRepository.GetAllAsync(cancellationToken);

        var filtered = payments.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            filtered = filtered.Where(p => p.PaymentReference.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase));
        if (request.Status.HasValue)
            filtered = filtered.Where(p => p.Status == request.Status.Value);
        if (request.FromDate.HasValue)
            filtered = filtered.Where(p => p.PaymentDate >= request.FromDate.Value);
        if (request.ToDate.HasValue)
            filtered = filtered.Where(p => p.PaymentDate <= request.ToDate.Value);

        var paged = filtered
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => new PaymentSearchHit(p.Id, p.PaymentReference, p.Amount, p.Status.ToString(), p.CreatedAt))
            .ToList();

        return Result<IReadOnlyList<PaymentSearchHit>>.Success(paged);
    }
}
