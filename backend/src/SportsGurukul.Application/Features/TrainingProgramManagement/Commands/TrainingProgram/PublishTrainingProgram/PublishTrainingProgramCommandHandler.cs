using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Commands.TrainingProgram.PublishTrainingProgram;

public class PublishTrainingProgramCommandHandler : IRequestHandler<PublishTrainingProgramCommand, Result<TrainingProgramDto>>
{
    private readonly ITrainingProgramRepository _trainingProgramRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<PublishTrainingProgramCommandHandler> _logger;

    public PublishTrainingProgramCommandHandler(
        ITrainingProgramRepository trainingProgramRepository,
        IUnitOfWork unitOfWork,
        ILogger<PublishTrainingProgramCommandHandler> logger)
    {
        _trainingProgramRepository = trainingProgramRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<TrainingProgramDto>> Handle(PublishTrainingProgramCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Publishing training program with ID: {ProgramId}", request.Id);

        var program = await _trainingProgramRepository.GetByIdWithDetailsAsync(request.Id);
        if (program == null)
        {
            _logger.LogWarning("Training program with ID {ProgramId} not found", request.Id);
            return Result<TrainingProgramDto>.Failure("Training program not found");
        }

        if (program.Status != TrainingProgramStatus.Draft)
        {
            _logger.LogWarning("Training program with code {ProgramCode} cannot be published, current status: {Status}", program.ProgramCode, program.Status);
            return Result<TrainingProgramDto>.Failure("Training program can only be published from Draft status");
        }

        program.Status = TrainingProgramStatus.Active;

        _trainingProgramRepository.Update(program);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Training program published successfully with code: {ProgramCode}", program.ProgramCode);

        var dto = TrainingProgramDto.MapToDto(program);
        return Result<TrainingProgramDto>.Success(dto);
    }
}
