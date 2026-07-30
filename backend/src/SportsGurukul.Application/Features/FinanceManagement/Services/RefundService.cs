using SportsGurukul.Application.Common.Interfaces.Finance;
using SportsGurukul.Application.Common.Interfaces.Finance.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;
using SportsGurukul.Domain.Entities.Finance;
using SportsGurukul.Domain.Enums.Finance;

namespace SportsGurukul.Application.Features.FinanceManagement.Services;

public class RefundService : IRefundService
{
    private readonly IRefundRepository _refundRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly ILedgerService _ledgerService;

    public RefundService(
        IRefundRepository refundRepository,
        IPaymentRepository paymentRepository,
        ILedgerService ledgerService)
    {
        _refundRepository = refundRepository;
        _paymentRepository = paymentRepository;
        _ledgerService = ledgerService;
    }

    public async Task<Result<RefundDto>> RequestRefundAsync(RequestRefundRequest request, CancellationToken cancellationToken)
    {
        var payment = await _paymentRepository.GetByIdAsync(request.PaymentId, cancellationToken);
        if (payment is null)
            return Result<RefundDto>.Failure("Payment not found");

        if (payment.Status != PaymentStatus.Captured)
            return Result<RefundDto>.Failure("Can only refund completed payments");

        if (request.Amount > payment.Amount)
            return Result<RefundDto>.Failure("Refund amount cannot exceed payment amount");

        var refundNumber = await GenerateRefundNumberInternalAsync(cancellationToken);
        var refund = new Refund
        {
            PaymentId = request.PaymentId,
            TotalAmount = request.Amount,
            Reason = request.Reason ?? string.Empty,
            Status = RefundStatus.Requested,
            RefundNumber = refundNumber,
            RefundDate = DateTime.UtcNow,
        };

        if (request.Items is not null)
        {
            foreach (var item in request.Items)
            {
                refund.RefundItems.Add(new RefundItem
                {
                    Description = item.Description ?? string.Empty,
                    Amount = item.Amount,
                    Quantity = 1,
                });
            }
        }

        var created = await _refundRepository.AddAsync(refund, cancellationToken);
        return Result<RefundDto>.Success(MapToDto(created));
    }

    public async Task<Result<RefundDto>> ApproveRefundAsync(Guid refundId, string approvedBy, CancellationToken cancellationToken)
    {
        var refund = await _refundRepository.GetByIdWithItemsAsync(refundId, cancellationToken);
        if (refund is null)
            return Result<RefundDto>.Failure("Refund not found");

        if (refund.Status != RefundStatus.Requested)
            return Result<RefundDto>.Failure("Only requested refunds can be approved");

        refund.Status = RefundStatus.Approved;
        refund.ApprovedBy = approvedBy;
        refund.ApprovedAt = DateTime.UtcNow;
        _refundRepository.Update(refund);

        return Result<RefundDto>.Success(MapToDto(refund));
    }

    public async Task<Result<RefundDto>> RejectRefundAsync(Guid refundId, string reason, CancellationToken cancellationToken)
    {
        var refund = await _refundRepository.GetByIdAsync(refundId, cancellationToken);
        if (refund is null)
            return Result<RefundDto>.Failure("Refund not found");

        if (refund.Status != RefundStatus.Requested)
            return Result<RefundDto>.Failure("Only requested refunds can be rejected");

        refund.Status = RefundStatus.Rejected;
        refund.Notes = reason;
        _refundRepository.Update(refund);

        return Result<RefundDto>.Success(MapToDto(refund));
    }

    public async Task<Result<RefundDto>> CompleteRefundAsync(Guid refundId, string? gatewayReference, CancellationToken cancellationToken)
    {
        var refund = await _refundRepository.GetByIdWithItemsAsync(refundId, cancellationToken);
        if (refund is null)
            return Result<RefundDto>.Failure("Refund not found");

        if (refund.Status != RefundStatus.Approved)
            return Result<RefundDto>.Failure("Only approved refunds can be completed");

        refund.Status = RefundStatus.Completed;
        refund.GatewayReference = gatewayReference;
        _refundRepository.Update(refund);

        await PostRefundLedgerEntries(refund, cancellationToken);

        return Result<RefundDto>.Success(MapToDto(refund));
    }

    public async Task<Result<string>> GenerateRefundNumberAsync(CancellationToken cancellationToken)
    {
        var number = await GenerateRefundNumberInternalAsync(cancellationToken);
        return Result<string>.Success(number);
    }

    public async Task<Result<IReadOnlyList<RefundDto>>> GetRefundHistoryAsync(Guid paymentId, CancellationToken cancellationToken)
    {
        var refunds = await _refundRepository.GetByPaymentIdAsync(paymentId, cancellationToken);
        return Result<IReadOnlyList<RefundDto>>.Success(refunds.Select(MapToDto).ToList());
    }

    private async Task<string> GenerateRefundNumberInternalAsync(CancellationToken cancellationToken)
    {
        var count = await _refundRepository.CountAsync(cancellationToken: cancellationToken);
        return $"RFN-{DateTime.UtcNow:yyyyMMdd}-{(count + 1):D5}";
    }

    private async Task PostRefundLedgerEntries(Refund refund, CancellationToken cancellationToken)
    {
        var cashLedger = await _ledgerService.GetOrCreateLedgerAsync("CASH", "Cash", LedgerType.Asset, "Cash & Bank", cancellationToken);
        var refundLiability = await _ledgerService.GetOrCreateLedgerAsync("REF", "Refund Payable", LedgerType.Liability, "Refunds", cancellationToken);

        if (cashLedger.IsSuccess && refundLiability.IsSuccess)
        {
            await _ledgerService.PostLedgerEntryAsync(refundLiability.Value!, new LedgerEntry
            {
                DebitAmount = refund.TotalAmount,
                CreditAmount = 0,
                Description = $"Refund {refund.RefundNumber}",
                Reference = refund.Id.ToString(),
                EntryDate = DateTime.UtcNow,
            }, cancellationToken);

            await _ledgerService.PostLedgerEntryAsync(cashLedger.Value!, new LedgerEntry
            {
                DebitAmount = 0,
                CreditAmount = refund.TotalAmount,
                Description = $"Refund {refund.RefundNumber}",
                Reference = refund.Id.ToString(),
                EntryDate = DateTime.UtcNow,
            }, cancellationToken);
        }
    }

    private static RefundDto MapToDto(Refund refund)
    {
        return new RefundDto(
            refund.Id,
            refund.PaymentId,
            refund.RefundNumber,
            refund.TotalAmount,
            refund.Reason,
            refund.Status,
            refund.ApprovedBy,
            refund.Notes,
            refund.GatewayReference,
            refund.ApprovedAt,
            refund.CreatedAt
        );
    }
}
