using MediatR;
using SportsGurukul.Application.Common.Interfaces.Finance.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;

namespace SportsGurukul.Application.Features.FinanceManagement.Queries;

public class GetWalletByUserIdQueryHandler : IRequestHandler<GetWalletByUserIdQuery, Result<WalletDto>>
{
    private readonly IWalletService _walletService;

    public GetWalletByUserIdQueryHandler(IWalletService walletService)
    {
        _walletService = walletService;
    }

    public async Task<Result<WalletDto>> Handle(GetWalletByUserIdQuery request, CancellationToken cancellationToken)
    {
        return await _walletService.GetByUserIdAsync(request.UserId, cancellationToken);
    }
}
