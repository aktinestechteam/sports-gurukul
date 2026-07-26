using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FacilityManagement.DTOs;

namespace SportsGurukul.Application.Features.FacilityManagement.Queries.GetFacilitySchedule;

public class GetFacilityScheduleQuery : IRequest<Result<IReadOnlyList<ScheduleDto>>>
{
    public Guid FacilityId { get; set; }
}
