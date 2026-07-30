using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces.Finance;
using SportsGurukul.Domain.Entities.Finance;

namespace SportsGurukul.Infrastructure.Persistence.Repositories.Finance;

public class CouponRepository : Repository<Coupon>, ICouponRepository
{
    public CouponRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Coupon?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Coupon>()
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Code == code, cancellationToken);
    }

    public async Task<Coupon?> GetByCodeWithUsagesAsync(string code, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Coupon>()
            .AsNoTracking()
            .Include(c => c.Usages)
            .FirstOrDefaultAsync(c => c.Code == code, cancellationToken);
    }

    public async Task<IReadOnlyList<Coupon>> GetActiveCouponsAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await Context.Set<Coupon>()
            .AsNoTracking()
            .Where(c => c.IsActive && c.ValidFrom <= now && c.ValidTo >= now)
            .ToListAsync(cancellationToken);
    }
}
