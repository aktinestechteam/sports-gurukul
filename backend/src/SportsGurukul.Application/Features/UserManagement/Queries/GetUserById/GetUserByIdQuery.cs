using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.UserManagement.DTOs;

namespace SportsGurukul.Application.Features.UserManagement.Queries.GetUserById;

public class GetUserByIdQuery : IRequest<Result<UserProfileDto>>
{
    public Guid UserId { get; set; }
}
