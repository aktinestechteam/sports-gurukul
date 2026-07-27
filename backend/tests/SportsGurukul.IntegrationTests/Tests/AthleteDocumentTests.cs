using System.Net;
using System.Net.Http.Json;
using System.Text;
using FluentAssertions;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.AthleteManagement.DTOs;
using SportsGurukul.Application.Features.DocumentManagement.DTOs;
using SportsGurukul.Domain.Enums;
using SportsGurukul.IntegrationTests.Bases;
using SportsGurukul.IntegrationTests.Fixtures;
using Xunit;

namespace SportsGurukul.IntegrationTests.Tests;

public class AthleteDocumentTests : AthleteIntegrationTestBase
{
    public AthleteDocumentTests(PostgresFixture postgresFixture) : base(postgresFixture) { }

    private MultipartFormDataContent CreateDocumentUploadContent(
        string title = "Test Document",
        string description = "Test description",
        string category = "IdentityDocument",
        string fileName = "test.pdf",
        string content = "test file content")
    {
        var fileContent = new ByteArrayContent(Encoding.UTF8.GetBytes(content));
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");

        var form = new MultipartFormDataContent();
        form.Add(fileContent, "file", fileName);
        form.Add(new StringContent(title), "Title");
        form.Add(new StringContent(description), "Description");
        form.Add(new StringContent(category), "Category");
        return form;
    }

    [Fact]
    public async Task UploadDocument_ValidFile_UploadsSuccessfully()
    {
        var athlete = await CreateTestAthleteAsync();

        using var form = CreateDocumentUploadContent();
        var response = await AdminClient.PostAsync($"/api/v1/athletes/{athlete!.Id}/documents", form);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<AthleteDocumentDto>>();
        content!.Success.Should().BeTrue();
        content.Data.Should().NotBeNull();
        content.Data!.Title.Should().Be("Test Document");
        content.Data.OriginalFileName.Should().Be("test.pdf");
        content.Data.MimeType.Should().Be("application/pdf");
    }

