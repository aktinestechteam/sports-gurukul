using MediatR;
using SportsGurukul.Application.Common.Interfaces.Finance.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;

namespace SportsGurukul.Application.Features.FinanceManagement.Commands.Settlement;

public class CreateSettlementBatchCommandHandler : IRequestHandler<CreateSettlementBatchCommand, Result<SettlementDto>>
{
    private readonly ISettlementService _settlementService;

    public CreateSettlementBatchCommandHandler(ISettlementService settlementService)
    {
        _settlementService = settlementService;
    }

    public async Task<Result<SettlementDto>> Handle(CreateSettlementBatchCommand request, CancellationToken cancellationToken)
    {
        return await _settlementService.CreateSettlementBatchAsync(request.PaymentIds, cancellationToken);
    }
}
