using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Queries.GetEnrollmentsByBatchQuery
{
    public class GetEnrollmentsByBatchQueryHandler : IRequestHandler<GetEnrollmentsByBatchQuery, Result<IReadOnlyList<EnrollmentDto>>>
    {
        private readonly ITrainingBatchRepository _batchRepository;
        private readonly ILogger<GetEnrollmentsByBatchQueryHandler> _logger;

        public GetEnrollmentsByBatchQueryHandler(
            ITrainingBatchRepository batchRepository,
            ILogger<GetEnrollmentsByBatchQueryHandler> logger)
        {
            _batchRepository = batchRepository;
            _logger = logger;
        }

        public async Task<Result<IReadOnlyList<EnrollmentDto>>> Handle(GetEnrollmentsByBatchQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting enrollments for batch: {BatchId}", request.BatchId);

            var batch = await _batchRepository.GetByIdWithDetailsAsync(request.BatchId, cancellationToken);

            if (batch is null)
            {
                return Result<IReadOnlyList<EnrollmentDto>>.Failure($"Training batch with ID {request.BatchId} not found.");
            }

            var dtos = batch.Enrollments?.Select(EnrollmentDto.MapToDto).ToList()
                       ?? new List<EnrollmentDto>();

            return Result<IReadOnlyList<EnrollmentDto>>.Success(dtos);
        }
    }
}