    [Fact]
    public async Task UploadDocument_NonExistentAthlete_ReturnsNotFound()
    {
        using var form = CreateDocumentUploadContent();
        var response = await AdminClient.PostAsync($"/api/v1/athletes/{Guid.NewGuid()}/documents", form);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetDocuments_HasDocuments_ReturnsList()
    {
        var athlete = await CreateTestAthleteAsync();
        using (var form = CreateDocumentUploadContent(title: "Doc 1"))
        {
            await AdminClient.PostAsync($"/api/v1/athletes/{athlete!.Id}/documents", form);
        }
        using (var form = CreateDocumentUploadContent(title: "Doc 2"))
        {
            await AdminClient.PostAsync($"/api/v1/athletes/{athlete.Id}/documents", form);
        }

        var response = await AdminClient.GetAsync($"/api/v1/athletes/{athlete.Id}/documents");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<IReadOnlyList<AthleteDocumentDto>>>();
        apiResponse!.Success.Should().BeTrue();
        apiResponse.Data.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetDocuments_NoDocuments_ReturnsEmptyList()
    {
        var athlete = await CreateTestAthleteAsync();

        var response = await AdminClient.GetAsync($"/api/v1/athletes/{athlete!.Id}/documents");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<IReadOnlyList<AthleteDocumentDto>>>();
        apiResponse!.Data.Should().BeEmpty();
    }

    [Fact]
    public async Task GetDocument_Exists_ReturnsDocument()
    {
        var athlete = await CreateTestAthleteAsync();
        using (var form = CreateDocumentUploadContent())
        {
            var uploadResponse = await AdminClient.PostAsync($"/api/v1/athletes/{athlete!.Id}/documents", form);
            var uploaded = await uploadResponse.Content.ReadFromJsonAsync<ApiResponse<AthleteDocumentDto>>();
            var documentId = uploaded!.Data!.Id;

            var response = await AdminClient.GetAsync($"/api/v1/athletes/{athlete.Id}/documents/{documentId}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadFromJsonAsync<ApiResponse<AthleteDocumentDto>>();
            content!.Success.Should().BeTrue();
            content.Data!.Id.Should().Be(documentId);
        }
    }

    [Fact]
    public async Task GetDocument_NotExists_ReturnsNotFound()
    {
        var athlete = await CreateTestAthleteAsync();

        var response = await AdminClient.GetAsync($"/api/v1/athletes/{athlete!.Id}/documents/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateDocumentMetadata_ValidRequest_UpdatesSuccessfully()
    {
        var athlete = await CreateTestAthleteAsync();
        Guid documentId;
        using (var form = CreateDocumentUploadContent())
        {
            var uploadResponse = await AdminClient.PostAsync($"/api/v1/athletes/{athlete!.Id}/documents", form);
            var uploaded = await uploadResponse.Content.ReadFromJsonAsync<ApiResponse<AthleteDocumentDto>>();
            documentId = uploaded!.Data!.Id;
        }

        var response = await AdminClient.PutAsJsonAsync(
            $"/api/v1/athletes/{athlete.Id}/documents/{documentId}/metadata", new
        {
            Title = "Updated Title",
            Description = "Updated description",
            Category = "MedicalCertificate"
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<AthleteDocumentDto>>();
        content!.Success.Should().BeTrue();
        content.Data!.Title.Should().Be("Updated Title");
        content.Data.Description.Should().Be("Updated description");
    }

    [Fact]
    public async Task DeleteDocument_Exists_DeletesSuccessfully()
    {
        var athlete = await CreateTestAthleteAsync();
        Guid documentId;
        using (var form = CreateDocumentUploadContent())
        {
            var uploadResponse = await AdminClient.PostAsync($"/api/v1/athletes/{athlete!.Id}/documents", form);
            var uploaded = await uploadResponse.Content.ReadFromJsonAsync<ApiResponse<AthleteDocumentDto>>();
            documentId = uploaded!.Data!.Id;
        }

        var response = await AdminClient.DeleteAsync($"/api/v1/athletes/{athlete.Id}/documents/{documentId}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await AdminClient.GetAsync($"/api/v1/athletes/{athlete.Id}/documents/{documentId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteDocument_NotExists_ReturnsNotFound()
    {
        var athlete = await CreateTestAthleteAsync();

        var response = await AdminClient.DeleteAsync($"/api/v1/athletes/{athlete!.Id}/documents/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task RestoreDocument_DeletedDocument_RestoresSuccessfully()
    {
        var athlete = await CreateTestAthleteAsync();
        Guid documentId;
        using (var form = CreateDocumentUploadContent())
        {
            var uploadResponse = await AdminClient.PostAsync($"/api/v1/athletes/{athlete!.Id}/documents", form);
            var uploaded = await uploadResponse.Content.ReadFromJsonAsync<ApiResponse<AthleteDocumentDto>>();
            documentId = uploaded!.Data!.Id;
        }

        await AdminClient.DeleteAsync($"/api/v1/athletes/{athlete.Id}/documents/{documentId}");

        var response = await AdminClient.PostAsync($"/api/v1/athletes/{athlete.Id}/documents/{documentId}/restore", null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var getResponse = await AdminClient.GetAsync($"/api/v1/athletes/{athlete.Id}/documents/{documentId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task VerifyDocument_ValidRequest_VerifiesSuccessfully()
    {
        var athlete = await CreateTestAthleteAsync();
        Guid documentId;
        using (var form = CreateDocumentUploadContent())
        {
            var uploadResponse = await AdminClient.PostAsync($"/api/v1/athletes/{athlete!.Id}/documents", form);
            var uploaded = await uploadResponse.Content.ReadFromJsonAsync<ApiResponse<AthleteDocumentDto>>();
            documentId = uploaded!.Data!.Id;
        }

        var response = await AdminClient.PostAsJsonAsync(
            $"/api/v1/athletes/{athlete.Id}/documents/{documentId}/verify", new
        {
            Status = "Verified",
            Notes = "Document verified successfully"
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DownloadDocument_Exists_ReturnsFile()
    {
        var athlete = await CreateTestAthleteAsync();
        Guid documentId;
        using (var form = CreateDocumentUploadContent())
        {
            var uploadResponse = await AdminClient.PostAsync($"/api/v1/athletes/{athlete!.Id}/documents", form);
            var uploaded = await uploadResponse.Content.ReadFromJsonAsync<ApiResponse<AthleteDocumentDto>>();
            documentId = uploaded!.Data!.Id;
        }

        var response = await AdminClient.GetAsync($"/api/v1/athletes/{athlete.Id}/documents/{documentId}/download");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType.Should().NotBeNull();
    }

    [Fact]
    public async Task DownloadDocument_NotExists_ReturnsNotFound()
    {
        var athlete = await CreateTestAthleteAsync();

        var response = await AdminClient.GetAsync($"/api/v1/athletes/{athlete!.Id}/documents/{Guid.NewGuid()}/download");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AthleteRole_CannotUploadDocument()
    {
        var athlete = await CreateTestAthleteAsync();
        using var form = CreateDocumentUploadContent();

        var response = await AthleteClient.PostAsync($"/api/v1/athletes/{athlete!.Id}/documents", form);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Unauthenticated_CannotUploadDocument()
    {
        using var form = CreateDocumentUploadContent();

        var response = await UnauthenticatedClient.PostAsync($"/api/v1/athletes/{Guid.NewGuid()}/documents", form);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}