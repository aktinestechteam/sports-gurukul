using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.Authentication.Commands.ForgotPassword;

public class ForgotPasswordCommand : IRequest<Result<Unit>>
{
    public string Email { get; set; } = string.Empty;
}
