using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Repositories;

public class RefreshTokenRepository : Repository<RefreshToken>, IRefreshTokenRepository
{
    public RefreshTokenRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await Context.RefreshTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(rt => rt.Token == token && !rt.IsDeleted, cancellationToken);
    }

    public async Task<IReadOnlyList<RefreshToken>> GetActiveTokensByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await Context.RefreshTokens
            .AsNoTracking()
            .Where(rt => rt.UserId == userId
                && !rt.IsDeleted
                && rt.RevokedAt == null
                && rt.ExpiresAt > DateTime.UtcNow)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> RevokeAllUserTokensAsync(
        Guid userId,
        string? replacedByToken,
        CancellationToken cancellationToken = default)
    {
        var activeTokens = await Context.RefreshTokens
            .Where(rt => rt.UserId == userId
                && !rt.IsDeleted
                && rt.RevokedAt == null)
            .ToListAsync(cancellationToken);

        foreach (var token in activeTokens)
        {
            token.RevokedAt = DateTime.UtcNow;
            token.ReplacedByToken = replacedByToken;
        }

        Context.RefreshTokens.UpdateRange(activeTokens);
        return activeTokens.Count;
    }
}
