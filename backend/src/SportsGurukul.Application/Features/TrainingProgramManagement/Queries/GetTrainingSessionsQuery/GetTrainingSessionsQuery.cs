using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Queries.GetTrainingSessionsQuery
{
    public class GetTrainingSessionsQuery : IRequest<Result<IReadOnlyList<TrainingSessionDto>>>
    {
        public Guid? BatchId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
