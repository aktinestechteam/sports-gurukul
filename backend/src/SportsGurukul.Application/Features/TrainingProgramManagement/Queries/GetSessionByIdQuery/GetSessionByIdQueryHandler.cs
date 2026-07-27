using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Queries.GetSessionByIdQuery
{
    public class GetSessionByIdQueryHandler : IRequestHandler<GetSessionByIdQuery, Result<TrainingSessionDto>>
    {
        private readonly ISessionRepository _repository;
        private readonly ILogger<GetSessionByIdQueryHandler> _logger;

        public GetSessionByIdQueryHandler(
            ISessionRepository repository,
            ILogger<GetSessionByIdQueryHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<Result<TrainingSessionDto>> Handle(GetSessionByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting training session by ID: {Id}", request.Id);

            var session = await _repository.GetByIdWithDetailsAsync(request.Id, cancellationToken);

            if (session is null)
            {
                return Result<TrainingSessionDto>.Failure($"Training session with ID {request.Id} not found.");
            }

            var dto = TrainingSessionDto.MapToDto(session);
            return Result<TrainingSessionDto>.Success(dto);
        }
    }
}
