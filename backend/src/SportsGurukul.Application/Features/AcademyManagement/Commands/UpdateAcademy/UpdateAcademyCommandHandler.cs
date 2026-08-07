using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AcademyManagement.Commands.CreateAcademy;
using SportsGurukul.Application.Features.AcademyManagement.DTOs;

namespace SportsGurukul.Application.Features.AcademyManagement.Commands.UpdateAcademy;

public class UpdateAcademyCommandHandler : IRequestHandler<UpdateAcademyCommand, Result<AcademyDto>>
{
    private readonly IAcademyRepository _academyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateAcademyCommandHandler> _logger;

    public UpdateAcademyCommandHandler(
        IAcademyRepository academyRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateAcademyCommandHandler> logger)
    {
        _academyRepository = academyRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<AcademyDto>> Handle(UpdateAcademyCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating academy with Id: {AcademyId}", request.AcademyId);

        var academy = await _academyRepository.GetByIdWithDetailsAsync(request.AcademyId, cancellationToken);
        if (academy is null)
            return Result<AcademyDto>.Failure("Academy not found.");

        if (academy.IsDeleted)
            return Result<AcademyDto>.Failure("Academy has been deleted.");

        if (request.Name is not null)
            academy.Name = request.Name;

        if (request.LegalName is not null)
            academy.LegalName = request.LegalName;

        if (request.Description is not null)
            academy.Description = request.Description;

        if (request.RegistrationNumber is not null)
            academy.RegistrationNumber = request.RegistrationNumber;

        if (request.GSTNumber is not null)
            academy.GSTNumber = request.GSTNumber;

        if (request.EstablishedDate.HasValue)
            academy.EstablishedDate = request.EstablishedDate.Value;

        if (request.Website is not null)
            academy.Website = request.Website;

        if (request.Email is not null)
            academy.Email = request.Email;

        if (request.Phone is not null)
            academy.Phone = request.Phone;

        if (request.LogoUrl is not null)
            academy.LogoUrl = request.LogoUrl;

        if (request.BannerUrl is not null)
            academy.BannerUrl = request.BannerUrl;

        academy.UpdatedAt = DateTime.UtcNow;

        _academyRepository.Update(academy);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Academy updated with Id: {AcademyId}", request.AcademyId);

        return Result<AcademyDto>.Success(AcademyDtoMapper.Map(academy));
    }
}
