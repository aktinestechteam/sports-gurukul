using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AcademyManagement.DTOs;

namespace SportsGurukul.Application.Features.AcademyManagement.Queries.GetRegisteredAthletes;

public class GetRegisteredAthletesQuery : IRequest<Result<IReadOnlyList<AcademyAthleteSummaryDto>>>
{
    public Guid AcademyId { get; set; }
}
