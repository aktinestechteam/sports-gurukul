using MediatR;
using SportsGurukul.Application.Common.Interfaces.Finance.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;

namespace SportsGurukul.Application.Features.FinanceManagement.Queries;

public class GetSettlementByIdQueryHandler : IRequestHandler<GetSettlementByIdQuery, Result<SettlementDto>>
{
    private readonly ISettlementService _settlementService;

    public GetSettlementByIdQueryHandler(ISettlementService settlementService)
    {
        _settlementService = settlementService;
    }

    public async Task<Result<SettlementDto>> Handle(GetSettlementByIdQuery request, CancellationToken cancellationToken)
    {
        return await _settlementService.GetByIdAsync(request.BatchId, cancellationToken);
    }
}
