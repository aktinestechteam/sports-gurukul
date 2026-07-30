using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces.Notification;
using SportsGurukul.Domain.Entities.Notification;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Infrastructure.Persistence.Repositories.Notification;

public class TemplateRepository : Repository<NotificationTemplate>, ITemplateRepository
{
    public TemplateRepository(ApplicationDbContext context) : base(context) { }

    public async Task<NotificationTemplate?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await Context.Set<NotificationTemplate>()
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Name == name, cancellationToken);
    }

    public async Task<NotificationTemplate?> GetWithVersionsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<NotificationTemplate>()
            .Include(t => t.Versions)
            .Include(t => t.Variables)
            .AsSplitQuery()
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<NotificationTemplate>> GetByChannelAsync(NotificationChannelType channelType, CancellationToken cancellationToken = default)
    {
        return await Context.Set<NotificationTemplate>()
            .AsNoTracking()
            .Where(t => t.ChannelType == channelType)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<NotificationTemplate>> GetActiveTemplatesAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Set<NotificationTemplate>()
            .AsNoTracking()
            .Where(t => t.IsActive)
            .ToListAsync(cancellationToken);
    }
}
