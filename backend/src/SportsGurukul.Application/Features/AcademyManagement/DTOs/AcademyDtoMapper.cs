using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Features.AcademyManagement.DTOs;

/// <summary>
/// Maps an <see cref="Academy"/> aggregate to its response DTO.
/// </summary>
public static class AcademyDtoMapper
{
    public static AcademyDto Map(Academy academy)
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
            AcademyType = academy.AcademyType.ToString(),
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
