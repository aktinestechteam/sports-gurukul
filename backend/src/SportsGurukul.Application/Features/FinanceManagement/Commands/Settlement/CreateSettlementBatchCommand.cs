using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;

namespace SportsGurukul.Application.Features.FinanceManagement.Commands.Settlement;

public record CreateSettlementBatchCommand(Guid[] PaymentIds) : IRequest<Result<SettlementDto>>;
