using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Queries.GetTrainingProgramByIdQuery
{
    public class GetTrainingProgramByIdQuery : IRequest<Result<TrainingProgramDto>>
    {
        public Guid Id { get; set; }
    }
}
