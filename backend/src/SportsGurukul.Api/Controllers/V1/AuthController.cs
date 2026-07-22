using System.Net.Mime;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Api.Common.Models.SwaggerExamples;
using SportsGurukul.Application.Features.Authentication.Commands.ForgotPassword;
using SportsGurukul.Application.Features.Authentication.Commands.LoginUser;
using SportsGurukul.Application.Features.Authentication.Commands.Logout;
using SportsGurukul.Application.Features.Authentication.Commands.RefreshToken;
using SportsGurukul.Application.Features.Authentication.Commands.RegisterUser;
using SportsGurukul.Application.Features.Authentication.Commands.ResetPassword;
using SportsGurukul.Application.Features.Authentication.Commands.SendEmailVerification;
using SportsGurukul.Application.Features.Authentication.Commands.VerifyEmail;
using SportsGurukul.Application.Features.Authentication.DTOs.Requests;
using SportsGurukul.Application.Features.Authentication.DTOs.Responses;
using Swashbuckle.AspNetCore.Filters;

namespace SportsGurukul.Api.Controllers.V1;

/// <summary>
/// Handles user authentication operations: registration, login, token refresh, logout,
/// email verification, and password reset.
/// </summary>
[ApiController]
[Route("api/v1/auth")]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IMediator mediator, ILogger<AuthController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    #region Registration & Login

    /// <summary>
    /// Registers a new user account.
    /// </summary>
    /// <remarks>
    /// Creates a new user with the Athlete role by default.
    /// Password must be at least 8 characters with uppercase, lowercase, number, and special character.
    /// </remarks>
    /// <param name="request">Registration details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created user details with JWT tokens</returns>
    /// <response code="201">User registered successfully</response>
    /// <response code="400">Validation error or duplicate email/phone</response>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [SwaggerRequestExample(typeof(RegisterRequest), typeof(RegisterRequestExample))]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Registration attempt for email: {Email}", request.Email);

        var command = new RegisterUserCommand
        {
            FullName = request.FullName,
            Email = request.Email,
            Password = request.Password,
            ConfirmPassword = request.ConfirmPassword,
            PhoneNumber = request.PhoneNumber
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Registration failed for email {Email}: {Error}", request.Email, result.Error);
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Registration Failed",
                Detail = result.Error,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
            });
        }

        _logger.LogInformation("User registered successfully: {UserId}", result.Value!.UserId);

        return CreatedAtAction(
            nameof(Register),
            new { version = "1.0" },
            ApiResponse<AuthResponse>.SuccessResult(result.Value, "User registered successfully."));
    }

    /// <summary>
    /// Authenticates a user with email and password.
    /// </summary>
    /// <remarks>
    /// Returns JWT access token and refresh token.
    /// Account locks after 5 failed attempts for 15 minutes.
    /// </remarks>
    /// <param name="request">Login credentials</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Access token and refresh token</returns>
    /// <response code="200">Login successful</response>
    /// <response code="401">Invalid credentials or locked account</response>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [SwaggerRequestExample(typeof(LoginRequest), typeof(LoginRequestExample))]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Login attempt for email: {Email}", request.Email);

        var command = new LoginUserCommand
        {
            Email = request.Email,
            Password = request.Password
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Login failed for email {Email}: {Error}", request.Email, result.Error);

            var isLocked = result.Error!.Contains("locked", StringComparison.OrdinalIgnoreCase);

            if (isLocked)
            {
                return Unauthorized(new ProblemDetails
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Title = "Account Locked",
                    Detail = result.Error,
                    Type = "https://tools.ietf.org/html/rfc7235#section-3.1"
                });
            }

            return Unauthorized(new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Authentication Failed",
                Detail = result.Error,
                Type = "https://tools.ietf.org/html/rfc7235#section-3.1"
            });
        }

        _logger.LogInformation("User logged in successfully: {UserId}", result.Value!.UserId);

        return Ok(ApiResponse<LoginResponse>.SuccessResult(result.Value, "Login successful."));
    }

    /// <summary>
    /// Refreshes an access token using a valid refresh token.
    /// </summary>
    /// <remarks>
    /// Implements refresh token rotation: the old refresh token is revoked
    /// and a new pair of access + refresh tokens is issued.
    /// </remarks>
    /// <param name="request">Refresh token</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>New access token and refresh token</returns>
    /// <response code="200">Token refreshed successfully</response>
    /// <response code="401">Invalid, expired, or revoked refresh token</response>
    [HttpPost("refresh-token")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<TokenResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [SwaggerRequestExample(typeof(RefreshTokenRequest), typeof(RefreshTokenRequestExample))]
    public async Task<IActionResult> RefreshToken(
        [FromBody] RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Refresh token request received");

        var command = new RefreshTokenCommand
        {
            RefreshToken = request.RefreshToken
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Refresh token failed: {Error}", result.Error);
            return Unauthorized(new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Token Refresh Failed",
                Detail = result.Error,
                Type = "https://tools.ietf.org/html/rfc7235#section-3.1"
            });
        }

        _logger.LogInformation("Token refreshed successfully");

        return Ok(ApiResponse<TokenResponse>.SuccessResult(result.Value!, "Token refreshed successfully."));
    }

    /// <summary>
    /// Logs out the current user by revoking all active refresh tokens.
    /// </summary>
    /// <remarks>
    /// Requires a valid JWT access token.
    /// Revokes all active refresh tokens for the authenticated user.
    /// </remarks>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content</returns>
    /// <response code="204">Logout successful</response>
    /// <response code="401">Not authenticated</response>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Unauthorized",
                Detail = "Invalid user identity.",
                Type = "https://tools.ietf.org/html/rfc7235#section-3.1"
            });
        }

        _logger.LogInformation("Logout request for user: {UserId}", userId);

        var command = new LogoutCommand { UserId = userId };
        await _mediator.Send(command, cancellationToken);

        _logger.LogInformation("User {UserId} logged out successfully", userId);

        return NoContent();
    }

    #endregion

    #region Email Verification

    /// <summary>
    /// Sends a verification email to the specified address.
    /// </summary>
    /// <remarks>
    /// Always returns 200 regardless of whether the email exists (prevents email enumeration).
    /// If the email is already verified, no email is sent.
    /// Rate limit: 5 requests per email per hour.
    /// </remarks>
    /// <param name="request">Email address to verify</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Confirmation message</returns>
    /// <response code="200">Verification email sent (or email already verified)</response>
    /// <response code="400">Validation error</response>
    [HttpPost("send-verification-email")]
    [AllowAnonymous]
    [EnableRateLimiting("sensitive")]
    [ProducesResponseType(typeof(ApiResponse<MessageResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [SwaggerRequestExample(typeof(SendVerificationEmailRequest), typeof(SendVerificationEmailRequestExample))]
    public async Task<IActionResult> SendVerificationEmail(
        [FromBody] SendVerificationEmailRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Verification email request for: {Email}", request.Email);

        var command = new SendEmailVerificationCommand { Email = request.Email };
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Request Failed",
                Detail = result.Error,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
            });
        }

        return Ok(ApiResponse<MessageResponse>.SuccessResult(
            new MessageResponse { Message = "If an account with that email exists, a verification link has been sent." },
            "Request processed."));
    }

    /// <summary>
    /// Verifies a user's email address using the token from the verification email.
    /// </summary>
    /// <remarks>
    /// Tokens expire after 24 hours. Each token can only be used once.
    /// </remarks>
    /// <param name="request">Verification token</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Confirmation message</returns>
    /// <response code="200">Email verified successfully</response>
    /// <response code="400">Invalid or expired token</response>
    [HttpPost("verify-email")]
    [AllowAnonymous]
    [EnableRateLimiting("sensitive")]
    [ProducesResponseType(typeof(ApiResponse<MessageResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [SwaggerRequestExample(typeof(VerifyEmailRequest), typeof(VerifyEmailRequestExample))]
    public async Task<IActionResult> VerifyEmail(
        [FromBody] VerifyEmailRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Email verification attempt");

        var command = new VerifyEmailCommand { Token = request.Token };
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Verification Failed",
                Detail = result.Error,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
            });
        }

        return Ok(ApiResponse<MessageResponse>.SuccessResult(
            new MessageResponse { Message = "Email verified successfully." },
            "Email verified."));
    }

    #endregion

    #region Password Reset

    /// <summary>
    /// Sends a password reset email to the specified address.
    /// </summary>
    /// <remarks>
    /// Always returns 200 regardless of whether the email exists (prevents email enumeration).
    /// If the account is locked, it will be unlocked after successful password reset.
    /// Rate limit: 5 requests per email per hour.
    /// </remarks>
    /// <param name="request">Email address</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Confirmation message</returns>
    /// <response code="200">Reset email sent (or email doesn't exist — same response for security)</response>
    /// <response code="400">Validation error</response>
    [HttpPost("forgot-password")]
    [AllowAnonymous]
    [EnableRateLimiting("sensitive")]
    [ProducesResponseType(typeof(ApiResponse<MessageResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [SwaggerRequestExample(typeof(ForgotPasswordRequest), typeof(ForgotPasswordRequestExample))]
    public async Task<IActionResult> ForgotPassword(
        [FromBody] ForgotPasswordRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Password reset request for: {Email}", request.Email);

        var command = new ForgotPasswordCommand { Email = request.Email };
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Request Failed",
                Detail = result.Error,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
            });
        }

        return Ok(ApiResponse<MessageResponse>.SuccessResult(
            new MessageResponse { Message = "If an account with that email exists, a password reset link has been sent." },
            "Request processed."));
    }

    /// <summary>
    /// Resets a user's password using the token from the reset email.
    /// </summary>
    /// <remarks>
    /// Tokens expire after 30 minutes. Each token can only be used once.
    /// After reset, all active refresh tokens are revoked (forces re-login on all devices).
    /// Locked accounts are automatically unlocked.
    /// </remarks>
    /// <param name="request">Reset token and new password</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Confirmation message</returns>
    /// <response code="200">Password reset successfully</response>
    /// <response code="400">Invalid/expired token or validation error</response>
    [HttpPost("reset-password")]
    [AllowAnonymous]
    [EnableRateLimiting("sensitive")]
    [ProducesResponseType(typeof(ApiResponse<MessageResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [SwaggerRequestExample(typeof(ResetPasswordRequest), typeof(ResetPasswordRequestExample))]
    public async Task<IActionResult> ResetPassword(
        [FromBody] ResetPasswordRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Password reset attempt");

        var command = new ResetPasswordCommand
        {
            Token = request.Token,
            NewPassword = request.NewPassword,
            ConfirmNewPassword = request.ConfirmNewPassword
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Reset Failed",
                Detail = result.Error,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
            });
        }

        return Ok(ApiResponse<MessageResponse>.SuccessResult(
            new MessageResponse { Message = "Password reset successfully. Please log in with your new password." },
            "Password reset."));
    }

    #endregion
}
