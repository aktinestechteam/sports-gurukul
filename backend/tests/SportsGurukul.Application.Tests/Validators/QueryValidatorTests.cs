using FluentAssertions;
using FluentValidation.TestHelper;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Queries.GetBookingById;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Queries.SearchBookings;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Queries.GetFacilityBookings;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Queries.GetCoachBookings;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Queries.GetAthleteBookings;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Queries.GetUpcomingBookings;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Queries.GetBookingHistory;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Queries.GetBookingStatistics;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Queries.GetBookingConflicts;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Validators;

namespace SportsGurukul.Application.Tests.Validators;

public class QueryValidatorTests
{
    [Fact]
    public void GetBookingByIdQuery_EmptyId_ShouldHaveError()
    {
        var query = new GetBookingByIdQuery { BookingId = Guid.Empty };
        var validator = new GetBookingByIdQueryValidator();
        var result = validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.BookingId);
    }

    [Fact]
    public void GetBookingByIdQuery_ValidId_ShouldNotHaveErrors()
    {
        var query = new GetBookingByIdQuery { BookingId = Guid.NewGuid() };
        var validator = new GetBookingByIdQueryValidator();
        var result = validator.TestValidate(query);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void SearchBookingsQuery_PageZero_ShouldHaveError()
    {
        var query = new SearchBookingsQuery { Page = 0, PageSize = 20 };
        var validator = new SearchBookingsQueryValidator();
        var result = validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.Page);
    }

    [Fact]
    public void SearchBookingsQuery_PageSizeOver100_ShouldHaveError()
    {
        var query = new SearchBookingsQuery { Page = 1, PageSize = 101 };
        var validator = new SearchBookingsQueryValidator();
        var result = validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.PageSize);
    }

    [Fact]
    public void GetFacilityBookingsQuery_EmptyFacilityId_ShouldHaveError()
    {
        var query = new GetFacilityBookingsQuery { FacilityId = Guid.Empty, Date = DateTime.UtcNow };
        var validator = new GetFacilityBookingsQueryValidator();
        var result = validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.FacilityId);
    }

    [Fact]
    public void GetCoachBookingsQuery_EmptyCoachId_ShouldHaveError()
    {
        var query = new GetCoachBookingsQuery { CoachId = Guid.Empty, Date = DateTime.UtcNow };
        var validator = new GetCoachBookingsQueryValidator();
        var result = validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.CoachId);
    }

    [Fact]
    public void GetAthleteBookingsQuery_EmptyAthleteId_ShouldHaveError()
    {
        var query = new GetAthleteBookingsQuery { AthleteId = Guid.Empty };
        var validator = new GetAthleteBookingsQueryValidator();
        var result = validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.AthleteId);
    }

    [Fact]
    public void GetUpcomingBookingsQuery_EmptyAcademyId_ShouldHaveError()
    {
        var query = new GetUpcomingBookingsQuery { AcademyId = Guid.Empty };
        var validator = new GetUpcomingBookingsQueryValidator();
        var result = validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.AcademyId);
    }

    [Fact]
    public void GetBookingHistoryQuery_EmptyBookingId_ShouldHaveError()
    {
        var query = new GetBookingHistoryQuery { BookingId = Guid.Empty };
        var validator = new GetBookingHistoryQueryValidator();
        var result = validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.BookingId);
    }

    [Fact]
    public void GetBookingStatisticsQuery_EmptyAcademyId_ShouldHaveError()
    {
        var query = new GetBookingStatisticsQuery { AcademyId = Guid.Empty };
        var validator = new GetBookingStatisticsQueryValidator();
        var result = validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.AcademyId);
    }

    [Fact]
    public void GetBookingConflictsQuery_EmptyBookingId_ShouldHaveError()
    {
        var query = new GetBookingConflictsQuery { BookingId = Guid.Empty };
        var validator = new GetBookingConflictsQueryValidator();
        var result = validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.BookingId);
    }
}
