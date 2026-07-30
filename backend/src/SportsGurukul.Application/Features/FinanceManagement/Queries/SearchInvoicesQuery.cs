using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;
using SportsGurukul.Domain.Enums.Finance;

namespace SportsGurukul.Application.Features.FinanceManagement.Queries;

public record SearchInvoicesQuery(string? SearchTerm, InvoiceStatus? Status, Guid? AthleteId, Guid? AcademyId, DateTime? FromDate, DateTime? ToDate, int Page = 1, int PageSize = 20) : IRequest<Result<IReadOnlyList<InvoiceSummaryDto>>>;
