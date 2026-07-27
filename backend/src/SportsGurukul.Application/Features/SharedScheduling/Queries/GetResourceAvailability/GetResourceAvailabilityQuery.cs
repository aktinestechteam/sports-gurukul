using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.SharedScheduling.Models;

namespace SportsGurukul.Application.Features.SharedScheduling.Queries.GetResourceAvailability;

public class GetResourceAvailabilityQuery : IRequest<Result<AvailabilityWindow>>
{
    public Guid ResourceId { get; set; }
    public string ResourceType { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public Guid AcademyId { get; set; }
    public Guid? BranchId { get; set; }
    public string? TimeZoneId { get; set; }
}
