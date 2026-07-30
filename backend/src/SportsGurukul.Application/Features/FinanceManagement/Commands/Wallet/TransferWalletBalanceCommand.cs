using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;

namespace SportsGurukul.Application.Features.FinanceManagement.Commands.Wallet;

public record TransferWalletBalanceCommand(Guid FromWalletId, Guid ToWalletId, decimal Amount, string? Description) : IRequest<Result<WalletDto>>;
