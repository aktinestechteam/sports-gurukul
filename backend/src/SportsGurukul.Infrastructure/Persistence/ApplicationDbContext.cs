namespace SportsGurukul.Infrastructure.Persistence;

using SportsGurukul.Application.Common.Interfaces;

public class ApplicationDbContext : IApplicationDbContext
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(0);
    }
}
