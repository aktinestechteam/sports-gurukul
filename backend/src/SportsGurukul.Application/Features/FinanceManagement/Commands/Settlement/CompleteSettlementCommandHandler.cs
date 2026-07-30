using MediatR;
using SportsGurukul.Application.Common.Interfaces.Finance.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;

namespace SportsGurukul.Application.Features.FinanceManagement.Commands.Settlement;

public class CompleteSettlementCommandHandler : IRequestHandler<CompleteSettlementCommand, Result<SettlementDto>>
{
    private readonly ISettlementService _settlementService;

    public CompleteSettlementCommandHandler(ISettlementService settlementService)
    {
        _settlementService = settlementService;
    }

    public async Task<Result<SettlementDto>> Handle(CompleteSettlementCommand request, CancellationToken cancellationToken)
    {
        return await _settlementService.CompleteSettlementAsync(request.BatchId, request.Reference, cancellationToken);
    }
}
