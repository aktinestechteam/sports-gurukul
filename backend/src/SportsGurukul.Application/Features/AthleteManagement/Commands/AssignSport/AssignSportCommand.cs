using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AthleteManagement.DTOs;

namespace SportsGurukul.Application.Features.AthleteManagement.Commands.AssignSport;

public class AssignSportCommand : IRequest<Result<SportDto>>
{
    public Guid AthleteId { get; set; }
    public Guid SportId { get; set; }
    public bool IsPrimarySport { get; set; }
}
