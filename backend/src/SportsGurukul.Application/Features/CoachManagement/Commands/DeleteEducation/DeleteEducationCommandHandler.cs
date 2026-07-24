using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Features.CoachManagement.Commands.DeleteEducation;

public class DeleteEducationCommandHandler : IRequestHandler<DeleteEducationCommand, Result<Unit>>
{
    private readonly IRepository<CoachEducation> _educationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteEducationCommandHandler> _logger;

    public DeleteEducationCommandHandler(
        IRepository<CoachEducation> educationRepository,
        IUnitOfWork unitOfWork,
        ILogger<DeleteEducationCommandHandler> logger)
    {
        _educationRepository = educationRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(DeleteEducationCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting education with Id: {EducationId}", request.EducationId);

        var education = await _educationRepository.GetByIdAsync(request.EducationId, cancellationToken);
        if (education is null)
            return Result<Unit>.Failure("Education not found.");

        _educationRepository.Remove(education);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Education deleted with Id: {EducationId}", request.EducationId);

        return Result<Unit>.Success(Unit.Value);
    }
}
