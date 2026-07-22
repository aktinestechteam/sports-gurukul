using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.Authentication.Commands.VerifyEmail;

public class VerifyEmailCommand : IRequest<Result<Unit>>
{
    public string Token { get; set; } = string.Empty;
}
