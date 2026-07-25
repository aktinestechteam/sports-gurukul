using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.DTOs;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Features.CoachManagement.Commands.AddEducation;

public class AddEducationCommandHandler : IRequestHandler<AddEducationCommand, Result<EducationDto>>
{
    private readonly ICoachRepository _coachRepository;
    private readonly IRepository<CoachEducation> _educationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AddEducationCommandHandler> _logger;
    private readonly ICurrentUser _currentUser;

    public AddEducationCommandHandler(
        ICoachRepository coachRepository,
        IRepository<CoachEducation> educationRepository,
        IUnitOfWork unitOfWork,
        ILogger<AddEducationCommandHandler> logger,
        ICurrentUser currentUser)
    {
        _coachRepository = coachRepository;
        _educationRepository = educationRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _currentUser = currentUser;
    }

    public async Task<Result<EducationDto>> Handle(AddEducationCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Adding education to coach: {CoachId}", request.CoachId);

        var coach = await _coachRepository.GetByIdAsync(request.CoachId, cancellationToken);
        if (coach is null)
            return Result<EducationDto>.Failure("Coach not found.");

        if (_currentUser.Roles.Contains("Coach") && coach.UserId != _currentUser.UserId)
            return Result<EducationDto>.Failure("You are not authorized to modify this coach's data.");

        var education = new CoachEducation
        {
            Id = Guid.NewGuid(),
            CoachId = request.CoachId,
            Degree = request.Degree,
            Institution = request.Institution,
            FieldOfStudy = request.FieldOfStudy,
            YearCompleted = request.YearCompleted
        };

        await _educationRepository.AddAsync(education, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Education added to coach: {CoachId}, EducationId: {EducationId}", request.CoachId, education.Id);

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
