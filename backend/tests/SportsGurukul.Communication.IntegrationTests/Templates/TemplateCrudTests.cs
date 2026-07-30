using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.Commands.Template;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Communication.IntegrationTests.Templates;

public class TemplateCrudTests : CommunicationTestBase
{
    public TemplateCrudTests(CommunicationTestApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task CreateTemplate_WithValidData_Returns201()
    {
        var client = CreateAuthenticatedClient("Admin");
        var command = new CreateTemplateCommand(
            "Welcome Email", "Welcome notification template",
            NotificationChannelType.Email,
            "Welcome {{name}}!", "<h1>Hello {{name}}</h1>",
            new List<CreateTemplateVariableRequest>
            {
                new("name", "User's name", true, null, "string")
            });

        var response = await PostJsonAsync(client, "/api/v1/templates", command);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<TemplateDto>>();
        content.Should().NotBeNull();
        content!.Success.Should().BeTrue();
        content.Data.Should().NotBeNull();
        content.Data!.Name.Should().Be("Welcome Email");
        content.Data.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task CreateTemplate_DuplicateName_Returns409()
    {
        var client = CreateAuthenticatedClient("Admin");
        var command = new CreateTemplateCommand(
            "Unique Template", null,
            NotificationChannelType.Email,
            "Subject", "<p>Body</p>", null);

        await PostJsonAsync(client, "/api/v1/templates", command);

        var response = await PostJsonAsync(client, "/api/v1/templates", command);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task GetTemplateById_WithExistingId_Returns200()
    {
        var client = CreateAuthenticatedClient("Admin");
        var createCmd = new CreateTemplateCommand(
            "Get Test Template", null,
            NotificationChannelType.Email,
            "Subject", "<p>Body</p>", null);

        var createResponse = await PostJsonAsync(client, "/api/v1/templates", createCmd);
        var created = await createResponse.Content.ReadFromJsonAsync<ApiResponse<TemplateDto>>();

        var getResponse = await GetAsync(client, $"/api/v1/templates/{created!.Data!.Id}");

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await getResponse.Content.ReadFromJsonAsync<ApiResponse<TemplateDto>>();
        content!.Data!.Id.Should().Be(created.Data.Id);
    }

    [Fact]
    public async Task GetTemplateById_NonExistent_Returns404()
    {
        var client = CreateAuthenticatedClient("Admin");

        var response = await GetAsync(client, $"/api/v1/templates/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateTemplate_WithValidData_Returns200()
    {
        var client = CreateAuthenticatedClient("Admin");
        var createCmd = new CreateTemplateCommand(
            "Update Test", null,
            NotificationChannelType.Email,
            "Original Subject", "<p>Original</p>", null);

        var createResponse = await PostJsonAsync(client, "/api/v1/templates", createCmd);
        var created = await createResponse.Content.ReadFromJsonAsync<ApiResponse<TemplateDto>>();

        var updateCmd = new UpdateTemplateCommand(
            created!.Data!.Id, "Updated Name", "Updated desc",
            "Updated Subject", "<p>Updated</p>", null);

        var updateResponse = await PutJsonAsync(client, $"/api/v1/templates/{created.Data.Id}", updateCmd);

        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await updateResponse.Content.ReadFromJsonAsync<ApiResponse<TemplateDto>>();
        content!.Data!.Name.Should().Be("Updated Name");
    }

    [Fact]
    public async Task PublishTemplate_CreatesNewVersion()
    {
        var client = CreateAuthenticatedClient("Admin");
        var createCmd = new CreateTemplateCommand(
            "Publish Test", null,
            NotificationChannelType.Email,
            "Subject", "<p>Body</p>", null);

        var createResponse = await PostJsonAsync(client, "/api/v1/templates", createCmd);
        var created = await createResponse.Content.ReadFromJsonAsync<ApiResponse<TemplateDto>>();

        var publishResponse = await PostJsonAsync(client, $"/api/v1/templates/{created!.Data!.Id}/publish", new { });

        publishResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await publishResponse.Content.ReadFromJsonAsync<ApiResponse<TemplateDto>>();
        content!.Data!.CurrentVersion.Should().BeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task ArchiveTemplate_Deactivates()
    {
        var client = CreateAuthenticatedClient("SuperAdmin");
        var createCmd = new CreateTemplateCommand(
            "Archive Test", null,
            NotificationChannelType.Email,
            "Subject", "<p>Body</p>", null);

        var createResponse = await PostJsonAsync(client, "/api/v1/templates", createCmd);
        var created = await createResponse.Content.ReadFromJsonAsync<ApiResponse<TemplateDto>>();

        var archiveResponse = await DeleteAsync(client, $"/api/v1/templates/{created!.Data!.Id}");

        archiveResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetVersions_WithPublishedTemplate_ReturnsVersions()
    {
        var client = CreateAuthenticatedClient("Admin");
        var createCmd = new CreateTemplateCommand(
            "Versions Test", null,
            NotificationChannelType.Email,
            "Subject", "<p>Body</p>", null);

        var createResponse = await PostJsonAsync(client, "/api/v1/templates", createCmd);
        var created = await createResponse.Content.ReadFromJsonAsync<ApiResponse<TemplateDto>>();

        await PostJsonAsync(client, $"/api/v1/templates/{created!.Data!.Id}/publish", new { });

        var versionsResponse = await GetAsync(client, $"/api/v1/templates/{created.Data.Id}/versions");

        versionsResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await versionsResponse.Content.ReadFromJsonAsync<ApiResponse<List<TemplateVersionDto>>>();
        content!.Data.Should().NotBeNull();
        content.Data!.Count.Should().BeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task CreateTemplate_AsNonAdmin_Returns403()
    {
        var client = CreateAuthenticatedClient("Athlete");
        var command = new CreateTemplateCommand(
            "Athlete Template", null,
            NotificationChannelType.Email,
            "Subject", "<p>Body</p>", null);

        var response = await PostJsonAsync(client, "/api/v1/templates", command);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
