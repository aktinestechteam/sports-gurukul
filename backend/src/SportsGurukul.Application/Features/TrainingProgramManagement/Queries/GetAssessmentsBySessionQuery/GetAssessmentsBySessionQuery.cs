using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Queries.GetAssessmentsBySessionQuery;

public class GetAssessmentsBySessionQuery : IRequest<Result<IReadOnlyList<AssessmentDto>>>
{
    public Guid SessionId { get; set; }
}
