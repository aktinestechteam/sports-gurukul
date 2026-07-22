using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.Authentication.Constants;
using SportsGurukul.Application.Features.Authentication.DTOs.Responses;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.Authentication.Commands.RegisterUser;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Result<AuthResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IApplicationDbContext _context;
    private readonly ILogger<RegisterUserCommandHandler> _logger;

    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IUserRoleRepository userRoleRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService,
        IApplicationDbContext context,
        ILogger<RegisterUserCommandHandler> logger)
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

    public async Task<Result<AuthResponse>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Attempting to register user with email: {Email}", request.Email);

        var existingUserByEmail = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existingUserByEmail is not null)
        {
            _logger.LogWarning("Registration failed: email {Email} already exists", request.Email);
            return Result<AuthResponse>.Failure("An account with this email already exists.");
        }

        if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
        {
            var existingUserByPhone = await _userRepository.GetByPhoneNumberAsync(request.PhoneNumber, cancellationToken);
            if (existingUserByPhone is not null)
            {
                _logger.LogWarning("Registration failed: phone number {PhoneNumber} already exists", request.PhoneNumber);
                return Result<AuthResponse>.Failure("An account with this phone number already exists.");
            }
        }

        var defaultRole = await _roleRepository.GetByNameAsync(AuthenticationConstants.DefaultRole, cancellationToken);
        if (defaultRole is null)
        {
            _logger.LogError("Default role '{DefaultRole}' not found in database", AuthenticationConstants.DefaultRole);
            return Result<AuthResponse>.Failure("System configuration error. Please contact support.");
        }

        var passwordHash = _passwordHasher.HashPassword(request.Password);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            PhoneNumber = request.PhoneNumber ?? string.Empty,
            PasswordHash = passwordHash,
            FullName = request.FullName,
            Status = UserStatus.Active,
            AuthMethod = AuthenticationMethod.EmailPassword,
            FailedLoginAttempts = 0
        };

        await _userRepository.AddAsync(user, cancellationToken);

        var userRole = new UserRole
        {
            UserId = user.Id,
            RoleId = defaultRole.Id,
            AssignedAt = DateTime.UtcNow
        };

        await _userRoleRepository.AddAsync(userRole, cancellationToken);

        var roles = new List<string> { defaultRole.Name };
        var permissions = new List<string>();

        var accessToken = _jwtTokenService.GenerateAccessToken(user, roles, permissions);
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

        _logger.LogInformation("User registered successfully: {UserId}", user.Id);

        return Result<AuthResponse>.Success(new AuthResponse
        {
            UserId = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            AccessToken = accessToken,
            RefreshToken = refreshTokenValue,
            AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(AuthenticationConstants.AccessTokenExpirationMinutes),
            Roles = roles
        });
    }
}
