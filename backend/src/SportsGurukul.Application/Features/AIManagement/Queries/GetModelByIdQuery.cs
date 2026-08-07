using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.ModelRouting;

namespace SportsGurukul.Application.Features.AIManagement.Queries;

public record GetModelByIdQuery(Guid ModelId) : IRequest<Result<ModelCandidate>>;
