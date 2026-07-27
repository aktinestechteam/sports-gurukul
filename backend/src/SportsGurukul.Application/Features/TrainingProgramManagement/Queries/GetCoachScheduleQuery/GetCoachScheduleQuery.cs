using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Queries.GetCoachScheduleQuery
{
    public class GetCoachScheduleQuery : IRequest<Result<IReadOnlyList<TrainingSessionDto>>>
    {
        public Guid CoachId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
