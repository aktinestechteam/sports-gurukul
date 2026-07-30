using MediatR;
using SportsGurukul.Platform.FinancialReporting.Models;

namespace SportsGurukul.Platform.FinancialReporting.Queries;

public record ReconciliationQuery(
    ReconciliationType Type,
    DateTime FromDate,
    DateTime ToDate,
    string? Source = null,
    ReportFilter? Filter = null
) : IRequest<ReconciliationResult>;

public class ReconciliationQueryHandler : IRequestHandler<ReconciliationQuery, ReconciliationResult>
{
    private readonly Interfaces.IReconciliationService _reconciliationService;

    public ReconciliationQueryHandler(Interfaces.IReconciliationService reconciliationService)
    {
        _reconciliationService = reconciliationService;
    }

    public async Task<ReconciliationResult> Handle(ReconciliationQuery request, CancellationToken cancellationToken)
    {
        var differences = await _reconciliationService.DetectDifferencesAsync(request.Type, request.Filter, cancellationToken);

        return new ReconciliationResult
        {
            Type = request.Type,
            Status = differences.TotalExceptions == 0 ? ReconciliationStatus.Matched : ReconciliationStatus.Discrepancy,
            PerformedAt = DateTime.UtcNow,
            TotalRecords = differences.TotalExceptions,
            MatchedRecords = 0,
            UnmatchedRecords = differences.TotalExceptions,
            DiscrepancyCount = differences.Exceptions.Count,
            Differences = differences.Exceptions.Select(e => new ReconciliationDifference
            {
                RecordId = e.ExceptionId,
                Description = e.Description,
                ExpectedAmount = e.Amount,
                Difference = e.Amount,
                Status = ReconciliationStatus.Discrepancy
            }).ToList()
        };
    }
}
