using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.FinanceManagement.Queries;

public record GetPaymentStatisticsQuery(DateTime? FromDate, DateTime? ToDate) : IRequest<Result<PaymentStatisticsDto>>;

public record PaymentStatisticsDto(
    int TotalTransactions,
    decimal TotalAmount,
    decimal SuccessfulAmount,
    decimal FailedAmount,
    decimal RefundedAmount,
    int SuccessfulCount,
    int FailedCount,
    int RefundedCount
);
