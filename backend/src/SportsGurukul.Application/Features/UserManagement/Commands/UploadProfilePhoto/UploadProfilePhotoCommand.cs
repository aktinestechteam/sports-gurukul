using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.UserManagement.Commands.UploadProfilePhoto;

public class UploadProfilePhotoCommand : IRequest<Result<string>>
{
    public Guid UserId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
}
