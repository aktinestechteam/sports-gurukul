using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;
using SportsGurukul.Domain.Enums.Finance;

namespace SportsGurukul.Application.Features.FinanceManagement.Commands.Payment;

public record RecordOfflinePaymentCommand(Guid InvoiceId, decimal Amount, PaymentMethod PaymentMethod, string? Reference, DateTime PaidAt, string? Description) : IRequest<Result<PaymentDto>>;
