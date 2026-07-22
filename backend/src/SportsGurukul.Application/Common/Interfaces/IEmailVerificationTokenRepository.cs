using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Common.Interfaces;

public interface IEmailVerificationTokenRepository : IRepository<EmailVerificationToken>
{
    Task<EmailVerificationToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
    Task<EmailVerificationToken?> GetActiveTokenByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<int> InvalidateAllUserTokensAsync(Guid userId, CancellationToken cancellationToken = default);
}
