using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.UserManagement.Commands.RestoreUserProfile;

public class RestoreUserProfileCommand : IRequest<Result<Unit>>
{
    public Guid UserId { get; set; }
}
