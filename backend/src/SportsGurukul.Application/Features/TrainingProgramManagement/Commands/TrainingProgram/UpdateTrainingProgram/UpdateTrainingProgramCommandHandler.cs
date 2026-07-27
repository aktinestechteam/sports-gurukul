using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Commands.TrainingProgram.UpdateTrainingProgram;

public class UpdateTrainingProgramCommandHandler : IRequestHandler<UpdateTrainingProgramCommand, Result<TrainingProgramDto>>
{
    private readonly ITrainingProgramRepository _trainingProgramRepository;
    private readonly ISportRepository _sportRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateTrainingProgramCommandHandler> _logger;

    public UpdateTrainingProgramCommandHandler(
        ITrainingProgramRepository trainingProgramRepository,
        ISportRepository sportRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateTrainingProgramCommandHandler> logger)
    {
        _trainingProgramRepository = trainingProgramRepository;
        _sportRepository = sportRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<TrainingProgramDto>> Handle(UpdateTrainingProgramCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating training program with ID: {ProgramId}", request.Id);

        var program = await _trainingProgramRepository.GetByIdWithDetailsAsync(request.Id);
        if (program == null)
        {
            _logger.LogWarning("Training program with ID {ProgramId} not found", request.Id);
            return Result<TrainingProgramDto>.Failure("Training program not found");
        }

        var sport = await _sportRepository.GetByIdAsync(request.SportId);
        if (sport == null)
        {
            _logger.LogWarning("Sport with ID {SportId} not found", request.SportId);
            return Result<TrainingProgramDto>.Failure("Sport not found");
        }

        var existingPrograms = await _trainingProgramRepository.GetByAcademyIdAsync(program.AcademyId);
        if (existingPrograms.Any(p => p.ProgramName.Equals(request.ProgramName, StringComparison.OrdinalIgnoreCase) && p.Id != request.Id))
        {
            _logger.LogWarning("Training program with name {ProgramName} already exists in academy {AcademyId}", request.ProgramName, program.AcademyId);
            return Result<TrainingProgramDto>.Failure("Training program with this name already exists in this academy");
        }

        program.ProgramName = request.ProgramName;
        program.SportId = request.SportId;
        program.Description = request.Description;
        program.DifficultyLevel = request.DifficultyLevel;
        program.MinimumAge = request.MinimumAge;
        program.MaximumAge = request.MaximumAge;
        program.DurationWeeks = request.DurationWeeks;
        program.Capacity = request.Capacity;

        _trainingProgramRepository.Update(program);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Training program updated successfully with code: {ProgramCode}", program.ProgramCode);

        var dto = TrainingProgramDto.MapToDto(program);
        return Result<TrainingProgramDto>.Success(dto);
    }
}
