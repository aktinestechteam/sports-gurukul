using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;

namespace SportsGurukul.Application.Common.Interfaces.Finance.Services;

public interface ISettlementService
{
    Task<Result<SettlementDto>> CreateSettlementBatchAsync(Guid[] paymentIds, CancellationToken cancellationToken = default);
    Task<Result<SettlementDto>> ApproveSettlementAsync(Guid batchId, CancellationToken cancellationToken = default);
    Task<Result<SettlementDto>> CompleteSettlementAsync(Guid batchId, string? reference = null, CancellationToken cancellationToken = default);
    Task<Result<SettlementDto>> GetByIdAsync(Guid batchId, CancellationToken cancellationToken = default);
    Task<Result<string>> GenerateBatchNumberAsync(CancellationToken cancellationToken = default);
}
