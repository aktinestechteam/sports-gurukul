using System.Security.Cryptography;
using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.Authentication.Constants;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Features.Authentication.Commands.SendEmailVerification;

public class SendEmailVerificationCommandHandler : IRequestHandler<SendEmailVerificationCommand, Result<Unit>>
{
    private readonly IUserRepository _userRepository;
    private readonly IEmailVerificationTokenRepository _tokenRepository;
    private readonly IEmailService _emailService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SendEmailVerificationCommandHandler> _logger;

    public SendEmailVerificationCommandHandler(
        IUserRepository userRepository,
        IEmailVerificationTokenRepository tokenRepository,
        IEmailService emailService,
        IUnitOfWork unitOfWork,
        ILogger<SendEmailVerificationCommandHandler> logger)
    {
        _userRepository = userRepository;
        _tokenRepository = tokenRepository;
        _emailService = emailService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(SendEmailVerificationCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Email verification requested for: {Email}", request.Email);

        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user is null)
        {
            _logger.LogInformation("Email verification requested for non-existent email: {Email}", request.Email);
            return Result<Unit>.Success(Unit.Value);
        }

        if (user.IsEmailVerified)
        {
            _logger.LogInformation("Email already verified for user: {UserId}", user.Id);
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

        var verificationToken = new EmailVerificationToken
        {
            Id = Guid.NewGuid(),
            Token = token,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddHours(AuthenticationConstants.EmailVerificationTokenExpiryHours)
        };

        await _tokenRepository.AddAsync(verificationToken, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var verificationLink = $"https://sportsgurukul.com/verify-email?token={Uri.EscapeDataString(token)}";

        var htmlBody = EmailTemplates.VerificationEmail(user.FullName, verificationLink, AuthenticationConstants.EmailVerificationTokenExpiryHours);

        await _emailService.SendAsync(user.Email, "Verify your Sports Gurukul email", htmlBody, cancellationToken);

        _logger.LogInformation("Verification email sent to: {Email}", request.Email);

        return Result<Unit>.Success(Unit.Value);
    }
}
