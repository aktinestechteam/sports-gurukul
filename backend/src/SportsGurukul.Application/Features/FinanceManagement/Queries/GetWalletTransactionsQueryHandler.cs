using MediatR;
using SportsGurukul.Application.Common.Interfaces.Finance.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;

namespace SportsGurukul.Application.Features.FinanceManagement.Queries;

public class GetWalletTransactionsQueryHandler : IRequestHandler<GetWalletTransactionsQuery, Result<IReadOnlyList<WalletTransactionDto>>>
{
    private readonly IWalletService _walletService;

    public GetWalletTransactionsQueryHandler(IWalletService walletService)
    {
        _walletService = walletService;
    }

    public async Task<Result<IReadOnlyList<WalletTransactionDto>>> Handle(GetWalletTransactionsQuery request, CancellationToken cancellationToken)
    {
        return await _walletService.GetTransactionsAsync(request.WalletId, request.Page, request.PageSize, cancellationToken);
    }
}
