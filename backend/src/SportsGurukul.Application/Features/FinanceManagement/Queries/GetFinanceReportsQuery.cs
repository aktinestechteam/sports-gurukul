using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.FinanceManagement.Queries;

public record GetFinanceReportsQuery(DateTime? FromDate, DateTime? ToDate) : IRequest<Result<FinanceReportDto>>;

public record FinanceReportDto(
    decimal TotalRevenue,
    decimal TotalExpenses,
    decimal NetIncome,
    decimal TotalTaxCollected,
    decimal TotalDiscountGiven,
    int TotalInvoicesIssued,
    int TotalPaymentsReceived,
    int TotalRefundsProcessed,
    decimal OutstandingReceivables,
    Dictionary<string, decimal> RevenueByCategory
);
