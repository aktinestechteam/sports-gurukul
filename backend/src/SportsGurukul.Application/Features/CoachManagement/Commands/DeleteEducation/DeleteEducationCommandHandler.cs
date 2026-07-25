using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Features.CoachManagement.Commands.DeleteEducation;

public class DeleteEducationCommandHandler : IRequestHandler<DeleteEducationCommand, Result<Unit>>
{
    private readonly IRepository<CoachEducation> _educationRepository;
    private readonly ICoachRepository _coachRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteEducationCommandHandler> _logger;
    private readonly ICurrentUser _currentUser;

    public DeleteEducationCommandHandler(
        IRepository<CoachEducation> educationRepository,
        ICoachRepository coachRepository,
        IUnitOfWork unitOfWork,
        ILogger<DeleteEducationCommandHandler> logger,
        ICurrentUser currentUser)
    {
        _educationRepository = educationRepository;
        _coachRepository = coachRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _currentUser = currentUser;
    }

    public async Task<Result<Unit>> Handle(DeleteEducationCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting education with Id: {EducationId}", request.EducationId);

        var education = await _educationRepository.GetByIdAsync(request.EducationId, cancellationToken);
        if (education is null)
            return Result<Unit>.Failure("Education not found.");

        var coach = await _coachRepository.GetByIdAsync(education.CoachId, cancellationToken);
        if (coach is not null && _currentUser.Roles.Contains("Coach") && coach.UserId != _currentUser.UserId)
            return Result<Unit>.Failure("You are not authorized to modify this coach's data.");

        _educationRepository.Remove(education);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Education deleted with Id: {EducationId}", request.EducationId);

        return Result<Unit>.Success(Unit.Value);
    }
}
