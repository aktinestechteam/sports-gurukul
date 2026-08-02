using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Queries;

public record GetModelsQuery(
    string? SearchTerm,
    Guid? ProviderId,
    string? Capability,
    bool? ActiveOnly,
    int Page = 1,
    int PageSize = 50
) : IRequest<Result<PaginatedResult<ModelCatalogDto>>>;
