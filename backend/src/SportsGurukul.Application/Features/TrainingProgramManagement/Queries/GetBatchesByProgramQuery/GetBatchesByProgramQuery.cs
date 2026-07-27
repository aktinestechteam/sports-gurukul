using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Queries.GetBatchesByProgramQuery
{
    public class GetBatchesByProgramQuery : IRequest<Result<IReadOnlyList<TrainingBatchDto>>>
    {
        public Guid ProgramId { get; set; }
    }
}
