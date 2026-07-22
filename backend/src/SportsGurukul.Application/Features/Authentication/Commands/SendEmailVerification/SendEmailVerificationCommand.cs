using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.Authentication.Commands.SendEmailVerification;

public class SendEmailVerificationCommand : IRequest<Result<Unit>>
{
    public string Email { get; set; } = string.Empty;
}
