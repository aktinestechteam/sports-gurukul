using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.DTOs;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Features.CoachManagement.Commands.UpdateEducation;

public class UpdateEducationCommandHandler : IRequestHandler<UpdateEducationCommand, Result<EducationDto>>
{
    private readonly IRepository<CoachEducation> _educationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateEducationCommandHandler> _logger;

    public UpdateEducationCommandHandler(
        IRepository<CoachEducation> educationRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateEducationCommandHandler> logger)
    {
        _educationRepository = educationRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<EducationDto>> Handle(UpdateEducationCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating education with Id: {EducationId}", request.EducationId);

        var education = await _educationRepository.GetByIdAsync(request.EducationId, cancellationToken);
        if (education is null)
            return Result<EducationDto>.Failure("Education not found.");

        if (request.Degree is not null)
            education.Degree = request.Degree;

        if (request.Institution is not null)
            education.Institution = request.Institution;

        if (request.FieldOfStudy is not null)
            education.FieldOfStudy = request.FieldOfStudy;

        if (request.YearCompleted.HasValue)
            education.YearCompleted = request.YearCompleted.Value;

        education.UpdatedAt = DateTime.UtcNow;

        _educationRepository.Update(education);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Education updated with Id: {EducationId}", request.EducationId);

        var dto = new EducationDto
        {
            Id = education.Id,
            Degree = education.Degree,
            Institution = education.Institution,
            FieldOfStudy = education.FieldOfStudy,
            YearCompleted = education.YearCompleted,
            CreatedAt = education.CreatedAt,
            UpdatedAt = education.UpdatedAt
        };

        return Result<EducationDto>.Success(dto);
    }
}
