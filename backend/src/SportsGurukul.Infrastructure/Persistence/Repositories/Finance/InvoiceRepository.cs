using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces.Finance;
using SportsGurukul.Domain.Entities.Finance;
using SportsGurukul.Domain.Enums.Finance;

namespace SportsGurukul.Infrastructure.Persistence.Repositories.Finance;

public class InvoiceRepository : Repository<Invoice>, IInvoiceRepository
{
    public InvoiceRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Invoice?> GetByInvoiceNumberAsync(string invoiceNumber, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Invoice>()
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.InvoiceNumber == invoiceNumber, cancellationToken);
    }

    public async Task<Invoice?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Invoice>()
            .AsNoTracking()
            .Include(i => i.Items)
            .Include(i => i.Taxes)
            .Include(i => i.Discounts)
            .Include(i => i.InvoicePayments).ThenInclude(ip => ip.Payment)
            .Include(i => i.Reminders)
            .Include(i => i.CreditNotes)
            .Include(i => i.DebitNotes)
            .AsSplitQuery()
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Invoice>> GetByAthleteIdAsync(Guid athleteId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Invoice>()
            .AsNoTracking()
            .Where(i => i.AthleteId == athleteId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Invoice>> GetByAcademyIdAsync(Guid academyId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Invoice>()
            .AsNoTracking()
            .Where(i => i.AcademyId == academyId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Invoice>> GetByStatusAsync(InvoiceStatus status, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Invoice>()
            .AsNoTracking()
            .Where(i => i.Status == status)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Invoice>> GetOverdueInvoicesAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Set<Invoice>()
            .AsNoTracking()
            .Where(i => i.Status == InvoiceStatus.Issued || i.Status == InvoiceStatus.PartiallyPaid)
            .Where(i => i.DueDate < DateTime.UtcNow)
            .ToListAsync(cancellationToken);
    }
}
