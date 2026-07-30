using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Enums.Finance;

namespace SportsGurukul.Application.Features.FinanceManagement.Queries;

public record PaymentSearchQuery(string? SearchTerm, PaymentStatus? Status, Guid? InvoiceId, DateTime? FromDate, DateTime? ToDate, int Page, int PageSize) : IRequest<Result<IReadOnlyList<PaymentSearchHit>>>;
