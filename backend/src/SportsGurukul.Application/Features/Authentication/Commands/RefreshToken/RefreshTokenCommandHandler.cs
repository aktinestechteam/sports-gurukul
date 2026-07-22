using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.Authentication.Constants;
using SportsGurukul.Application.Features.Authentication.DTOs.Responses;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.Authentication.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<TokenResponse>>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IApplicationDbContext _context;
    private readonly ILogger<RefreshTokenCommandHandler> _logger;

    public RefreshTokenCommandHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IUserRoleRepository userRoleRepository,
        IJwtTokenService jwtTokenService,
        IApplicationDbContext context,
        ILogger<RefreshTokenCommandHandler> logger)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _userRoleRepository = userRoleRepository;
        _jwtTokenService = jwtTokenService;
        _context = context;
        _logger = logger;
    }

    public async Task<Result<TokenResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Refresh token request received");

        var existingToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken, cancellationToken);
        if (existingToken is null)
        {
            _logger.LogWarning("Refresh token not found");
            return Result<TokenResponse>.Failure("Invalid refresh token.");
        }

        if (existingToken.RevokedAt.HasValue)
        {
            _logger.LogWarning("Refresh token {TokenId} has been revoked", existingToken.Id);
            return Result<TokenResponse>.Failure("Refresh token has been revoked.");
        }

        if (existingToken.ExpiresAt <= DateTime.UtcNow)
        {
            _logger.LogWarning("Refresh token {TokenId} has expired", existingToken.Id);
            return Result<TokenResponse>.Failure("Refresh token has expired.");
        }

        var user = await _userRepository.GetByIdAsync(existingToken.UserId, cancellationToken);
        if (user is null)
        {
            _logger.LogWarning("User {UserId} not found for refresh token", existingToken.UserId);
            return Result<TokenResponse>.Failure("User not found.");
        }

        if (user.Status != UserStatus.Active)
        {
            _logger.LogWarning("User {UserId} has status {Status}, cannot refresh token", user.Id, user.Status);
            return Result<TokenResponse>.Failure("User account is not active.");
        }

        existingToken.RevokedAt = DateTime.UtcNow;
        existingToken.ReplacedByToken = _jwtTokenService.GenerateRefreshToken();
        _refreshTokenRepository.Update(existingToken);

        var userRoles = await _userRoleRepository.GetByUserIdAsync(user.Id, cancellationToken);
        var roleNames = new List<string>();
        var permissions = new HashSet<string>();

        foreach (var ur in userRoles)
        {
            var roleWithPerms = await _roleRepository.GetAllWithPermissionsAsync(cancellationToken);
            var matchedRole = roleWithPerms.FirstOrDefault(r => r.Id == ur.RoleId);
            if (matchedRole is not null)
            {
                roleNames.Add(matchedRole.Name);
                foreach (var rp in matchedRole.RolePermissions)
                {
                    if (rp.Permission is not null)
                        permissions.Add(rp.Permission.Name);
                }
            }
        }

        var newAccessToken = _jwtTokenService.GenerateAccessToken(user, roleNames, permissions.ToList());
        var newRefreshTokenValue = existingToken.ReplacedByToken;

        var newRefreshToken = new Domain.Entities.RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = newRefreshTokenValue,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(AuthenticationConstants.RefreshTokenExpirationDays)
        };

        await _refreshTokenRepository.AddAsync(newRefreshToken, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Refresh token rotated for user {UserId}", user.Id);

        return Result<TokenResponse>.Success(new TokenResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshTokenValue,
            AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(AuthenticationConstants.AccessTokenExpirationMinutes)
        });
    }
}
