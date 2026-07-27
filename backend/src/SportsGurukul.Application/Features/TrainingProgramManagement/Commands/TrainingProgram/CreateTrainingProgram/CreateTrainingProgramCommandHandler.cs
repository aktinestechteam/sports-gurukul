using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Commands.TrainingProgram.CreateTrainingProgram;

public class CreateTrainingProgramCommandHandler : IRequestHandler<CreateTrainingProgramCommand, Result<TrainingProgramDto>>
{
    private readonly ITrainingProgramRepository _trainingProgramRepository;
    private readonly IAcademyRepository _academyRepository;
    private readonly ISportRepository _sportRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateTrainingProgramCommandHandler> _logger;

    public CreateTrainingProgramCommandHandler(
        ITrainingProgramRepository trainingProgramRepository,
        IAcademyRepository academyRepository,
        ISportRepository sportRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateTrainingProgramCommandHandler> logger)
    {
        _trainingProgramRepository = trainingProgramRepository;
        _academyRepository = academyRepository;
        _sportRepository = sportRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<TrainingProgramDto>> Handle(CreateTrainingProgramCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating training program with name: {ProgramName} for academy: {AcademyId}", request.ProgramName, request.AcademyId);

        var academy = await _academyRepository.GetByIdAsync(request.AcademyId);
        if (academy == null)
        {
            _logger.LogWarning("Academy with ID {AcademyId} not found", request.AcademyId);
            return Result<TrainingProgramDto>.Failure("Academy not found");
        }

        var sport = await _sportRepository.GetByIdAsync(request.SportId);
        if (sport == null)
        {
            _logger.LogWarning("Sport with ID {SportId} not found", request.SportId);
            return Result<TrainingProgramDto>.Failure("Sport not found");
        }

        var programCode = $"TPR-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpper()}";

        var existingPrograms = await _trainingProgramRepository.GetByAcademyIdAsync(request.AcademyId);
        if (existingPrograms.Any(p => p.ProgramName.Equals(request.ProgramName, StringComparison.OrdinalIgnoreCase)))
        {
            _logger.LogWarning("Training program with name {ProgramName} already exists in academy {AcademyId}", request.ProgramName, request.AcademyId);
            return Result<TrainingProgramDto>.Failure("Training program with this name already exists in this academy");
        }

        var program = new Domain.Entities.TrainingProgram
        {
            Id = Guid.NewGuid(),
            ProgramCode = programCode,
            ProgramName = request.ProgramName,
            SportId = request.SportId,
            AcademyId = request.AcademyId,
            Description = request.Description,
            DifficultyLevel = request.DifficultyLevel,
            MinimumAge = request.MinimumAge,
            MaximumAge = request.MaximumAge,
            DurationWeeks = request.DurationWeeks,
            Capacity = request.Capacity,
            Status = TrainingProgramStatus.Draft
        };

        await _trainingProgramRepository.AddAsync(program);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Training program created successfully with code: {ProgramCode}", programCode);

        var dto = TrainingProgramDto.MapToDto(program);
        dto.SportName = sport.Name;
        dto.AcademyName = academy.Name;

        return Result<TrainingProgramDto>.Success(dto);
    }
}
