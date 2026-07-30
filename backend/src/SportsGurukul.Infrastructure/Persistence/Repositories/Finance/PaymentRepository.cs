using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces.Finance;
using SportsGurukul.Domain.Entities.Finance;

namespace SportsGurukul.Infrastructure.Persistence.Repositories.Finance;

public class PaymentRepository : Repository<Payment>, IPaymentRepository
{
    public PaymentRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Payment?> GetByPaymentReferenceAsync(string paymentReference, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Payment>()
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.PaymentReference == paymentReference, cancellationToken);
    }

    public async Task<Payment?> GetByIdWithTransactionsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Payment>()
            .AsNoTracking()
            .Include(p => p.Transactions)
            .Include(p => p.GatewayTransactions)
            .Include(p => p.Refunds)
            .Include(p => p.Receipts)
            .AsSplitQuery()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Payment>> GetByInvoiceIdAsync(Guid invoiceId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Payment>()
            .AsNoTracking()
            .Where(p => p.InvoiceId == invoiceId)
            .ToListAsync(cancellationToken);
    }

    public async Task<Payment?> GetByIdempotencyKeyAsync(string idempotencyKey, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Payment>()
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.IdempotencyKey == idempotencyKey, cancellationToken);
    }
}
