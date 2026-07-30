using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.FinanceManagement.Queries;

public class GetPaymentStatisticsQueryHandler : IRequestHandler<GetPaymentStatisticsQuery, Result<PaymentStatisticsDto>>
{
    public async Task<Result<PaymentStatisticsDto>> Handle(GetPaymentStatisticsQuery request, CancellationToken cancellationToken)
    {
        // Placeholder: would aggregate from payment repository
        var dto = new PaymentStatisticsDto(0, 0, 0, 0, 0, 0, 0, 0);
        return Result<PaymentStatisticsDto>.Success(dto);
    }
}
