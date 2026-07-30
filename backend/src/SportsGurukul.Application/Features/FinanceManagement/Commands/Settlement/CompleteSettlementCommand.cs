using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;

namespace SportsGurukul.Application.Features.FinanceManagement.Commands.Settlement;

public record CompleteSettlementCommand(Guid BatchId, string? Reference) : IRequest<Result<SettlementDto>>;
