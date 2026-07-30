using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;

namespace SportsGurukul.Application.Features.FinanceManagement.Queries;

public record GetOutstandingInvoicesQuery : IRequest<Result<IReadOnlyList<InvoiceSummaryDto>>>;
