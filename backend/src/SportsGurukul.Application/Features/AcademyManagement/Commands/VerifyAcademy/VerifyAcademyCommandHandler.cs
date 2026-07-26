using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AcademyManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.AcademyManagement.Commands.VerifyAcademy;

public class VerifyAcademyCommandHandler : IRequestHandler<VerifyAcademyCommand, Result<AcademyDto>>
{
    private readonly IAcademyRepository _academyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<VerifyAcademyCommandHandler> _logger;

    public VerifyAcademyCommandHandler(
        IAcademyRepository academyRepository,
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser,
        ILogger<VerifyAcademyCommandHandler> logger)
    {
        _academyRepository = academyRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<Result<AcademyDto>> Handle(VerifyAcademyCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Verifying academy: {AcademyId}", request.AcademyId);

        var academy = await _academyRepository.GetByIdWithDetailsAsync(request.AcademyId, cancellationToken);
        if (academy is null)
        {
            _logger.LogWarning("Academy not found: {AcademyId}", request.AcademyId);
            return Result<AcademyDto>.Failure("Academy not found.");
        }

        if (academy.Verification is null)
        {
            _logger.LogWarning("Academy verification record not found: {AcademyId}", request.AcademyId);
            return Result<AcademyDto>.Failure("Academy verification record not found.");
        }

        if (academy.Verification.VerificationStatus == VerificationStatus.Verified)
        {
            _logger.LogWarning("Academy is already verified: {AcademyId}", request.AcademyId);
            return Result<AcademyDto>.Failure("Academy is already verified.");
        }

        var now = DateTime.UtcNow;

        academy.Verification.VerificationStatus = VerificationStatus.Verified;
        academy.Verification.VerifiedBy = _currentUser.UserId;
        academy.Verification.VerifiedOn = now;
        academy.Verification.Remarks = request.Remarks;
        academy.Verification.UpdatedAt = now;

        academy.VerificationStatus = VerificationStatus.Verified;
        academy.UpdatedAt = now;

        _academyRepository.Update(academy);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Academy verified: {AcademyId}", request.AcademyId);

        var dto = MapToDto(academy);

        return Result<AcademyDto>.Success(dto);
    }

    private static AcademyDto MapToDto(Domain.Entities.Academy academy)
    {
        return new AcademyDto
        {
            Id = academy.Id,
            AcademyCode = academy.AcademyCode,
            Name = academy.Name,
            LegalName = academy.LegalName,
            Description = academy.Description,
            RegistrationNumber = academy.RegistrationNumber,
            GSTNumber = academy.GSTNumber,
            EstablishedDate = academy.EstablishedDate,
            Website = academy.Website,
            Email = academy.Email,
            Phone = academy.Phone,
            Status = academy.Status.ToString(),
            VerificationStatus = academy.VerificationStatus.ToString(),
            LogoUrl = academy.LogoUrl,
            BannerUrl = academy.BannerUrl,
            Contact = academy.Contact is null ? null : new DTOs.ContactDto
            {
                Id = academy.Contact.Id,
                PrimaryContactName = academy.Contact.PrimaryContactName,
                PrimaryPhone = academy.Contact.PrimaryPhone,
                PrimaryEmail = academy.Contact.PrimaryEmail,
                SecondaryContactName = academy.Contact.SecondaryContactName,
                SecondaryPhone = academy.Contact.SecondaryPhone,
                SecondaryEmail = academy.Contact.SecondaryEmail
            },
            Branches = academy.Branches.Select(b => new BranchDto
            {
                Id = b.Id,
                AcademyId = b.AcademyId,
                BranchName = b.BranchName,
                Address = b.Address,
                Country = b.Country,
                State = b.State,
                City = b.City,
                District = b.District,
                PostalCode = b.PostalCode,
                Latitude = b.Latitude,
                Longitude = b.Longitude,
                CreatedAt = b.CreatedAt,
                UpdatedAt = b.UpdatedAt
            }).ToList(),
            CreatedAt = academy.CreatedAt,
            UpdatedAt = academy.UpdatedAt
        };
    }
}
