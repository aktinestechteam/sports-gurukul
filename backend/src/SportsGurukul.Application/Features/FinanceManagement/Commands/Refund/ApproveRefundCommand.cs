using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;

namespace SportsGurukul.Application.Features.FinanceManagement.Commands.Refund;

public record ApproveRefundCommand(Guid RefundId, string ApprovedBy) : IRequest<Result<RefundDto>>;
