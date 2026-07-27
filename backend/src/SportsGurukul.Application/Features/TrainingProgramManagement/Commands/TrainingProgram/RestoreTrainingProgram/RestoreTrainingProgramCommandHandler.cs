using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Commands.TrainingProgram.RestoreTrainingProgram;

public class RestoreTrainingProgramCommandHandler : IRequestHandler<RestoreTrainingProgramCommand, Result<TrainingProgramDto>>
{
    private readonly ITrainingProgramRepository _trainingProgramRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RestoreTrainingProgramCommandHandler> _logger;

    public RestoreTrainingProgramCommandHandler(
        ITrainingProgramRepository trainingProgramRepository,
        IUnitOfWork unitOfWork,
        ILogger<RestoreTrainingProgramCommandHandler> logger)
    {
        _trainingProgramRepository = trainingProgramRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<TrainingProgramDto>> Handle(RestoreTrainingProgramCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Restoring training program with ID: {ProgramId}", request.Id);

        var program = await _trainingProgramRepository.GetByIdWithDetailsAsync(request.Id);
        if (program == null)
        {
            _logger.LogWarning("Training program with ID {ProgramId} not found", request.Id);
            return Result<TrainingProgramDto>.Failure("Training program not found");
        }

        if (program.Status != TrainingProgramStatus.Archived)
        {
            _logger.LogWarning("Training program with code {ProgramCode} is not archived, current status: {Status}", program.ProgramCode, program.Status);
            return Result<TrainingProgramDto>.Failure("Only archived training programs can be restored");
        }

        program.Status = TrainingProgramStatus.Draft;

        _trainingProgramRepository.Update(program);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Training program restored successfully with code: {ProgramCode}", program.ProgramCode);

        var dto = TrainingProgramDto.MapToDto(program);
        return Result<TrainingProgramDto>.Success(dto);
    }
}
