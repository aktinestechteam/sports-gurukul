using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.AthleteManagement.DTOs;
using SportsGurukul.Domain.Enums;
using SportsGurukul.IntegrationTests.Bases;
using SportsGurukul.IntegrationTests.Fixtures;

namespace SportsGurukul.IntegrationTests.Tests;

public class AthleteMedicalEmergencyTests : AthleteIntegrationTestBase
{
    public AthleteMedicalEmergencyTests(PostgresFixture postgresFixture) : base(postgresFixture) { }

    #region Medical Profile

    [Fact]
    public async Task UpdateMedicalProfile_ValidRequest_CreatesProfile()
    {
        var athlete = await CreateTestAthleteAsync();

        var response = await AdminClient.PutAsJsonAsync($"/api/v1/athletes/{athlete!.Id}/medical-profile", new
        {
            MedicalConditions = "None",
            Allergies = "Peanuts",
            Medications = "None",
            BloodGroup = "O+",
            InsuranceNumber = "INS-12345",
            DoctorName = "Dr. Smith",
            DoctorContact = "+919876543210"
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<MedicalProfileDto>>();
        content!.Success.Should().BeTrue();
        content.Data.Should().NotBeNull();
        content.Data!.Allergies.Should().Be("Peanuts");
        content.Data.InsuranceNumber.Should().Be("INS-12345");
        content.Data.DoctorName.Should().Be("Dr. Smith");
    }

    [Fact]
    public async Task UpdateMedicalProfile_ExistingProfile_UpdatesSuccessfully()
    {
        var athlete = await CreateTestAthleteAsync();
        await AdminClient.PutAsJsonAsync($"/api/v1/athletes/{athlete!.Id}/medical-profile", new
        {
            MedicalConditions = "Asthma",
            Allergies = "None",
            BloodGroup = "A+"
        });

        var response = await AdminClient.PutAsJsonAsync($"/api/v1/athletes/{athlete.Id}/medical-profile", new
        {
            MedicalConditions = "Asthma (controlled)",
            Medications = "Inhaler"
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<MedicalProfileDto>>();
        content!.Success.Should().BeTrue();
        content.Data!.MedicalConditions.Should().Be("Asthma (controlled)");
        content.Data.Medications.Should().Be("Inhaler");
        content.Data.Allergies.Should().Be("None");
    }

    [Fact]
    public async Task GetMedicalProfile_Exists_ReturnsProfile()
    {
        var athlete = await CreateTestAthleteAsync();
        await AdminClient.PutAsJsonAsync($"/api/v1/athletes/{athlete!.Id}/medical-profile", new
        {
            MedicalConditions = "Diabetes",
            BloodGroup = "B+"
        });

        var response = await AdminClient.GetAsync($"/api/v1/athletes/{athlete.Id}/medical-profile");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<MedicalProfileDto>>();
        content!.Success.Should().BeTrue();
        content.Data!.MedicalConditions.Should().Be("Diabetes");
        content.Data.BloodGroup.Should().Be("B+");
    }

    [Fact]
    public async Task GetMedicalProfile_NotExists_ReturnsNotFound()
    {
        var athlete = await CreateTestAthleteAsync();

        var response = await AdminClient.GetAsync($"/api/v1/athletes/{athlete!.Id}/medical-profile");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetMedicalProfile_NonExistentAthlete_ReturnsNotFound()
    {
        var response = await AdminClient.GetAsync($"/api/v1/athletes/{Guid.NewGuid()}/medical-profile");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Emergency Contact

    [Fact]
    public async Task UpdateEmergencyContact_ValidRequest_CreatesContact()
    {
        var athlete = await CreateTestAthleteAsync();

        var response = await AdminClient.PutAsJsonAsync($"/api/v1/athletes/{athlete!.Id}/emergency-contact", new
        {
            Name = "John Doe",
            Relationship = EmergencyRelationship.Parent,
            Phone = "+919876543210",
            Email = "john@example.com"
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<EmergencyContactDto>>();
        content!.Success.Should().BeTrue();
        content.Data.Should().NotBeNull();
        content.Data!.Name.Should().Be("John Doe");
        content.Data.Relationship.Should().Be("Parent");
        content.Data.Phone.Should().Be("+919876543210");
        content.Data.Email.Should().Be("john@example.com");
    }

    [Fact]
    public async Task UpdateEmergencyContact_ExistingContact_UpdatesSuccessfully()
    {
        var athlete = await CreateTestAthleteAsync();
        await AdminClient.PutAsJsonAsync($"/api/v1/athletes/{athlete!.Id}/emergency-contact", new
        {
            Name = "Original Name",
            Relationship = EmergencyRelationship.Parent,
            Phone = "+919876543210"
        });

        var response = await AdminClient.PutAsJsonAsync($"/api/v1/athletes/{athlete.Id}/emergency-contact", new
        {
            Name = "Updated Name",
            Relationship = EmergencyRelationship.Coach,
            Phone = "+919876543211"
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<EmergencyContactDto>>();
        content!.Success.Should().BeTrue();
        content.Data!.Name.Should().Be("Updated Name");
        content.Data.Relationship.Should().Be("Coach");
        content.Data.Phone.Should().Be("+919876543211");
    }

    [Fact]
    public async Task GetEmergencyContact_Exists_ReturnsContact()
    {
        var athlete = await CreateTestAthleteAsync();
        await AdminClient.PutAsJsonAsync($"/api/v1/athletes/{athlete!.Id}/emergency-contact", new
        {
            Name = "Jane Doe",
            Relationship = EmergencyRelationship.Sibling,
            Phone = "+919876543210"
        });

        var response = await AdminClient.GetAsync($"/api/v1/athletes/{athlete.Id}/emergency-contact");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<EmergencyContactDto>>();
        content!.Success.Should().BeTrue();
        content.Data!.Name.Should().Be("Jane Doe");
        content.Data.Relationship.Should().Be("Sibling");
    }

    [Fact]
    public async Task GetEmergencyContact_NotExists_ReturnsNotFound()
    {
        var athlete = await CreateTestAthleteAsync();

        var response = await AdminClient.GetAsync($"/api/v1/athletes/{athlete!.Id}/emergency-contact");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetEmergencyContact_NonExistentAthlete_ReturnsNotFound()
    {
        var response = await AdminClient.GetAsync($"/api/v1/athletes/{Guid.NewGuid()}/emergency-contact");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Ranking

    [Fact]
    public async Task UpdateRanking_ValidRequest_CreatesRanking()
    {
        var athlete = await CreateTestAthleteAsync();

        var response = await AdminClient.PutAsJsonAsync($"/api/v1/athletes/{athlete!.Id}/ranking", new
        {
            CurrentRank = "10",
            StateRank = "5",
            NationalRank = "200",
            InternationalRank = "1000",
            RankingAuthority = "National Sports Federation"
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<RankingDto>>();
        content!.Success.Should().BeTrue();
        content.Data.Should().NotBeNull();
        content.Data!.CurrentRank.Should().Be("10");
        content.Data.StateRank.Should().Be("5");
        content.Data.NationalRank.Should().Be("200");
        content.Data.InternationalRank.Should().Be("1000");
        content.Data.RankingAuthority.Should().Be("National Sports Federation");
    }

    [Fact]
    public async Task UpdateRanking_ExistingRanking_UpdatesSuccessfully()
    {
        var athlete = await CreateTestAthleteAsync();
        await AdminClient.PutAsJsonAsync($"/api/v1/athletes/{athlete!.Id}/ranking", new
        {
            CurrentRank = "20",
            StateRank = "10"
        });

        var response = await AdminClient.PutAsJsonAsync($"/api/v1/athletes/{athlete.Id}/ranking", new
        {
            CurrentRank = "5",
            NationalRank = "150"
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<RankingDto>>();
        content!.Success.Should().BeTrue();
        content.Data!.CurrentRank.Should().Be("5");
        content.Data.StateRank.Should().Be("10");
        content.Data.NationalRank.Should().Be("150");
    }

    [Fact]
    public async Task GetRanking_Exists_ReturnsRanking()
    {
        var athlete = await CreateTestAthleteAsync();
        await AdminClient.PutAsJsonAsync($"/api/v1/athletes/{athlete!.Id}/ranking", new
        {
            CurrentRank = "15",
            StateRank = "8"
        });

        var response = await AdminClient.GetAsync($"/api/v1/athletes/{athlete.Id}/ranking");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<RankingDto>>();
        content!.Success.Should().BeTrue();
        content.Data!.CurrentRank.Should().Be("15");
    }

    [Fact]
    public async Task GetRanking_NotExists_ReturnsNotFound()
    {
        var athlete = await CreateTestAthleteAsync();

        var response = await AdminClient.GetAsync($"/api/v1/athletes/{athlete!.Id}/ranking");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion
}

