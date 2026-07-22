using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.Authentication.Commands.ResetPassword;

public class ResetPasswordCommand : IRequest<Result<Unit>>
{
    public string Token { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmNewPassword { get; set; } = string.Empty;
}
