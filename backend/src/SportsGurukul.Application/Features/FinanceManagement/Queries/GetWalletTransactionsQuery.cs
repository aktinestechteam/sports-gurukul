using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;

namespace SportsGurukul.Application.Features.FinanceManagement.Queries;

public record GetWalletTransactionsQuery(Guid WalletId, int Page = 1, int PageSize = 20) : IRequest<Result<IReadOnlyList<WalletTransactionDto>>>;
