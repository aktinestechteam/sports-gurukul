using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.UserManagement.DTOs;

namespace SportsGurukul.Application.Features.UserManagement.Commands.UploadProfilePhoto;

public class UploadProfilePhotoCommand : IRequest<Result<ProfilePhotoResponse>>
{
    public Guid UserId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public byte[] FileContent { get; set; } = Array.Empty<byte>();
}
