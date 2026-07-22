using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.UserManagement.DTOs;

namespace SportsGurukul.Application.Features.UserManagement.Queries.GetCurrentUser;

public class GetCurrentUserQuery : IRequest<Result<UserProfileDto>>
{
}
