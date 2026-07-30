using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;

namespace SportsGurukul.Application.Common.Interfaces.Finance.Services;

public interface IPaymentService
{
    Task<Result<PaymentDto>> InitiatePaymentAsync(InitiatePaymentRequest request, CancellationToken cancellationToken = default);
    Task<Result<PaymentDto>> AuthorizePaymentAsync(Guid paymentId, CancellationToken cancellationToken = default);
    Task<Result<PaymentDto>> CapturePaymentAsync(Guid paymentId, CancellationToken cancellationToken = default);
    Task<Result<PaymentDto>> RecordOfflinePaymentAsync(RecordOfflinePaymentRequest request, CancellationToken cancellationToken = default);
    Task<Result<PaymentDto>> CancelPaymentAsync(Guid paymentId, string reason, CancellationToken cancellationToken = default);
    Task<Result<PaymentDto>> RetryPaymentAsync(Guid paymentId, CancellationToken cancellationToken = default);
    Task<Result<string>> GeneratePaymentReferenceAsync(CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<PaymentDto>>> GetPaymentHistoryAsync(Guid invoiceId, CancellationToken cancellationToken = default);
}
