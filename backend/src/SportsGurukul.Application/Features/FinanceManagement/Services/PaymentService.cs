using SportsGurukul.Application.Common.Interfaces.Finance;
using SportsGurukul.Application.Common.Interfaces.Finance.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;
using SportsGurukul.Domain.Entities.Finance;
using SportsGurukul.Domain.Enums.Finance;

namespace SportsGurukul.Application.Features.FinanceManagement.Services;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly ILedgerService _ledgerService;

    public PaymentService(
        IPaymentRepository paymentRepository,
        IInvoiceRepository invoiceRepository,
        ILedgerService ledgerService)
    {
        _paymentRepository = paymentRepository;
        _invoiceRepository = invoiceRepository;
        _ledgerService = ledgerService;
    }

    public async Task<Result<PaymentDto>> InitiatePaymentAsync(InitiatePaymentRequest request, CancellationToken cancellationToken)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(request.InvoiceId, cancellationToken);
        if (invoice is null)
            return Result<PaymentDto>.Failure("Invoice not found");

        if (!string.IsNullOrEmpty(request.IdempotencyKey))
        {
            var existing = await _paymentRepository.GetByIdempotencyKeyAsync(request.IdempotencyKey, cancellationToken);
            if (existing is not null)
                return Result<PaymentDto>.Success(MapToDto(existing));
        }

        var payRef = await GeneratePaymentReferenceInternalAsync(cancellationToken);
        var payment = new Payment
        {
            InvoiceId = request.InvoiceId,
            Amount = request.Amount,
            PaymentMethod = request.PaymentMethod,
            Status = PaymentStatus.Pending,
            IdempotencyKey = request.IdempotencyKey,
            IsIdempotent = !string.IsNullOrEmpty(request.IdempotencyKey),
            PaymentReference = payRef,
            PaymentDate = DateTime.UtcNow,
        };

        var created = await _paymentRepository.AddAsync(payment, cancellationToken);
        return Result<PaymentDto>.Success(MapToDto(created));
    }

    public async Task<Result<PaymentDto>> AuthorizePaymentAsync(Guid paymentId, CancellationToken cancellationToken)
    {
        var payment = await _paymentRepository.GetByIdAsync(paymentId, cancellationToken);
        if (payment is null)
            return Result<PaymentDto>.Failure("Payment not found");

        if (payment.Status != PaymentStatus.Pending)
            return Result<PaymentDto>.Failure("Only pending payments can be authorized");

        payment.Status = PaymentStatus.Authorized;
        _paymentRepository.Update(payment);

        return Result<PaymentDto>.Success(MapToDto(payment));
    }

    public async Task<Result<PaymentDto>> CapturePaymentAsync(Guid paymentId, CancellationToken cancellationToken)
    {
        var payment = await _paymentRepository.GetByIdWithTransactionsAsync(paymentId, cancellationToken);
        if (payment is null)
            return Result<PaymentDto>.Failure("Payment not found");

        if (payment.Status != PaymentStatus.Authorized)
            return Result<PaymentDto>.Failure("Only authorized payments can be captured");

        payment.Status = PaymentStatus.Captured;

        _paymentRepository.Update(payment);

        await UpdateInvoiceAfterPayment(payment.InvoiceId!.Value, payment.Amount, cancellationToken);

        await PostPaymentLedgerEntries(payment, cancellationToken);

        return Result<PaymentDto>.Success(MapToDto(payment));
    }

    public async Task<Result<PaymentDto>> RecordOfflinePaymentAsync(RecordOfflinePaymentRequest request, CancellationToken cancellationToken)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(request.InvoiceId, cancellationToken);
        if (invoice is null)
            return Result<PaymentDto>.Failure("Invoice not found");

        var payRef = await GeneratePaymentReferenceInternalAsync(cancellationToken);
        var payment = new Payment
        {
            InvoiceId = request.InvoiceId,
            Amount = request.Amount,
            PaymentMethod = request.PaymentMethod,
            Status = PaymentStatus.Captured,
            PaymentReference = payRef,
            PaymentDate = request.PaidAt,
            Description = request.Description,
        };

        var created = await _paymentRepository.AddAsync(payment, cancellationToken);

        await UpdateInvoiceAfterPayment(request.InvoiceId, request.Amount, cancellationToken);
        await PostPaymentLedgerEntries(payment, cancellationToken);

        return Result<PaymentDto>.Success(MapToDto(created));
    }

    public async Task<Result<PaymentDto>> CancelPaymentAsync(Guid paymentId, string reason, CancellationToken cancellationToken)
    {
        var payment = await _paymentRepository.GetByIdAsync(paymentId, cancellationToken);
        if (payment is null)
            return Result<PaymentDto>.Failure("Payment not found");

        if (payment.Status == PaymentStatus.Captured)
            return Result<PaymentDto>.Failure("Cannot cancel a completed payment");

        payment.Status = PaymentStatus.Failed;
        payment.FailureReason = reason;
        _paymentRepository.Update(payment);

        return Result<PaymentDto>.Success(MapToDto(payment));
    }

    public async Task<Result<PaymentDto>> RetryPaymentAsync(Guid paymentId, CancellationToken cancellationToken)
    {
        var payment = await _paymentRepository.GetByIdAsync(paymentId, cancellationToken);
        if (payment is null)
            return Result<PaymentDto>.Failure("Payment not found");

        if (payment.Status != PaymentStatus.Failed)
            return Result<PaymentDto>.Failure("Only failed payments can be retried");

        payment.Status = PaymentStatus.Pending;
        payment.FailureReason = null;
        _paymentRepository.Update(payment);

        return Result<PaymentDto>.Success(MapToDto(payment));
    }

    public async Task<Result<string>> GeneratePaymentReferenceAsync(CancellationToken cancellationToken)
    {
        var reference = await GeneratePaymentReferenceInternalAsync(cancellationToken);
        return Result<string>.Success(reference);
    }

    public async Task<Result<IReadOnlyList<PaymentDto>>> GetPaymentHistoryAsync(Guid invoiceId, CancellationToken cancellationToken)
    {
        var payments = await _paymentRepository.GetByInvoiceIdAsync(invoiceId, cancellationToken);
        return Result<IReadOnlyList<PaymentDto>>.Success(payments.Select(MapToDto).ToList());
    }

    private async Task<string> GeneratePaymentReferenceInternalAsync(CancellationToken cancellationToken)
    {
        var count = await _paymentRepository.CountAsync(cancellationToken: cancellationToken);
        return $"PAY-{DateTime.UtcNow:yyyyMMdd}-{(count + 1):D5}";
    }

    private async Task UpdateInvoiceAfterPayment(Guid invoiceId, decimal amount, CancellationToken cancellationToken)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(invoiceId, cancellationToken);
        if (invoice is null) return;

        invoice.AmountPaid += amount;
        invoice.AmountDue = invoice.Total - invoice.AmountPaid;

        if (invoice.AmountDue <= 0)
            invoice.Status = InvoiceStatus.Paid;

        _invoiceRepository.Update(invoice);
    }

    private async Task PostPaymentLedgerEntries(Payment payment, CancellationToken cancellationToken)
    {
        var cashLedger = await _ledgerService.GetOrCreateLedgerAsync("CASH", "Cash", LedgerType.Asset, "Cash & Bank", cancellationToken);
        var receivableLedger = await _ledgerService.GetOrCreateLedgerAsync("AR", "Accounts Receivable", LedgerType.Asset, "Accounts Receivable", cancellationToken);

        if (cashLedger.IsSuccess && receivableLedger.IsSuccess)
        {
            await _ledgerService.PostLedgerEntryAsync(cashLedger.Value!, new LedgerEntry
            {
                DebitAmount = payment.Amount,
                CreditAmount = 0,
                Description = $"Payment {payment.PaymentReference}",
                Reference = payment.Id.ToString(),
                EntryDate = DateTime.UtcNow,
            }, cancellationToken);

            await _ledgerService.PostLedgerEntryAsync(receivableLedger.Value!, new LedgerEntry
            {
                DebitAmount = 0,
                CreditAmount = payment.Amount,
                Description = $"Payment against invoice - {payment.PaymentReference}",
                Reference = payment.Id.ToString(),
                EntryDate = DateTime.UtcNow,
            }, cancellationToken);
        }
    }

    private static PaymentDto MapToDto(Payment payment)
    {
        return new PaymentDto(
            payment.Id,
            payment.InvoiceId ?? Guid.Empty,
            payment.PaymentReference,
            payment.Amount,
            null,
            null,
            payment.Amount,
            payment.PaymentMethod,
            payment.Status,
            payment.IdempotencyKey,
            payment.GatewayTransactionId,
            payment.FailureReason,
            payment.PaymentDate,
            null,
            payment.CreatedAt
        );
    }
}
