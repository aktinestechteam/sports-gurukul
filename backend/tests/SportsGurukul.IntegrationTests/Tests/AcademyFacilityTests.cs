using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.AcademyManagement.DTOs;
using SportsGurukul.Application.Features.FacilityManagement.DTOs;
using SportsGurukul.Domain.Enums;
using SportsGurukul.IntegrationTests.Bases;
using SportsGurukul.IntegrationTests.Fixtures;

namespace SportsGurukul.IntegrationTests.Tests;

public class AcademyFacilityTests : AcademyIntegrationTestBase
{
    public AcademyFacilityTests(PostgresFixture postgresFixture) : base(postgresFixture) { }

    private async Task<Guid> CreateAcademyAndGetIdAsync()
    {
        var response = await AcademyAdminClient.PostAsJsonAsync("/api/v1/academies", new
        {
            Name = $"Facility Academy {Guid.NewGuid().ToString()[..6]}",
            Email = $"facility{Guid.NewGuid().ToString()[..6]}@test.com",
            Phone = "+919876543210"
        });
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<AcademyDto>>();
        return content!.Data!.Id;
    }

    private async Task<Guid> CreateFacilityAndGetIdAsync(Guid academyId)
    {
        var response = await AcademyAdminClient.PostAsJsonAsync("/api/v1/facilities", new
        {
            AcademyId = academyId,
            FacilityName = $"Badminton Hall {Guid.NewGuid().ToString()[..4]}",
            FacilityType = FacilityType.BadmintonCourt,
            Capacity = 120,
            IndoorOutdoor = IndoorOutdoor.Indoor,
            LightingAvailable = true,
            ParkingAvailable = true
        });
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<FacilityDetailDto>>();
        return content?.Data?.Id ?? Guid.Empty;
    }

