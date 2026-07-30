using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;

namespace SportsGurukul.Application.Features.FinanceManagement.Commands.Wallet;

public record CreditWalletCommand(Guid WalletId, decimal Amount, string? Reference, string? Description) : IRequest<Result<WalletDto>>;
