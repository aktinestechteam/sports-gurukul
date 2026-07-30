using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces.Notification;
using SportsGurukul.Domain.Entities.Notification;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Infrastructure.Persistence.Repositories.Notification;

public class PreferenceRepository : Repository<NotificationPreference>, IPreferenceRepository
{
    public PreferenceRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IReadOnlyList<NotificationPreference>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<NotificationPreference>()
            .AsNoTracking()
            .Where(p => p.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<NotificationPreference?> GetByUserAndChannelAsync(Guid userId, NotificationChannelType channelType, CancellationToken cancellationToken = default)
    {
        return await Context.Set<NotificationPreference>()
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.UserId == userId && p.ChannelType == channelType, cancellationToken);
    }

    public async Task<bool> IsChannelEnabledAsync(Guid userId, NotificationChannelType channelType, CancellationToken cancellationToken = default)
    {
        return await Context.Set<NotificationPreference>()
            .AsNoTracking()
            .Where(p => p.UserId == userId && p.ChannelType == channelType)
            .Select(p => p.IsEnabled)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