    [Fact]
    public async Task CreateFacility_ValidRequest_ReturnsCreated()
    {
        var academyId = await CreateAcademyAndGetIdAsync();

        var response = await AcademyAdminClient.PostAsJsonAsync("/api/v1/facilities", new
        {
            AcademyId = academyId,
            FacilityName = "Indoor Badminton Hall",
            FacilityType = FacilityType.BadmintonCourt,
            Capacity = 120,
            IndoorOutdoor = IndoorOutdoor.Indoor,
            LightingAvailable = true,
            ParkingAvailable = true,
            ChangingRoomAvailable = true,
            WashroomAvailable = true,
            Description = "Professional-grade indoor badminton hall"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<FacilityDetailDto>>();
        content!.Success.Should().BeTrue();
        content.Data!.FacilityName.Should().Be("Indoor Badminton Hall");
    }

    [Fact]
    public async Task CreateFacility_EmptyName_ReturnsBadRequest()
    {
        var academyId = await CreateAcademyAndGetIdAsync();

        var response = await AcademyAdminClient.PostAsJsonAsync("/api/v1/facilities", new
        {
            AcademyId = academyId,
            FacilityName = "",
            FacilityType = FacilityType.BadmintonCourt,
            Capacity = 120
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateFacility_AcademyNotFound_ReturnsNotFound()
    {
        var response = await AcademyAdminClient.PostAsJsonAsync("/api/v1/facilities", new
        {
            AcademyId = Guid.NewGuid(),
            FacilityName = "Test Facility",
            FacilityType = FacilityType.BadmintonCourt,
            Capacity = 120
        });

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetFacilityById_Exists_ReturnsFacility()
    {
        var academyId = await CreateAcademyAndGetIdAsync();
        var facilityId = await CreateFacilityAndGetIdAsync(academyId);

        var response = await AcademyAdminClient.GetAsync($"/api/v1/facilities/{facilityId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<FacilityDetailDto>>();
        content!.Data!.Id.Should().Be(facilityId);
    }

    [Fact]
    public async Task GetFacilityById_NotExists_ReturnsNotFound()
    {
        var response = await AcademyAdminClient.GetAsync($"/api/v1/facilities/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateFacility_Exists_UpdatesSuccessfully()
    {
        var academyId = await CreateAcademyAndGetIdAsync();
        var facilityId = await CreateFacilityAndGetIdAsync(academyId);

        var response = await AcademyAdminClient.PutAsJsonAsync($"/api/v1/facilities/{facilityId}", new
        {
            FacilityName = "Pro Badminton Arena",
            Capacity = 200
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<FacilityDetailDto>>();
        content!.Data!.FacilityName.Should().Be("Pro Badminton Arena");
    }

    [Fact]
    public async Task UpdateFacility_NotExists_ReturnsNotFound()
    {
        var response = await AcademyAdminClient.PutAsJsonAsync($"/api/v1/facilities/{Guid.NewGuid()}", new
        {
            FacilityName = "Updated"
        });

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteFacility_Exists_DeletesSuccessfully()
    {
        var academyId = await CreateAcademyAndGetIdAsync();
        var facilityId = await CreateFacilityAndGetIdAsync(academyId);

        var response = await AcademyAdminClient.DeleteAsync($"/api/v1/facilities/{facilityId}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteFacility_NotExists_ReturnsNotFound()
    {
        var response = await AcademyAdminClient.DeleteAsync($"/api/v1/facilities/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task RestoreFacility_Exists_RestoresSuccessfully()
    {
        var academyId = await CreateAcademyAndGetIdAsync();
        var facilityId = await CreateFacilityAndGetIdAsync(academyId);
        await AcademyAdminClient.DeleteAsync($"/api/v1/facilities/{facilityId}");

        var response = await AcademyAdminClient.PostAsync($"/api/v1/facilities/{facilityId}/restore", null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task RestoreFacility_NotDeleted_ReturnsBadRequest()
    {
        var academyId = await CreateAcademyAndGetIdAsync();
        var facilityId = await CreateFacilityAndGetIdAsync(academyId);

        var response = await AcademyAdminClient.PostAsync($"/api/v1/facilities/{facilityId}/restore", null);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AddCourt_ValidRequest_ReturnsCreated()
    {
        var academyId = await CreateAcademyAndGetIdAsync();
        var facilityId = await CreateFacilityAndGetIdAsync(academyId);

        var response = await AcademyAdminClient.PostAsJsonAsync($"/api/v1/facilities/{facilityId}/courts", new
        {
            CourtNumber = "1",
            CourtName = "Court A1",
            CourtType = "Doubles",
            Capacity = 4
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<CourtDto>>();
        content!.Success.Should().BeTrue();
        content.Data!.CourtNumber.Should().Be("1");
        content.Data.CourtName.Should().Be("Court A1");
    }

    [Fact]
    public async Task AddCourt_DuplicateNumber_ReturnsConflict()
    {
        var academyId = await CreateAcademyAndGetIdAsync();
        var facilityId = await CreateFacilityAndGetIdAsync(academyId);
        await AcademyAdminClient.PostAsJsonAsync($"/api/v1/facilities/{facilityId}/courts", new
        {
            CourtNumber = "1",
            CourtName = "Court A1"
        });

        var response = await AcademyAdminClient.PostAsJsonAsync($"/api/v1/facilities/{facilityId}/courts", new
        {
            CourtNumber = "1",
            CourtName = "Court A1 Duplicate"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task AddCourt_FacilityNotFound_ReturnsNotFound()
    {
        var response = await AcademyAdminClient.PostAsJsonAsync($"/api/v1/facilities/{Guid.NewGuid()}/courts", new
        {
            CourtNumber = "1",
            CourtName = "Test"
        });

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AddEquipment_ValidRequest_ReturnsCreated()
    {
        var academyId = await CreateAcademyAndGetIdAsync();
        var facilityId = await CreateFacilityAndGetIdAsync(academyId);

        var response = await AcademyAdminClient.PostAsJsonAsync($"/api/v1/facilities/{facilityId}/equipment", new
        {
            EquipmentName = "Yonex Astrox 88D Pro",
            Category = "Racket",
            Condition = EquipmentCondition.New,
            Quantity = 20,
            Description = "Professional-grade badminton rackets"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<EquipmentDto>>();
        content!.Success.Should().BeTrue();
        content.Data!.EquipmentName.Should().Be("Yonex Astrox 88D Pro");
    }

    [Fact]
    public async Task AddEquipment_EmptyName_ReturnsBadRequest()
    {
        var academyId = await CreateAcademyAndGetIdAsync();
        var facilityId = await CreateFacilityAndGetIdAsync(academyId);

        var response = await AcademyAdminClient.PostAsJsonAsync($"/api/v1/facilities/{facilityId}/equipment", new
        {
            EquipmentName = "",
            Category = "Racket",
            Condition = EquipmentCondition.New
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdatePricing_ValidRequest_ReturnsOk()
    {
        var academyId = await CreateAcademyAndGetIdAsync();
        var facilityId = await CreateFacilityAndGetIdAsync(academyId);

        var response = await AcademyAdminClient.PutAsJsonAsync($"/api/v1/facilities/{facilityId}/pricing", new
        {
            PricingName = "Standard Court Rental",
            HourlyRate = 500.00m,
            DailyRate = 3000.00m,
            MonthlyRate = 25000.00m,
            PeakHourlyRate = 750.00m,
            OffPeakHourlyRate = 350.00m,
            Description = "Standard pricing"
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<PricingDto>>();
        content!.Success.Should().BeTrue();
        content.Data!.PricingName.Should().Be("Standard Court Rental");
        content.Data.HourlyRate.Should().Be(500.00m);
    }

    [Fact]
    public async Task UpdateSchedule_ValidRequest_ReturnsOk()
    {
        var academyId = await CreateAcademyAndGetIdAsync();
        var facilityId = await CreateFacilityAndGetIdAsync(academyId);

        var response = await AcademyAdminClient.PutAsJsonAsync($"/api/v1/facilities/{facilityId}/schedule", new
        {
            DayOfWeek = 1,
            OpeningTime = "06:00",
            ClosingTime = "22:00",
            IsClosed = false,
            IsMaintenanceWindow = false,
            Notes = "Extended hours on match days"
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<ScheduleDto>>();
        content!.Success.Should().BeTrue();
    }

    [Fact]
    public async Task GetFacilities_FilterByAcademy_ReturnsCorrectList()
    {
        var academyId = await CreateAcademyAndGetIdAsync();
        await CreateFacilityAndGetIdAsync(academyId);

        var response = await AcademyAdminClient.GetAsync($"/api/v1/facilities?academyId={academyId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<FacilitySearchResponse>>();
        content!.Success.Should().BeTrue();
        content.Data!.Items.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Facility_Authorization_Unauthenticated_ReturnsUnauthorized()
    {
        var response = await UnauthenticatedClient.GetAsync("/api/v1/facilities");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Facility_Authorization_AthleteRole_ReturnsForbiddenForCreate()
    {
        var response = await AthleteClient.PostAsJsonAsync("/api/v1/facilities", new
        {
            AcademyId = Guid.NewGuid(),
            FacilityName = "Test",
            FacilityType = FacilityType.BadmintonCourt,
            Capacity = 10
        });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Facility_Authorization_AthleteRole_CanGetFacilities()
    {
        var response = await AthleteClient.GetAsync("/api/v1/facilities");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
