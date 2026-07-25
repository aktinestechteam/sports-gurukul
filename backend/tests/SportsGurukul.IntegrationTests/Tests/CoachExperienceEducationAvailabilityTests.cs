using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.DTOs;
using SportsGurukul.IntegrationTests.Bases;
using SportsGurukul.IntegrationTests.Fixtures;

namespace SportsGurukul.IntegrationTests.Tests;

public class CoachExperienceEducationAvailabilityTests : CoachIntegrationTestBase
{
    public CoachExperienceEducationAvailabilityTests(PostgresFixture postgresFixture) : base(postgresFixture) { }

    [Fact]
    public async Task AddExperience_Admin_AddsSuccessfully()
    {
        var coach = await CreateTestCoachAsync();
        coach.Should().NotBeNull();

        var request = new AddExperienceRequest
        {
            Organization = "Mumbai Cricket Academy",
            Role = "Head Coach",
            Sport = "Cricket",
            StartDate = new DateTime(2018, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            EndDate = new DateTime(2022, 12, 31, 0, 0, 0, DateTimeKind.Utc),
            Description = "Led senior team"
        };

        var response = await AdminClient.PostAsJsonAsync($"/api/v1/coach/{coach!.Id}/experience", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<ExperienceDto>>();
        content.Should().NotBeNull();
        content!.Success.Should().BeTrue();
        content.Data.Should().NotBeNull();
        content.Data!.Organization.Should().Be("Mumbai Cricket Academy");
        content.Data.Role.Should().Be("Head Coach");
    }

    [Fact]
    public async Task AddExperience_CoachOwner_AddsSuccessfully()
    {
        var coach = await CreateTestCoachAsync(SeedData.CoachUserId);
        coach.Should().NotBeNull();

        var request = new AddExperienceRequest
        {
            Organization = "Delhi FC",
            Role = "Assistant Coach",
            StartDate = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        var response = await CoachClient.PostAsJsonAsync($"/api/v1/coach/{coach!.Id}/experience", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task AddExperience_AthleteRole_ReturnsForbidden()
    {
        var coach = await CreateTestCoachAsync();
        coach.Should().NotBeNull();

        var request = new AddExperienceRequest
        {
            Organization = "Org",
            Role = "Should Fail",
            StartDate = DateTime.UtcNow
        };

        var response = await AthleteClient.PostAsJsonAsync($"/api/v1/coach/{coach!.Id}/experience", request);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetExperience_Admin_ReturnsExperience()
    {
        var coach = await CreateTestCoachAsync();
        coach.Should().NotBeNull();
        await AdminClient.PostAsJsonAsync($"/api/v1/coach/{coach!.Id}/experience", new AddExperienceRequest
        {
            Organization = "MCA",
            Role = "Head Coach",
            StartDate = new DateTime(2018, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });

        var response = await AdminClient.GetAsync($"/api/v1/coach/{coach.Id}/experience");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<IReadOnlyList<ExperienceDto>>>();
        content.Should().NotBeNull();
        content!.Data.Should().NotBeNull();
        content.Data!.Should().HaveCount(1);
        content.Data![0].Role.Should().Be("Head Coach");
    }

    [Fact]
    public async Task UpdateExperience_Admin_UpdatesSuccessfully()
    {
        var coach = await CreateTestCoachAsync();
        coach.Should().NotBeNull();
        var expResponse = await AdminClient.PostAsJsonAsync($"/api/v1/coach/{coach!.Id}/experience", new AddExperienceRequest
        {
            Organization = "Old Org",
            Role = "Old Role",
            StartDate = DateTime.UtcNow
        });
        var expContent = await expResponse.Content.ReadFromJsonAsync<ApiResponse<ExperienceDto>>();
        var expId = expContent!.Data!.Id;

        var request = new UpdateExperienceRequest
        {
            Role = "Updated Role"
        };

        var response = await AdminClient.PutAsJsonAsync($"/api/v1/coach/{coach.Id}/experience/{expId}", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<ExperienceDto>>();
        content!.Data!.Role.Should().Be("Updated Role");
    }

    [Fact]
    public async Task DeleteExperience_Admin_DeletesSuccessfully()
    {
        var coach = await CreateTestCoachAsync();
        coach.Should().NotBeNull();
        var expResponse = await AdminClient.PostAsJsonAsync($"/api/v1/coach/{coach!.Id}/experience", new AddExperienceRequest
        {
            Organization = "To Delete",
            StartDate = DateTime.UtcNow
        });
        var expContent = await expResponse.Content.ReadFromJsonAsync<ApiResponse<ExperienceDto>>();
        var expId = expContent!.Data!.Id;

        var response = await AdminClient.DeleteAsync($"/api/v1/coach/{coach.Id}/experience/{expId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task AddEducation_Admin_AddsSuccessfully()
    {
        var coach = await CreateTestCoachAsync();
        coach.Should().NotBeNull();

        var request = new AddEducationRequest
        {
            Degree = "Masters in Sports Science",
            Institution = "University of Mumbai",
            FieldOfStudy = "Sports Coaching",
            YearCompleted = 2017
        };

        var response = await AdminClient.PostAsJsonAsync($"/api/v1/coach/{coach!.Id}/education", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<EducationDto>>();
        content.Should().NotBeNull();
        content!.Success.Should().BeTrue();
        content.Data!.Degree.Should().Be("Masters in Sports Science");
        content.Data.Institution.Should().Be("University of Mumbai");
    }

    [Fact]
    public async Task GetEducation_Admin_ReturnsEducation()
    {
        var coach = await CreateTestCoachAsync();
        coach.Should().NotBeNull();
        await AdminClient.PostAsJsonAsync($"/api/v1/coach/{coach!.Id}/education", new AddEducationRequest
        {
            Degree = "BPEd",
            Institution = "Delhi University",
            YearCompleted = 2015
        });

        var response = await AdminClient.GetAsync($"/api/v1/coach/{coach.Id}/education");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<IReadOnlyList<EducationDto>>>();
        content.Should().NotBeNull();
        content!.Data.Should().NotBeNull();
        content.Data!.Should().HaveCount(1);
    }

    [Fact]
    public async Task UpdateEducation_Admin_UpdatesSuccessfully()
    {
        var coach = await CreateTestCoachAsync();
        coach.Should().NotBeNull();
        var eduResponse = await AdminClient.PostAsJsonAsync($"/api/v1/coach/{coach!.Id}/education", new AddEducationRequest
        {
            Degree = "Old Degree",
            Institution = "Old Uni"
        });
        var eduContent = await eduResponse.Content.ReadFromJsonAsync<ApiResponse<EducationDto>>();
        var eduId = eduContent!.Data!.Id;

        var response = await AdminClient.PutAsJsonAsync($"/api/v1/coach/{coach.Id}/education/{eduId}", new UpdateEducationRequest
        {
            Degree = "New Degree"
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<EducationDto>>();
        content!.Data!.Degree.Should().Be("New Degree");
    }

    [Fact]
    public async Task DeleteEducation_Admin_DeletesSuccessfully()
    {
        var coach = await CreateTestCoachAsync();
        coach.Should().NotBeNull();
        var eduResponse = await AdminClient.PostAsJsonAsync($"/api/v1/coach/{coach!.Id}/education", new AddEducationRequest
        {
            Degree = "To Delete",
            Institution = "Uni"
        });
        var eduContent = await eduResponse.Content.ReadFromJsonAsync<ApiResponse<EducationDto>>();
        var eduId = eduContent!.Data!.Id;

        var response = await AdminClient.DeleteAsync($"/api/v1/coach/{coach.Id}/education/{eduId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateAvailability_Admin_UpdatesSuccessfully()
    {
        var coach = await CreateTestCoachAsync();
        coach.Should().NotBeNull();

        var request = new UpdateAvailabilityRequest
        {
            WeeklySchedule = "{\"Monday\":\"09:00-17:00\",\"Tuesday\":\"09:00-17:00\"}",
            TimeSlots = "[\"09:00-11:00\",\"14:00-16:00\"]",
            OnlineAvailable = true,
            OfflineAvailable = true,
            TravelDistance = 50
        };

        var response = await AdminClient.PutAsJsonAsync($"/api/v1/coach/{coach!.Id}/availability", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<AvailabilityDto>>();
        content.Should().NotBeNull();
        content!.Data.Should().NotBeNull();
        content.Data!.OnlineAvailable.Should().BeTrue();
        content.Data.OfflineAvailable.Should().BeTrue();
        content.Data.TravelDistance.Should().Be(50);
    }

    [Fact]
    public async Task GetAvailability_Admin_ReturnsAvailability()
    {
        var coach = await CreateTestCoachAsync();
        coach.Should().NotBeNull();
        await AdminClient.PutAsJsonAsync($"/api/v1/coach/{coach!.Id}/availability", new UpdateAvailabilityRequest
        {
            WeeklySchedule = "{\"Monday\":\"09:00-17:00\"}",
            OnlineAvailable = true
        });

        var response = await AdminClient.GetAsync($"/api/v1/coach/{coach.Id}/availability");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<AvailabilityDto>>();
        content.Should().NotBeNull();
        content!.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateLocation_Admin_UpdatesSuccessfully()
    {
        var coach = await CreateTestCoachAsync();
        coach.Should().NotBeNull();

        var request = new UpdateLocationRequest
        {
            City = "Mumbai",
            State = "Maharashtra",
            Country = "India",
            Latitude = 19.0760m,
            Longitude = 72.8777m
        };

        var response = await AdminClient.PutAsJsonAsync($"/api/v1/coach/{coach!.Id}/location", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<LocationDto>>();
        content.Should().NotBeNull();
        content!.Data.Should().NotBeNull();
        content.Data!.City.Should().Be("Mumbai");
        content.Data.State.Should().Be("Maharashtra");
    }

    [Fact]
    public async Task GetLocation_Admin_ReturnsLocation()
    {
        var coach = await CreateTestCoachAsync();
        coach.Should().NotBeNull();
        await AdminClient.PutAsJsonAsync($"/api/v1/coach/{coach!.Id}/location", new UpdateLocationRequest
        {
            City = "Delhi",
            State = "Delhi",
            Country = "India"
        });

        var response = await UnauthenticatedClient.GetAsync($"/api/v1/coach/{coach.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<CoachProfileDto>>();
        content.Should().NotBeNull();
        content!.Data.Should().NotBeNull();
        content.Data!.Location.Should().NotBeNull();
        content.Data.Location!.City.Should().Be("Delhi");
    }
}
