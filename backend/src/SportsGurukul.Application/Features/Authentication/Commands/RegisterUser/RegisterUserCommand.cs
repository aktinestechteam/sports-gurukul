using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.Authentication.DTOs.Responses;

namespace SportsGurukul.Application.Features.Authentication.Commands.RegisterUser;

public class RegisterUserCommand : IRequest<Result<AuthResponse>>
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
}
