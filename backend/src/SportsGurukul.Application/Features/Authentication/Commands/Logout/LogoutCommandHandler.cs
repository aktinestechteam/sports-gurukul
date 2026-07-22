using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;

namespace SportsGurukul.Application.Features.Authentication.Commands.Logout;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Unit>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IApplicationDbContext _context;
    private readonly ILogger<LogoutCommandHandler> _logger;

    public LogoutCommandHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IApplicationDbContext context,
        ILogger<LogoutCommandHandler> logger)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _context = context;
        _logger = logger;
    }

    public async Task<Unit> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Logout request for user {UserId}", request.UserId);

        var revokedCount = await _refreshTokenRepository.RevokeAllUserTokensAsync(
            request.UserId, null, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Revoked {Count} refresh tokens for user {UserId}", revokedCount, request.UserId);

        return Unit.Value;
    }
}
