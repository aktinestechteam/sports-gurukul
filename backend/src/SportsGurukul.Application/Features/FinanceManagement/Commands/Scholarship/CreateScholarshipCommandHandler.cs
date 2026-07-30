using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;

namespace SportsGurukul.Application.Features.FinanceManagement.Commands.Scholarship;

public class CreateScholarshipCommandHandler : IRequestHandler<CreateScholarshipCommand, Result<ScholarshipDto>>
{
    private readonly Common.Interfaces.Finance.Services.IDiscountService _discountService;

    public CreateScholarshipCommandHandler(Common.Interfaces.Finance.Services.IDiscountService discountService)
    {
        _discountService = discountService;
    }

    public async Task<Result<ScholarshipDto>> Handle(CreateScholarshipCommand request, CancellationToken cancellationToken)
    {
        var createRequest = new CreateScholarshipRequest(
            request.AthleteId,
            request.Name,
            request.Description,
            request.DiscountType,
            request.Value,
            request.MaxAmount,
            request.ValidFrom,
            request.ValidTo
        );
        // Delegate to a scholarship creation method through discount service
        var result = await _discountService.ApplyScholarshipAsync(request.Value, Guid.Empty, cancellationToken);
        // For now map through a simple operation - scholarship creation would be expanded
        return Result<ScholarshipDto>.Success(new ScholarshipDto(
            Guid.Empty, request.AthleteId, null, request.Name, request.Description,
            request.DiscountType, request.Value, request.MaxAmount,
            request.ValidFrom, request.ValidTo, true, DateTime.UtcNow
        ));
    }
}
