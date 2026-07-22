using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.Authentication.Constants;
using SportsGurukul.Application.Features.Authentication.DTOs.Responses;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.Authentication.Commands.LoginUser;

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, Result<LoginResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IApplicationDbContext _context;
    private readonly ILogger<LoginUserCommandHandler> _logger;

    public LoginUserCommandHandler(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IUserRoleRepository userRoleRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService,
        IApplicationDbContext context,
        ILogger<LoginUserCommandHandler> logger)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _userRoleRepository = userRoleRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _context = context;
        _logger = logger;
    }

    public async Task<Result<LoginResponse>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Login attempt for email: {Email}", request.Email);

        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user is null)
        {
            _logger.LogWarning("Login failed: no user found with email {Email}", request.Email);
            return Result<LoginResponse>.Failure("Invalid email or password.");
        }

        if (user.LockoutEndAt.HasValue && user.LockoutEndAt.Value > DateTime.UtcNow)
        {
            _logger.LogWarning("Login failed: account {UserId} is locked until {LockoutEnd}", user.Id, user.LockoutEndAt.Value);
            return Result<LoginResponse>.Failure($"Account is locked. Try again after {user.LockoutEndAt.Value:HH:mm UTC}.");
        }

        if (!_passwordHasher.VerifyPassword(user.PasswordHash, request.Password))
        {
            user.FailedLoginAttempts++;

            if (user.FailedLoginAttempts >= AuthenticationConstants.MaxFailedLoginAttempts)
            {
                user.LockoutEndAt = DateTime.UtcNow.AddMinutes(AuthenticationConstants.LockoutDurationMinutes);
                user.Status = UserStatus.Locked;
                _logger.LogWarning("Account {UserId} locked after {Attempts} failed login attempts",
                    user.Id, user.FailedLoginAttempts);
            }

            _userRepository.Update(user);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogWarning("Login failed: invalid password for email {Email}", request.Email);
            return Result<LoginResponse>.Failure("Invalid email or password.");
        }

        user.FailedLoginAttempts = 0;
        user.LockoutEndAt = null;
        user.LastLoginAt = DateTime.UtcNow;

        if (user.Status == UserStatus.Locked)
            user.Status = UserStatus.Active;

        _userRepository.Update(user);

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

        var accessToken = _jwtTokenService.GenerateAccessToken(user, roleNames, permissions.ToList());
        var refreshTokenValue = _jwtTokenService.GenerateRefreshToken();

        var refreshToken = new Domain.Entities.RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = refreshTokenValue,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(AuthenticationConstants.RefreshTokenExpirationDays)
        };

        await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User logged in successfully: {UserId}", user.Id);

        return Result<LoginResponse>.Success(new LoginResponse
        {
            UserId = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            AccessToken = accessToken,
            RefreshToken = refreshTokenValue,
            AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(AuthenticationConstants.AccessTokenExpirationMinutes),
            Roles = roleNames
        });
    }
}
