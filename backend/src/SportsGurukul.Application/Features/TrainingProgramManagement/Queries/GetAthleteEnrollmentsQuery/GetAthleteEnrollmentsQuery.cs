using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Queries.GetAthleteEnrollmentsQuery
{
    public class GetAthleteEnrollmentsQuery : IRequest<Result<IReadOnlyList<EnrollmentDto>>>
    {
        public Guid AthleteId { get; set; }
    }
}
