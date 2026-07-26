using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Features.FacilityManagement.Commands.DeleteFacilityImage;

public class DeleteFacilityImageCommandHandler : IRequestHandler<DeleteFacilityImageCommand, Result<Unit>>
{
    private readonly IRepository<FacilityImage> _imageRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteFacilityImageCommandHandler> _logger;

    public DeleteFacilityImageCommandHandler(
        IRepository<FacilityImage> imageRepository,
        IUnitOfWork unitOfWork,
        ILogger<DeleteFacilityImageCommandHandler> logger)
    {
        _imageRepository = imageRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(DeleteFacilityImageCommand request, CancellationToken cancellationToken)
    {
        var image = await _imageRepository.GetByIdAsync(request.ImageId, cancellationToken);
        if (image is null)
        {
            return Result<Unit>.Failure("Facility image not found.");
        }

        _imageRepository.Remove(image);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Facility image soft-deleted with Id: {ImageId}", image.Id);

        return Result<Unit>.Success(Unit.Value);
    }
}
