using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Repositories;

public class PasswordResetTokenRepository : Repository<PasswordResetToken>, IPasswordResetTokenRepository
{
    private readonly ApplicationDbContext _context;

    public PasswordResetTokenRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<PasswordResetToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await _context.PasswordResetTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Token == token && !t.IsDeleted, cancellationToken);
    }

    public async Task<PasswordResetToken?> GetActiveTokenByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.PasswordResetTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(t =>
                t.UserId == userId
                && !t.IsDeleted
                && t.UsedAt == null
                && t.ExpiresAt > DateTime.UtcNow,
                cancellationToken);
    }

    public async Task<int> InvalidateAllUserTokensAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var activeTokens = await _context.PasswordResetTokens
            .Where(t => t.UserId == userId && !t.IsDeleted && t.UsedAt == null)
            .ToListAsync(cancellationToken);

        foreach (var token in activeTokens)
        {
            token.UsedAt = DateTime.UtcNow;
        }

        _context.PasswordResetTokens.UpdateRange(activeTokens);
        return activeTokens.Count;
    }
}
