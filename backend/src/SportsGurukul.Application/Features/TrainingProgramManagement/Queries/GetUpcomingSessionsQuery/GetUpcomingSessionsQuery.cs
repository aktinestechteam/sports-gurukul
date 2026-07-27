using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Queries.GetUpcomingSessionsQuery
{
    public class GetUpcomingSessionsQuery : IRequest<Result<IReadOnlyList<TrainingSessionDto>>>
    {
        public Guid? CoachId { get; set; }
        public Guid? BatchId { get; set; }
    }
}
