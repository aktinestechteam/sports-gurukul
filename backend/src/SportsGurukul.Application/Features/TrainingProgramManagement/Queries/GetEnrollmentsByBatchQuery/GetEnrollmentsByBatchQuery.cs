using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Queries.GetEnrollmentsByBatchQuery
{
    public class GetEnrollmentsByBatchQuery : IRequest<Result<IReadOnlyList<EnrollmentDto>>>
    {
        public Guid BatchId { get; set; }
    }
}
