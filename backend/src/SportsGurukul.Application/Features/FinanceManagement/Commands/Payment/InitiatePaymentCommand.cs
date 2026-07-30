using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;
using SportsGurukul.Domain.Enums.Finance;

namespace SportsGurukul.Application.Features.FinanceManagement.Commands.Payment;

public record InitiatePaymentCommand(Guid InvoiceId, decimal Amount, PaymentMethod PaymentMethod, string? IdempotencyKey, string? Description) : IRequest<Result<PaymentDto>>;
