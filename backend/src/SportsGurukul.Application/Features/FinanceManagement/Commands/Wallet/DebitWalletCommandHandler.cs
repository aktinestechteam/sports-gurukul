using MediatR;
using SportsGurukul.Application.Common.Interfaces.Finance.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;

namespace SportsGurukul.Application.Features.FinanceManagement.Commands.Wallet;

public class DebitWalletCommandHandler : IRequestHandler<DebitWalletCommand, Result<WalletDto>>
{
    private readonly IWalletService _walletService;

    public DebitWalletCommandHandler(IWalletService walletService)
    {
        _walletService = walletService;
    }

    public async Task<Result<WalletDto>> Handle(DebitWalletCommand request, CancellationToken cancellationToken)
    {
        return await _walletService.DebitWalletAsync(request.WalletId, request.Amount, request.Reference, request.Description, cancellationToken);
    }
}
