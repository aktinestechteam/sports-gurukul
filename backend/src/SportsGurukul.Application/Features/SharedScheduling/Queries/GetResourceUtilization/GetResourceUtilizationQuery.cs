using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.SharedScheduling.Models;

namespace SportsGurukul.Application.Features.SharedScheduling.Queries.GetResourceUtilization;

public class GetResourceUtilizationQuery : IRequest<Result<IReadOnlyList<UtilizationMetric>>>
{
    public string ResourceType { get; set; } = string.Empty;
    public IReadOnlyList<Guid> ResourceIds { get; set; } = [];
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Guid AcademyId { get; set; }
    public Guid? BranchId { get; set; }
    public string? TimeZoneId { get; set; }
}
