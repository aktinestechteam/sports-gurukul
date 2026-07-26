using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AcademyManagement.DTOs;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.AcademyManagement.Commands.CreateAcademy;

public class CreateAcademyCommandHandler : IRequestHandler<CreateAcademyCommand, Result<AcademyDto>>
{
    private readonly IAcademyRepository _academyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateAcademyCommandHandler> _logger;

    public CreateAcademyCommandHandler(
        IAcademyRepository academyRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateAcademyCommandHandler> logger)
    {
        _academyRepository = academyRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<AcademyDto>> Handle(CreateAcademyCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating academy with Email: {Email}", request.Email);

        var existingAcademy = await _academyRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existingAcademy is not null)
            return Result<AcademyDto>.Failure("An academy with this email already exists.");

        var academyCode = await GenerateUniqueAcademyCodeAsync(cancellationToken);

        var academy = new Academy
        {
            Id = Guid.NewGuid(),
            AcademyCode = academyCode,
            Name = request.Name,
            LegalName = request.LegalName,
            Description = request.Description,
            RegistrationNumber = request.RegistrationNumber,
            GSTNumber = request.GSTNumber,
            EstablishedDate = request.EstablishedDate,
            Website = request.Website,
            Email = request.Email,
            Phone = request.Phone,
            Status = AcademyStatus.Pending,
            VerificationStatus = VerificationStatus.Pending,
            Verification = new AcademyVerification
            {
                Id = Guid.NewGuid(),
                VerificationStatus = VerificationStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            OperatingHours = new AcademyOperatingHours
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _academyRepository.AddAsync(academy, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Academy created with Id: {AcademyId}, AcademyCode: {AcademyCode}", academy.Id, academyCode);

        return Result<AcademyDto>.Success(MapToDto(academy));
    }

    private async Task<string> GenerateUniqueAcademyCodeAsync(CancellationToken cancellationToken)
    {
        string academyCode;
        do
        {
            var datePart = DateTime.UtcNow.ToString("yyyyMMdd");
            var randomPart = Guid.NewGuid().ToString("N")[..4].ToUpperInvariant();
            academyCode = $"ACAD-{datePart}-{randomPart}";
        }
        while (await _academyRepository.AnyAsync(a => a.AcademyCode == academyCode, cancellationToken));

        return academyCode;
    }

    internal static AcademyDto MapToDto(Academy academy)
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
            Contact = academy.Contact is not null ? new ContactDto
            {
                Id = academy.Contact.Id,
                AcademyId = academy.Contact.AcademyId,
                PrimaryContactName = academy.Contact.PrimaryContactName,
                PrimaryPhone = academy.Contact.PrimaryPhone,
                PrimaryEmail = academy.Contact.PrimaryEmail,
                SecondaryContactName = academy.Contact.SecondaryContactName,
                SecondaryPhone = academy.Contact.SecondaryPhone,
                SecondaryEmail = academy.Contact.SecondaryEmail,
                Address = academy.Contact.Address,
                Country = academy.Contact.Country,
                State = academy.Contact.State,
                City = academy.Contact.City,
                PostalCode = academy.Contact.PostalCode,
                Latitude = academy.Contact.Latitude,
                Longitude = academy.Contact.Longitude,
                CreatedAt = academy.Contact.CreatedAt,
                UpdatedAt = academy.Contact.UpdatedAt
            } : null,
            OperatingHours = academy.OperatingHours is not null ? new OperatingHoursDto
            {
                Id = academy.OperatingHours.Id,
                AcademyId = academy.OperatingHours.AcademyId,
                MondayOpening = academy.OperatingHours.MondayOpening?.ToString("HH:mm"),
                MondayClosing = academy.OperatingHours.MondayClosing?.ToString("HH:mm"),
                TuesdayOpening = academy.OperatingHours.TuesdayOpening?.ToString("HH:mm"),
                TuesdayClosing = academy.OperatingHours.TuesdayClosing?.ToString("HH:mm"),
                WednesdayOpening = academy.OperatingHours.WednesdayOpening?.ToString("HH:mm"),
                WednesdayClosing = academy.OperatingHours.WednesdayClosing?.ToString("HH:mm"),
                ThursdayOpening = academy.OperatingHours.ThursdayOpening?.ToString("HH:mm"),
                ThursdayClosing = academy.OperatingHours.ThursdayClosing?.ToString("HH:mm"),
                FridayOpening = academy.OperatingHours.FridayOpening?.ToString("HH:mm"),
                FridayClosing = academy.OperatingHours.FridayClosing?.ToString("HH:mm"),
                SaturdayOpening = academy.OperatingHours.SaturdayOpening?.ToString("HH:mm"),
                SaturdayClosing = academy.OperatingHours.SaturdayClosing?.ToString("HH:mm"),
                SundayOpening = academy.OperatingHours.SundayOpening?.ToString("HH:mm"),
                SundayClosing = academy.OperatingHours.SundayClosing?.ToString("HH:mm"),
                HolidaySchedule = academy.OperatingHours.HolidaySchedule,
                CreatedAt = academy.OperatingHours.CreatedAt,
                UpdatedAt = academy.OperatingHours.UpdatedAt
            } : null,
            Branches = [],
            Sports = [],
            Facilities = [],
            Memberships = [],
            SocialLinks = [],
            CreatedAt = academy.CreatedAt,
            UpdatedAt = academy.UpdatedAt
        };
    }
}
