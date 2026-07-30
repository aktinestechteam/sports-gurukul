using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;

namespace SportsGurukul.Application.Features.FinanceManagement.Commands.Refund;

public record RequestRefundCommand(Guid PaymentId, decimal Amount, string? Reason, List<RefundItemRequest>? Items) : IRequest<Result<RefundDto>>;
