using MediatR;

namespace SportsGurukul.Application.Features.Authentication.Commands.Logout;

public class LogoutCommand : IRequest<Unit>
{
    public Guid UserId { get; set; }
}
