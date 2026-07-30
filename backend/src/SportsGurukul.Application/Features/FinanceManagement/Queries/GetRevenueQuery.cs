using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.FinanceManagement.Queries;

public record GetRevenueQuery(DateTime? FromDate, DateTime? ToDate) : IRequest<Result<RevenueDto>>;

public record RevenueDto(
    decimal TotalRevenue,
    decimal TotalTax,
    decimal TotalDiscount,
    decimal NetRevenue,
    int InvoiceCount,
    int PaymentCount
);
