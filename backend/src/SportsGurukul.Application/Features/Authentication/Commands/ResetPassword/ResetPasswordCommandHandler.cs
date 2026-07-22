using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.Authentication.Commands.ResetPassword;

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result<Unit>>
{
    private readonly IPasswordResetTokenRepository _tokenRepository;
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ResetPasswordCommandHandler> _logger;

    public ResetPasswordCommandHandler(
        IPasswordResetTokenRepository tokenRepository,
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork,
        ILogger<ResetPasswordCommandHandler> logger)
    {
        _tokenRepository = tokenRepository;
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Password reset attempt with token");

        if (request.NewPassword != request.ConfirmNewPassword)
        {
            return Result<Unit>.Failure("Passwords do not match.");
        }

        var tokenEntity = await _tokenRepository.GetByTokenAsync(request.Token, cancellationToken);
        if (tokenEntity is null)
        {
            _logger.LogWarning("Password reset failed: invalid token");
            return Result<Unit>.Failure("Invalid reset token.");
        }

        if (tokenEntity.UsedAt.HasValue)
        {
            _logger.LogWarning("Password reset failed: token already used for user {UserId}", tokenEntity.UserId);
            return Result<Unit>.Failure("Reset token has already been used.");
        }

        if (tokenEntity.ExpiresAt <= DateTime.UtcNow)
        {
            _logger.LogWarning("Password reset failed: token expired for user {UserId}", tokenEntity.UserId);
            return Result<Unit>.Failure("Reset token has expired. Please request a new one.");
        }

        var user = await _userRepository.GetByIdAsync(tokenEntity.UserId, cancellationToken);
        if (user is null)
        {
            _logger.LogWarning("Password reset failed: user not found {UserId}", tokenEntity.UserId);
            return Result<Unit>.Failure("User not found.");
        }

        if (_passwordHasher.VerifyPassword(user.PasswordHash, request.NewPassword))
        {
            _logger.LogWarning("Password reset failed: new password matches current password for user {UserId}", user.Id);
            return Result<Unit>.Failure("New password cannot be the same as your current password.");
        }

        var passwordHash = _passwordHasher.HashPassword(request.NewPassword);
        user.PasswordHash = passwordHash;
        user.FailedLoginAttempts = 0;
        user.LockoutEndAt = null;

        if (user.Status == UserStatus.Locked)
            user.Status = UserStatus.Active;

        _userRepository.Update(user);

        tokenEntity.UsedAt = DateTime.UtcNow;
        _tokenRepository.Update(tokenEntity);

        await _refreshTokenRepository.RevokeAllUserTokensAsync(user.Id, "Password reset", cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Password reset completed for user {UserId}. All refresh tokens revoked.", user.Id);

        return Result<Unit>.Success(Unit.Value);
    }
}
