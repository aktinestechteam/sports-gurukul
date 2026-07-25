using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.DTOs;
using SportsGurukul.Domain.Enums;
using SportsGurukul.IntegrationTests.Bases;
using SportsGurukul.IntegrationTests.Fixtures;

namespace SportsGurukul.IntegrationTests.Tests;

public class CoachSportsCertificationsTests : CoachIntegrationTestBase
{
    public CoachSportsCertificationsTests(PostgresFixture postgresFixture) : base(postgresFixture) { }

    [Fact]
    public async Task AssignSport_Admin_AssignsSuccessfully()
    {
        var coach = await CreateTestCoachAsync();
        coach.Should().NotBeNull();

        var request = new CoachAssignSportRequest { SportId = SeedData.CricketSportId, IsPrimarySport = true };
        var response = await AdminClient.PostAsJsonAsync($"/api/v1/coach/{coach!.Id}/sports", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<SportDto>>();
        content.Should().NotBeNull();
        content!.Success.Should().BeTrue();
        content.Data.Should().NotBeNull();
        content.Data!.SportId.Should().Be(SeedData.CricketSportId);
        content.Data.Name.Should().Be("Cricket");
    }

    [Fact]
    public async Task AssignSport_CoachOwner_AssignsSuccessfully()
    {
        var coach = await CreateTestCoachAsync(SeedData.CoachUserId);
        coach.Should().NotBeNull();

        var request = new CoachAssignSportRequest { SportId = SeedData.FootballSportId, IsPrimarySport = false };
        var response = await CoachClient.PostAsJsonAsync($"/api/v1/coach/{coach!.Id}/sports", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task AssignSport_AthleteRole_ReturnsForbidden()
    {
        var coach = await CreateTestCoachAsync();
        coach.Should().NotBeNull();

        var request = new CoachAssignSportRequest { SportId = SeedData.TennisSportId };
        var response = await AthleteClient.PostAsJsonAsync($"/api/v1/coach/{coach!.Id}/sports", request);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetSports_Admin_ReturnsSports()
    {
        var coach = await CreateTestCoachAsync();
        coach.Should().NotBeNull();
        await AdminClient.PostAsJsonAsync($"/api/v1/coach/{coach!.Id}/sports", new CoachAssignSportRequest { SportId = SeedData.CricketSportId, IsPrimarySport = true });
        await AdminClient.PostAsJsonAsync($"/api/v1/coach/{coach.Id}/sports", new CoachAssignSportRequest { SportId = SeedData.FootballSportId });

        var response = await AdminClient.GetAsync($"/api/v1/coach/{coach.Id}/sports");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<IReadOnlyList<SportDto>>>();
        content.Should().NotBeNull();
        content!.Data.Should().NotBeNull();
        content.Data!.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetSports_CoachOwner_ReturnsSports()
    {
        var coach = await CreateTestCoachAsync(SeedData.CoachUserId);
        coach.Should().NotBeNull();
        await CoachClient.PostAsJsonAsync($"/api/v1/coach/{coach!.Id}/sports", new CoachAssignSportRequest { SportId = SeedData.CricketSportId });

        var response = await CoachClient.GetAsync($"/api/v1/coach/{coach.Id}/sports");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task RemoveSport_Admin_RemovesSuccessfully()
    {
        var coach = await CreateTestCoachAsync();
        coach.Should().NotBeNull();
        await AdminClient.PostAsJsonAsync($"/api/v1/coach/{coach!.Id}/sports", new CoachAssignSportRequest { SportId = SeedData.CricketSportId });

        var response = await AdminClient.DeleteAsync($"/api/v1/coach/{coach.Id}/sports/{SeedData.CricketSportId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task RemoveSport_NonExistent_ReturnsNotFound()
    {
        var coach = await CreateTestCoachAsync();
        coach.Should().NotBeNull();

        var response = await AdminClient.DeleteAsync($"/api/v1/coach/{coach!.Id}/sports/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AddCertification_Admin_AddsSuccessfully()
    {
        var coach = await CreateTestCoachAsync();
        coach.Should().NotBeNull();

        var request = new AddCertificationRequest
        {
            CertificationName = "BCCI Level A",
            IssuingAuthority = "BCCI",
            IssueDate = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            ExpiryDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            CertificateNumber = "BCCI-001"
        };

        var response = await AdminClient.PostAsJsonAsync($"/api/v1/coach/{coach!.Id}/certifications", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<CertificationDto>>();
        content.Should().NotBeNull();
        content!.Success.Should().BeTrue();
        content.Data.Should().NotBeNull();
        content.Data!.CertificationName.Should().Be("BCCI Level A");
        content.Data.IssuingAuthority.Should().Be("BCCI");
    }

    [Fact]
    public async Task AddCertification_CoachOwner_AddsSuccessfully()
    {
        var coach = await CreateTestCoachAsync(SeedData.CoachUserId);
        coach.Should().NotBeNull();

        var request = new AddCertificationRequest
        {
            CertificationName = "UEFA Pro License",
            IssuingAuthority = "UEFA",
            IssueDate = new DateTime(2019, 6, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        var response = await CoachClient.PostAsJsonAsync($"/api/v1/coach/{coach!.Id}/certifications", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task AddCertification_AthleteRole_ReturnsForbidden()
    {
        var coach = await CreateTestCoachAsync();
        coach.Should().NotBeNull();

        var request = new AddCertificationRequest
        {
            CertificationName = "Should Fail",
            IssuingAuthority = "Org",
            IssueDate = DateTime.UtcNow
        };

        var response = await AthleteClient.PostAsJsonAsync($"/api/v1/coach/{coach!.Id}/certifications", request);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetCertifications_Admin_ReturnsCertifications()
    {
        var coach = await CreateTestCoachAsync();
        coach.Should().NotBeNull();
        await AdminClient.PostAsJsonAsync($"/api/v1/coach/{coach!.Id}/certifications", new AddCertificationRequest
        {
            CertificationName = "BCCI Level A",
            IssuingAuthority = "BCCI",
            IssueDate = DateTime.UtcNow
        });

        var response = await AdminClient.GetAsync($"/api/v1/coach/{coach.Id}/certifications");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<IReadOnlyList<CertificationDto>>>();
        content.Should().NotBeNull();
        content!.Data.Should().NotBeNull();
        content.Data!.Should().HaveCount(1);
    }

    [Fact]
    public async Task UpdateCertification_Admin_UpdatesSuccessfully()
    {
        var coach = await CreateTestCoachAsync();
        coach.Should().NotBeNull();
        var certResponse = await AdminClient.PostAsJsonAsync($"/api/v1/coach/{coach!.Id}/certifications", new AddCertificationRequest
        {
            CertificationName = "Old Cert",
            IssuingAuthority = "Old Org",
            IssueDate = DateTime.UtcNow
        });
        var certContent = await certResponse.Content.ReadFromJsonAsync<ApiResponse<CertificationDto>>();
        var certId = certContent!.Data!.Id;

        var request = new UpdateCertificationRequest
        {
            CertificationName = "Updated Cert",
            IssuingAuthority = "New Org"
        };

        var response = await AdminClient.PutAsJsonAsync($"/api/v1/coach/{coach.Id}/certifications/{certId}", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<CertificationDto>>();
        content!.Data!.CertificationName.Should().Be("Updated Cert");
        content.Data.IssuingAuthority.Should().Be("New Org");
    }

    [Fact]
    public async Task DeleteCertification_Admin_DeletesSuccessfully()
    {
        var coach = await CreateTestCoachAsync();
        coach.Should().NotBeNull();
        var certResponse = await AdminClient.PostAsJsonAsync($"/api/v1/coach/{coach!.Id}/certifications", new AddCertificationRequest
        {
            CertificationName = "To Delete",
            IssuingAuthority = "Org",
            IssueDate = DateTime.UtcNow
        });
        var certContent = await certResponse.Content.ReadFromJsonAsync<ApiResponse<CertificationDto>>();
        var certId = certContent!.Data!.Id;

        var response = await AdminClient.DeleteAsync($"/api/v1/coach/{coach.Id}/certifications/{certId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task VerifyCertification_Admin_VerifiesSuccessfully()
    {
        var coach = await CreateTestCoachAsync();
        coach.Should().NotBeNull();
        var certResponse = await AdminClient.PostAsJsonAsync($"/api/v1/coach/{coach!.Id}/certifications", new AddCertificationRequest
        {
            CertificationName = "To Verify",
            IssuingAuthority = "Org",
            IssueDate = DateTime.UtcNow
        });
        var certContent = await certResponse.Content.ReadFromJsonAsync<ApiResponse<CertificationDto>>();
        var certId = certContent!.Data!.Id;

        var response = await AdminClient.PostAsJsonAsync($"/api/v1/coach/{coach.Id}/certifications/{certId}/verify", new VerifyCertificationRequest
        {
            Status = VerificationStatus.Verified
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
