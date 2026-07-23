using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.UserManagement.DTOs;

namespace SportsGurukul.Application.Features.UserManagement.Queries.GetProfilePhoto;

public class GetProfilePhotoQuery : IRequest<Result<ProfilePhotoResponse>>
{
    public Guid UserId { get; set; }
}
