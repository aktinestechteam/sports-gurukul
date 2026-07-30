using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;

namespace SportsGurukul.Application.Features.FinanceManagement.Commands.Refund;

public record RejectRefundCommand(Guid RefundId, string Reason) : IRequest<Result<RefundDto>>;
