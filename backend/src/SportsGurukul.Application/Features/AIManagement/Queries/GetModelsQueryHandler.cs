using MediatR;
using SportsGurukul.Application.Common.Interfaces.AI;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Features.AIManagement.Queries;

public class GetModelsQueryHandler : IRequestHandler<GetModelsQuery, Result<PaginatedResult<ModelCatalogDto>>>
{
    private readonly IAIModelRepository _modelRepo;

    public GetModelsQueryHandler(IAIModelRepository modelRepo)
    {
        _modelRepo = modelRepo;
    }

    public async Task<Result<PaginatedResult<ModelCatalogDto>>> Handle(GetModelsQuery request, CancellationToken cancellationToken)
    {
        var all = await _modelRepo.FindAsync(m => !m.IsDeleted, cancellationToken);

        var filtered = all.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            filtered = filtered.Where(m =>
                m.Name.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                (m.DisplayName != null && m.DisplayName.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase)));

        if (request.ProviderId.HasValue)
            filtered = filtered.Where(m => m.ProviderId == request.ProviderId.Value);

        if (request.ActiveOnly == true)
            filtered = filtered.Where(m => m.Status == AIModelStatus.Active);

        var list = filtered.ToList();
        var total = list.Count;
        var paged = list
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(m => new ModelCatalogDto(
                m.Id, m.ProviderId, m.Provider?.Name ?? "Unknown",
                m.Name, m.DisplayName, m.Description, m.Capabilities,
                m.Status, m.MaxTokens, m.MaxContextLength,
                m.CostPerInputToken, m.CostPerOutputToken,
                m.DefaultTemperature, m.SupportsStreaming,
                m.SupportsFunctionCalling, m.SupportsVision,
                m.SupportsEmbeddings, m.ModelVersion, m.ReleasedAt,
                m.CreatedAt
            ))
            .ToList();

        return Result<PaginatedResult<ModelCatalogDto>>.Success(
            new PaginatedResult<ModelCatalogDto>(paged, total, request.Page, request.PageSize));
    }
}
