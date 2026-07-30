using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.FinanceManagement.Queries;

public record GetFinanceDashboardQuery : IRequest<Result<FinanceDashboardDto>>;

public record FinanceDashboardDto(
    decimal TotalRevenue,
    decimal OutstandingAmount,
    int PendingInvoices,
    int OverdueInvoices,
    int RecentPayments,
    decimal WalletBalance,
    int ActiveCoupons,
    int PendingRefunds
);
