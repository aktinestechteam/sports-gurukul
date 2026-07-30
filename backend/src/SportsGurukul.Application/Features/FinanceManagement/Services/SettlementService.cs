using SportsGurukul.Application.Common.Interfaces.Finance;
using SportsGurukul.Application.Common.Interfaces.Finance.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;
using SportsGurukul.Domain.Entities.Finance;

namespace SportsGurukul.Application.Features.FinanceManagement.Services;

public class SettlementService : ISettlementService
{
    private readonly ISettlementRepository _settlementRepository;
    private readonly IPaymentRepository _paymentRepository;

    public SettlementService(ISettlementRepository settlementRepository, IPaymentRepository paymentRepository)
    {
        _settlementRepository = settlementRepository;
        _paymentRepository = paymentRepository;
    }

    public async Task<Result<SettlementDto>> CreateSettlementBatchAsync(Guid[] paymentIds, CancellationToken cancellationToken)
    {
        var batchNumber = await GenerateBatchNumberInternalAsync(cancellationToken);
        var batch = new SettlementBatch
        {
            BatchNumber = batchNumber,
            Status = Domain.Enums.Finance.SettlementStatus.Pending,
        };

        decimal totalAmount = 0;
        foreach (var paymentId in paymentIds)
        {
            var payment = await _paymentRepository.GetByIdAsync(paymentId, cancellationToken);
            if (payment is null)
                return Result<SettlementDto>.Failure($"Payment not found: {paymentId}");

            batch.Settlements.Add(new Settlement
            {
                PaymentId = paymentId,
                Amount = payment.Amount,
                Status = Domain.Enums.Finance.SettlementStatus.Pending,
            });
            totalAmount += payment.Amount;
        }

        batch.TotalAmount = totalAmount;

        var created = await _settlementRepository.AddAsync(batch, cancellationToken);
        return Result<SettlementDto>.Success(MapToDto(created));
    }

    public async Task<Result<SettlementDto>> ApproveSettlementAsync(Guid batchId, CancellationToken cancellationToken)
    {
        var batch = await _settlementRepository.GetByIdWithSettlementsAsync(batchId, cancellationToken);
        if (batch is null)
            return Result<SettlementDto>.Failure("Settlement batch not found");

        if (batch.Status != Domain.Enums.Finance.SettlementStatus.Pending)
            return Result<SettlementDto>.Failure("Only pending batches can be approved");

        batch.Status = Domain.Enums.Finance.SettlementStatus.InProgress;

        foreach (var settlement in batch.Settlements)
            settlement.Status = Domain.Enums.Finance.SettlementStatus.InProgress;

        _settlementRepository.Update(batch);
        return Result<SettlementDto>.Success(MapToDto(batch));
    }

    public async Task<Result<SettlementDto>> CompleteSettlementAsync(Guid batchId, string? reference, CancellationToken cancellationToken)
    {
        var batch = await _settlementRepository.GetByIdWithSettlementsAsync(batchId, cancellationToken);
        if (batch is null)
            return Result<SettlementDto>.Failure("Settlement batch not found");

        if (batch.Status != Domain.Enums.Finance.SettlementStatus.InProgress)
            return Result<SettlementDto>.Failure("Only in-progress batches can be completed");

        batch.Status = Domain.Enums.Finance.SettlementStatus.Completed;
        batch.SettledAt = DateTime.UtcNow;

        foreach (var settlement in batch.Settlements)
        {
            settlement.Status = Domain.Enums.Finance.SettlementStatus.Completed;
            settlement.Reference = reference;
            settlement.SettledAt = DateTime.UtcNow;
        }

        _settlementRepository.Update(batch);
        return Result<SettlementDto>.Success(MapToDto(batch));
    }

    public async Task<Result<SettlementDto>> GetByIdAsync(Guid batchId, CancellationToken cancellationToken)
    {
        var batch = await _settlementRepository.GetByIdWithSettlementsAsync(batchId, cancellationToken);
        if (batch is null)
            return Result<SettlementDto>.Failure("Settlement batch not found");

        return Result<SettlementDto>.Success(MapToDto(batch));
    }

    public async Task<Result<string>> GenerateBatchNumberAsync(CancellationToken cancellationToken)
    {
        var number = await GenerateBatchNumberInternalAsync(cancellationToken);
        return Result<string>.Success(number);
    }

    private async Task<string> GenerateBatchNumberInternalAsync(CancellationToken cancellationToken)
    {
        var count = await _settlementRepository.CountAsync(cancellationToken: cancellationToken);
        return $"STL-{DateTime.UtcNow:yyyyMMdd}-{(count + 1):D5}";
    }

    private static SettlementDto MapToDto(SettlementBatch batch)
    {
        return new SettlementDto(
            batch.Id,
            batch.BatchNumber,
            batch.TotalAmount,
            batch.Settlements.Count,
            batch.Status,
            null,
            batch.SettledAt,
            batch.CreatedAt,
            batch.Settlements.Select(s => new SettlementItemDto(
                s.Id, s.PaymentId, string.Empty, s.Amount, s.Status
            )).ToList()
        );
    }
}
