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

public class CoachDocumentIntegrationTests : CoachIntegrationTestBase
{
    public CoachDocumentIntegrationTests(PostgresFixture postgresFixture) : base(postgresFixture) { }

    private async Task<(Guid docId, Guid coachId)> UploadTestDocumentAsync(HttpClient? client = null, Guid? coachId = null)
    {
        var targetClient = client ?? AdminClient;
        var targetCoachId = coachId ?? (await CreateTestCoachAsync())!.Id;

        var fileContent = new StringContent("fake certificate content");
        var formData = new MultipartFormDataContent();
        formData.Add(fileContent, "File", "certificate.pdf");
        formData.Add(new StringContent("BCCI Certificate"), "Title");
        formData.Add(new StringContent(((int)CoachDocumentCategory.CoachingCertification).ToString()), "Category");
        formData.Add(new StringContent("Test description"), "Description");
        formData.Add(new StringContent("false"), "IsPublic");

        var response = await targetClient.PostAsync($"/api/v1/coaches/{targetCoachId}/documents", formData);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<CoachDocumentDto>>();
        return (content!.Data!.Id, targetCoachId);
    }

    [Fact]
    public async Task UploadDocument_Admin_UploadsSuccessfully()
    {
        var coach = await CreateTestCoachAsync();
        coach.Should().NotBeNull();

        var fileContent = new StringContent("fake certificate content");
        var formData = new MultipartFormDataContent();
        formData.Add(fileContent, "File", "certificate.pdf");
        formData.Add(new StringContent("BCCI Certificate"), "Title");
        formData.Add(new StringContent(((int)CoachDocumentCategory.CoachingCertification).ToString()), "Category");
        formData.Add(new StringContent("Test description"), "Description");
        formData.Add(new StringContent("true"), "IsPublic");

        var response = await AdminClient.PostAsync($"/api/v1/coaches/{coach!.Id}/documents", formData);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<CoachDocumentDto>>();
        content.Should().NotBeNull();
        content!.Success.Should().BeTrue();
        content.Data.Should().NotBeNull();
        content.Data!.Title.Should().Be("BCCI Certificate");
        content.Data.OriginalFileName.Should().Be("certificate.pdf");
        content.Data.IsPublic.Should().BeTrue();
    }

    [Fact]
    public async Task UploadDocument_CoachOwner_UploadsSuccessfully()
    {
        var coach = await CreateTestCoachAsync(SeedData.CoachUserId);
        coach.Should().NotBeNull();

        var fileContent = new StringContent("fake content");
        var formData = new MultipartFormDataContent();
        formData.Add(fileContent, "File", "doc.pdf");
        formData.Add(new StringContent("My Certificate"), "Title");
        formData.Add(new StringContent(((int)CoachDocumentCategory.CoachingCertification).ToString()), "Category");

        var response = await CoachClient.PostAsync($"/api/v1/coaches/{coach!.Id}/documents", formData);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task UploadDocument_AthleteRole_ReturnsForbidden()
    {
        var coach = await CreateTestCoachAsync();
        coach.Should().NotBeNull();

        var fileContent = new StringContent("fake content");
        var formData = new MultipartFormDataContent();
        formData.Add(fileContent, "File", "doc.pdf");
        formData.Add(new StringContent("Test"), "Title");
        formData.Add(new StringContent(((int)CoachDocumentCategory.CoachingCertification).ToString()), "Category");

        var response = await AthleteClient.PostAsync($"/api/v1/coaches/{coach!.Id}/documents", formData);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetDocuments_Admin_ReturnsDocuments()
    {
        var (docId, coachId) = await UploadTestDocumentAsync();

        var response = await AdminClient.GetAsync($"/api/v1/coaches/{coachId}/documents");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<IReadOnlyList<CoachDocumentDto>>>();
        content.Should().NotBeNull();
        content!.Data.Should().NotBeNull();
        content.Data!.Count.Should().BeGreaterOrEqualTo(1);
    }

    [Fact]
    public async Task GetDocument_Admin_ReturnsDocument()
    {
        var (docId, _) = await UploadTestDocumentAsync();

        var response = await AdminClient.GetAsync($"/api/v1/coach-documents/{docId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<CoachDocumentDto>>();
        content.Should().NotBeNull();
        content!.Data.Should().NotBeNull();
        content.Data!.Id.Should().Be(docId);
    }

    [Fact]
    public async Task GetDocument_NonExistent_ReturnsNotFound()
    {
        var response = await AdminClient.GetAsync($"/api/v1/coach-documents/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateDocument_Admin_UpdatesSuccessfully()
    {
        var (docId, _) = await UploadTestDocumentAsync();

        var response = await AdminClient.PutAsJsonAsync($"/api/v1/coach-documents/{docId}", new UpdateCoachDocumentMetadataRequest
        {
            Title = "Updated Name"
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<CoachDocumentDto>>();
        content!.Data!.Title.Should().Be("Updated Name");
    }

    [Fact]
    public async Task VerifyDocument_Admin_VerifiesSuccessfully()
    {
        var (docId, _) = await UploadTestDocumentAsync();

        var response = await AdminClient.PostAsJsonAsync<object?>($"/api/v1/coach-documents/{docId}/verify", null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task RejectDocument_Admin_RejectsSuccessfully()
    {
        var (docId, _) = await UploadTestDocumentAsync();

        var response = await AdminClient.PostAsJsonAsync($"/api/v1/coach-documents/{docId}/reject", new RejectCoachDocumentRequest
        {
            Reason = "Document is blurry"
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeleteDocument_Admin_DeletesSuccessfully()
    {
        var (docId, _) = await UploadTestDocumentAsync();

        var response = await AdminClient.DeleteAsync($"/api/v1/coach-documents/{docId}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task RestoreDocument_Admin_RestoresSuccessfully()
    {
        var (docId, _) = await UploadTestDocumentAsync();
        await AdminClient.DeleteAsync($"/api/v1/coach-documents/{docId}");

        var response = await AdminClient.PostAsJsonAsync<object?>($"/api/v1/coach-documents/{docId}/restore", null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
