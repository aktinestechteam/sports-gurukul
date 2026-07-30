using MediatR;
using SportsGurukul.Application.Common.Interfaces.Finance.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;

namespace SportsGurukul.Application.Features.FinanceManagement.Commands.Wallet;

public class CreateWalletCommandHandler : IRequestHandler<CreateWalletCommand, Result<WalletDto>>
{
    private readonly IWalletService _walletService;

    public CreateWalletCommandHandler(IWalletService walletService)
    {
        _walletService = walletService;
    }

    public async Task<Result<WalletDto>> Handle(CreateWalletCommand request, CancellationToken cancellationToken)
    {
        return await _walletService.CreateWalletAsync(request.UserId, request.Currency, cancellationToken);
    }
}
