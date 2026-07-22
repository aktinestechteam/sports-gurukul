using System.Security.Cryptography;
using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.Authentication.Constants;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Features.Authentication.Commands.ForgotPassword;

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, Result<Unit>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordResetTokenRepository _tokenRepository;
    private readonly IEmailService _emailService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ForgotPasswordCommandHandler> _logger;

    public ForgotPasswordCommandHandler(
        IUserRepository userRepository,
        IPasswordResetTokenRepository tokenRepository,
        IEmailService emailService,
        IUnitOfWork unitOfWork,
        ILogger<ForgotPasswordCommandHandler> logger)
    {
        _userRepository = userRepository;
        _tokenRepository = tokenRepository;
        _emailService = emailService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Password reset requested for email: {Email}", request.Email);

        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user is null)
        {
            _logger.LogInformation("Password reset requested for non-existent email (security: returning success)");
            return Result<Unit>.Success(Unit.Value);
        }

        await _tokenRepository.InvalidateAllUserTokensAsync(user.Id, cancellationToken);

        var tokenBytes = new byte[AuthenticationConstants.TokenByteLength];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(tokenBytes);
        }
        var token = Convert.ToBase64String(tokenBytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .TrimEnd('=');

        var resetToken = new PasswordResetToken
        {
            Id = Guid.NewGuid(),
            Token = token,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddMinutes(AuthenticationConstants.PasswordResetTokenExpiryMinutes)
        };

        await _tokenRepository.AddAsync(resetToken, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var resetLink = $"https://sportsgurukul.com/reset-password?token={Uri.EscapeDataString(token)}";

        var htmlBody = EmailTemplates.PasswordResetEmail(user.FullName, resetLink, AuthenticationConstants.PasswordResetTokenExpiryMinutes);

        await _emailService.SendAsync(user.Email, "Sports Gurukul - Password Reset", htmlBody, cancellationToken);

        _logger.LogInformation("Password reset email sent to: {Email}", request.Email);

        return Result<Unit>.Success(Unit.Value);
    }
}
