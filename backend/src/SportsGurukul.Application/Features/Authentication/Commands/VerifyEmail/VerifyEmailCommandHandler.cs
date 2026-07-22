using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.Authentication.Commands.VerifyEmail;

public class VerifyEmailCommandHandler : IRequestHandler<VerifyEmailCommand, Result<Unit>>
{
    private readonly IEmailVerificationTokenRepository _tokenRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<VerifyEmailCommandHandler> _logger;

    public VerifyEmailCommandHandler(
        IEmailVerificationTokenRepository tokenRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        ILogger<VerifyEmailCommandHandler> logger)
    {
        _tokenRepository = tokenRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Email verification attempt with token");

        var tokenEntity = await _tokenRepository.GetByTokenAsync(request.Token, cancellationToken);
        if (tokenEntity is null)
        {
            _logger.LogWarning("Email verification failed: invalid token");
            return Result<Unit>.Failure("Invalid verification token.");
        }

        if (tokenEntity.UsedAt.HasValue)
        {
            _logger.LogWarning("Email verification failed: token already used for user {UserId}", tokenEntity.UserId);
            return Result<Unit>.Failure("Verification token has already been used.");
        }

        if (tokenEntity.ExpiresAt <= DateTime.UtcNow)
        {
            _logger.LogWarning("Email verification failed: token expired for user {UserId}", tokenEntity.UserId);
            return Result<Unit>.Failure("Verification token has expired. Please request a new one.");
        }

        var user = await _userRepository.GetByIdAsync(tokenEntity.UserId, cancellationToken);
        if (user is null)
        {
            _logger.LogWarning("Email verification failed: user not found {UserId}", tokenEntity.UserId);
            return Result<Unit>.Failure("User not found.");
        }

        if (user.IsEmailVerified)
        {
            _logger.LogInformation("Email already verified for user {UserId}", user.Id);
            return Result<Unit>.Success(Unit.Value);
        }

        user.IsEmailVerified = true;
        _userRepository.Update(user);

        tokenEntity.UsedAt = DateTime.UtcNow;
        _tokenRepository.Update(tokenEntity);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Email verified successfully for user {UserId}", user.Id);

        return Result<Unit>.Success(Unit.Value);
    }
}
