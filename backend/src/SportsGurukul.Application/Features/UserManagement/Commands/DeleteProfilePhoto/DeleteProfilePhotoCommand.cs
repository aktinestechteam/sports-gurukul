using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.UserManagement.Commands.DeleteProfilePhoto;

public class DeleteProfilePhotoCommand : IRequest<Result<Unit>>
{
    public Guid UserId { get; set; }
}
