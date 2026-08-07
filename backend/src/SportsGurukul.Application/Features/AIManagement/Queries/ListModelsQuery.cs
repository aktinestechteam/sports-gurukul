using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.ModelRouting;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Features.AIManagement.Queries;

public record ListModelsQuery(
    string? SearchTerm,
    AIModelFamily? Family,
    Guid? ProviderId,
    bool? SupportsChat,
    bool? SupportsFunctionCalling,
    bool? SupportsVision,
    bool? SupportsJsonMode,
    int Page = 1,
    int PageSize = 50
) : IRequest<Result<IReadOnlyList<ModelCandidate>>>;
