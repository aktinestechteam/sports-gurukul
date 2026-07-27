using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Queries.GetSessionByIdQuery
{
    public class GetSessionByIdQuery : IRequest<Result<TrainingSessionDto>>
    {
        public Guid Id { get; set; }
    }
}
