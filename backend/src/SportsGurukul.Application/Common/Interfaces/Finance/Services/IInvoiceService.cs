using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;

namespace SportsGurukul.Application.Common.Interfaces.Finance.Services;

public interface IInvoiceService
{
    Task<Result<InvoiceDto>> CreateInvoiceAsync(CreateInvoiceRequest request, CancellationToken cancellationToken = default);
    Task<Result<InvoiceDto>> UpdateInvoiceAsync(Guid invoiceId, UpdateInvoiceRequest request, CancellationToken cancellationToken = default);
    Task<Result<InvoiceDto>> IssueInvoiceAsync(Guid invoiceId, CancellationToken cancellationToken = default);
    Task<Result<InvoiceDto>> CancelInvoiceAsync(Guid invoiceId, string reason, CancellationToken cancellationToken = default);
    Task<Result<InvoiceDto>> MarkAsPaidAsync(Guid invoiceId, CancellationToken cancellationToken = default);
    Task<Result<InvoiceDto>> VoidInvoiceAsync(Guid invoiceId, string reason, CancellationToken cancellationToken = default);
    Task<Result<string>> GenerateInvoiceNumberAsync(CancellationToken cancellationToken = default);
    Task<Result<InvoiceDto>> GetByIdAsync(Guid invoiceId, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<InvoiceSummaryDto>>> SearchInvoicesAsync(InvoiceSearchRequest request, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<InvoiceSummaryDto>>> GetOutstandingInvoicesAsync(CancellationToken cancellationToken = default);
}
