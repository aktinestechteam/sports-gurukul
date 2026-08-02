using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.Interfaces.HumanInTheLoop;

public interface IApprovalStore
{
    Task<ApprovalRequest> AddAsync(ApprovalRequest request, CancellationToken cancellationToken = default);

    Task<ApprovalRequest?> GetAsync(Guid requestId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ApprovalRequest>> ListAsync(string? tenantId = null, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ApprovalRequest>> GetPendingAsync(string? tenantId = null, CancellationToken cancellationToken = default);

    Task UpdateAsync(ApprovalRequest request, CancellationToken cancellationToken = default);

    Task<ApprovalRequest> WaitAsync(Guid requestId, TimeSpan? timeout, CancellationToken cancellationToken = default);

    void Signal(Guid requestId);
}
