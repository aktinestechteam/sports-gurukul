using SportsGurukul.Domain.Entities.Finance;

namespace SportsGurukul.Application.Common.Interfaces.Finance;

public interface IPaymentRepository : IRepository<Payment>
{
    Task<Payment?> GetByPaymentReferenceAsync(string paymentReference, CancellationToken cancellationToken = default);
    Task<Payment?> GetByIdWithTransactionsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Payment>> GetByInvoiceIdAsync(Guid invoiceId, CancellationToken cancellationToken = default);
    Task<Payment?> GetByIdempotencyKeyAsync(string idempotencyKey, CancellationToken cancellationToken = default);
}
