using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Queries.GetAssessmentResultsQuery
{
    public class GetAssessmentResultsQuery : IRequest<Result<IReadOnlyList<AssessmentResultDto>>>
    {
        public Guid AssessmentId { get; set; }
    }
}
