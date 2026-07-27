using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Commands.TrainingProgram.DeleteTrainingProgram;

public class DeleteTrainingProgramCommandHandler : IRequestHandler<DeleteTrainingProgramCommand, Result<TrainingProgramDto>>
{
    private readonly ITrainingProgramRepository _trainingProgramRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteTrainingProgramCommandHandler> _logger;

    public DeleteTrainingProgramCommandHandler(
        ITrainingProgramRepository trainingProgramRepository,
        IUnitOfWork unitOfWork,
        ILogger<DeleteTrainingProgramCommandHandler> logger)
    {
        _trainingProgramRepository = trainingProgramRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<TrainingProgramDto>> Handle(DeleteTrainingProgramCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Soft deleting training program with ID: {ProgramId}", request.Id);

        var program = await _trainingProgramRepository.GetByIdWithDetailsAsync(request.Id);
        if (program == null)
        {
            _logger.LogWarning("Training program with ID {ProgramId} not found", request.Id);
            return Result<TrainingProgramDto>.Failure("Training program not found");
        }

        if (program.Status == TrainingProgramStatus.Archived)
        {
            _logger.LogWarning("Cannot delete archived training program with code: {ProgramCode}", program.ProgramCode);
            return Result<TrainingProgramDto>.Failure("Cannot delete an archived training program");
        }

        program.Status = TrainingProgramStatus.Archived;

        _trainingProgramRepository.Update(program);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Training program soft deleted successfully with code: {ProgramCode}", program.ProgramCode);

        var dto = TrainingProgramDto.MapToDto(program);
        return Result<TrainingProgramDto>.Success(dto);
    }
}
