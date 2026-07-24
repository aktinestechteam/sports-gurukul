using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.DTOs;

namespace SportsGurukul.Application.Features.CoachManagement.Commands.AssignSport;

public class AssignSportCommand : IRequest<Result<SportDto>>
{
    public Guid CoachId { get; set; }
    public Guid SportId { get; set; }
    public bool IsPrimarySport { get; set; }
}
