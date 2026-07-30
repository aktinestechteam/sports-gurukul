using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces.Finance;
using SportsGurukul.Domain.Entities.Finance;

namespace SportsGurukul.Infrastructure.Persistence.Repositories.Finance;

public class RefundRepository : Repository<Refund>, IRefundRepository
{
    public RefundRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Refund?> GetByRefundNumberAsync(string refundNumber, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Refund>()
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.RefundNumber == refundNumber, cancellationToken);
    }

    public async Task<Refund?> GetByIdWithItemsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Refund>()
            .AsNoTracking()
            .Include(r => r.RefundItems)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Refund>> GetByPaymentIdAsync(Guid paymentId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Refund>()
            .AsNoTracking()
            .Where(r => r.PaymentId == paymentId)
            .ToListAsync(cancellationToken);
    }
}
