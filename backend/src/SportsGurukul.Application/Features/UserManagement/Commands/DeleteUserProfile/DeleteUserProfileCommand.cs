using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.UserManagement.Commands.DeleteUserProfile;

public class DeleteUserProfileCommand : IRequest<Result<Unit>>
{
    public Guid UserId { get; set; }
}
