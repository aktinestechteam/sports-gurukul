using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Common.Interfaces;

public interface IPasswordResetTokenRepository : IRepository<PasswordResetToken>
{
    Task<PasswordResetToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
    Task<PasswordResetToken?> GetActiveTokenByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<int> InvalidateAllUserTokensAsync(Guid userId, CancellationToken cancellationToken = default);
}
