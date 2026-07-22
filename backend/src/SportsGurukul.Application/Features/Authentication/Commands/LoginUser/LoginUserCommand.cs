using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.Authentication.DTOs.Responses;

namespace SportsGurukul.Application.Features.Authentication.Commands.LoginUser;

public class LoginUserCommand : IRequest<Result<LoginResponse>>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
