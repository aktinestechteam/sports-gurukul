using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Commands.TrainingProgram.ArchiveTrainingProgram;

public class ArchiveTrainingProgramCommandHandler : IRequestHandler<ArchiveTrainingProgramCommand, Result<TrainingProgramDto>>
{
    private readonly ITrainingProgramRepository _trainingProgramRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ArchiveTrainingProgramCommandHandler> _logger;

    public ArchiveTrainingProgramCommandHandler(
        ITrainingProgramRepository trainingProgramRepository,
        IUnitOfWork unitOfWork,
        ILogger<ArchiveTrainingProgramCommandHandler> logger)
    {
        _trainingProgramRepository = trainingProgramRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<TrainingProgramDto>> Handle(ArchiveTrainingProgramCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Archiving training program with ID: {ProgramId}", request.Id);

        var program = await _trainingProgramRepository.GetByIdWithDetailsAsync(request.Id);
        if (program == null)
        {
            _logger.LogWarning("Training program with ID {ProgramId} not found", request.Id);
            return Result<TrainingProgramDto>.Failure("Training program not found");
        }

        if (program.Status != TrainingProgramStatus.Active && program.Status != TrainingProgramStatus.Completed)
        {
            _logger.LogWarning("Training program with code {ProgramCode} cannot be archived, current status: {Status}", program.ProgramCode, program.Status);
            return Result<TrainingProgramDto>.Failure("Training program can only be archived when Active or Completed");
        }

        program.Status = TrainingProgramStatus.Archived;

        _trainingProgramRepository.Update(program);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Training program archived successfully with code: {ProgramCode}", program.ProgramCode);

        var dto = TrainingProgramDto.MapToDto(program);
        return Result<TrainingProgramDto>.Success(dto);
    }
}
