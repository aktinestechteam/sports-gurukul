using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.Authentication.DTOs.Responses;

namespace SportsGurukul.Application.Features.Authentication.Commands.RefreshToken;

public class RefreshTokenCommand : IRequest<Result<TokenResponse>>
{
    public string RefreshToken { get; set; } = string.Empty;
}
