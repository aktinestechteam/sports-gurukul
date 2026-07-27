using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Queries.GetBatchesByProgramQuery
{
    public class GetBatchesByProgramQueryHandler : IRequestHandler<GetBatchesByProgramQuery, Result<IReadOnlyList<TrainingBatchDto>>>
    {
        private readonly ITrainingBatchRepository _repository;
        private readonly ILogger<GetBatchesByProgramQueryHandler> _logger;

        public GetBatchesByProgramQueryHandler(
            ITrainingBatchRepository repository,
            ILogger<GetBatchesByProgramQueryHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<Result<IReadOnlyList<TrainingBatchDto>>> Handle(GetBatchesByProgramQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting batches for program: {ProgramId}", request.ProgramId);

            var batches = await _repository.GetByProgramIdAsync(request.ProgramId, cancellationToken);

            var dtos = batches.Select(TrainingBatchDto.MapToDto).ToList();

            return Result<IReadOnlyList<TrainingBatchDto>>.Success(dtos);
        }
    }
}
