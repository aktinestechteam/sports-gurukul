using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AcademyManagement.DTOs;

namespace SportsGurukul.Application.Features.AcademyManagement.Commands.AssignSport;

public class AssignSportCommand : IRequest<Result<AcademySportDto>>
{
    public Guid AcademyId { get; set; }
    public Guid SportId { get; set; }
    public bool IsPrimarySport { get; set; }
}
