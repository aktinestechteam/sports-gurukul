using MediatR;
using SportsGurukul.Application.Common.Interfaces.Finance.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;

namespace SportsGurukul.Application.Features.FinanceManagement.Commands.Settlement;

public class ApproveSettlementCommandHandler : IRequestHandler<ApproveSettlementCommand, Result<SettlementDto>>
{
    private readonly ISettlementService _settlementService;

    public ApproveSettlementCommandHandler(ISettlementService settlementService)
    {
        _settlementService = settlementService;
    }

    public async Task<Result<SettlementDto>> Handle(ApproveSettlementCommand request, CancellationToken cancellationToken)
    {
        return await _settlementService.ApproveSettlementAsync(request.BatchId, cancellationToken);
    }
}
