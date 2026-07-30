using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.FinanceManagement.Queries;

public record ValidateCouponQuery(string Code, string? UserId, decimal OrderAmount) : IRequest<Result<bool>>;
