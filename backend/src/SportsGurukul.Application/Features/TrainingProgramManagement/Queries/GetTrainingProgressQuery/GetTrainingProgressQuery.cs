using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Queries.GetTrainingProgressQuery
{
    public class GetTrainingProgressQuery : IRequest<Result<TrainingProgressDto>>
    {
        public Guid EnrollmentId { get; set; }
    }
}
