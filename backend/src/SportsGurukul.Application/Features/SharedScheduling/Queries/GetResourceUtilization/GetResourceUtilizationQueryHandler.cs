using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.SharedScheduling.Engine;
using SportsGurukul.Application.Features.SharedScheduling.Models;

namespace SportsGurukul.Application.Features.SharedScheduling.Queries.GetResourceUtilization;

public class GetResourceUtilizationQueryHandler
    : IRequestHandler<GetResourceUtilizationQuery, Result<IReadOnlyList<UtilizationMetric>>>
{
    private readonly IAvailabilityEngine _availabilityEngine;
    private readonly ILogger<GetResourceUtilizationQueryHandler> _logger;

    public GetResourceUtilizationQueryHandler(
        IAvailabilityEngine availabilityEngine,
        ILogger<GetResourceUtilizationQueryHandler> logger)
    {
        _availabilityEngine = availabilityEngine;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<UtilizationMetric>>> Handle(
        GetResourceUtilizationQuery request, CancellationToken cancellationToken)
    {
        var context = new SchedulingContext
        {
            AcademyId = request.AcademyId,
            BranchId = request.BranchId,
            TimeZoneId = request.TimeZoneId ?? "UTC"
        };

        var metrics = await _availabilityEngine.GetResourceUtilizationAsync(
            request.ResourceType, request.ResourceIds, request.StartDate,
            request.EndDate, context, cancellationToken);

        _logger.LogInformation(
            "Retrieved utilization metrics for {Count} {ResourceType} resources",
            metrics.Count, request.ResourceType);

        return Result<IReadOnlyList<UtilizationMetric>>.Success(metrics);
    }
}
