using SportsGurukul.Domain.Entities.Finance;

namespace SportsGurukul.Application.Common.Interfaces.Finance;

public interface IRefundRepository : IRepository<Refund>
{
    Task<Refund?> GetByRefundNumberAsync(string refundNumber, CancellationToken cancellationToken = default);
    Task<Refund?> GetByIdWithItemsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Refund>> GetByPaymentIdAsync(Guid paymentId, CancellationToken cancellationToken = default);
}
