using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SportsGurukul.Finance.IntegrationTests.Fixtures;
using SportsGurukul.Finance.IntegrationTests.Seed;
using Xunit;

namespace SportsGurukul.Finance.IntegrationTests.Helpers;

[Collection("Finance")]
public abstract class FinanceTestBase : IAsyncLifetime
{
    protected FinanceWebApplicationFactory Factory { get; }
    protected IServiceScope ServiceScope { get; private set; } = null!;
    protected IMediator Mediator { get; private set; } = null!;
    protected HttpClient AdminClient { get; private set; } = null!;
    protected HttpClient AcademyClient { get; private set; } = null!;
    protected HttpClient CoachClient { get; private set; } = null!;
    protected HttpClient AthleteClient { get; private set; } = null!;
    protected HttpClient AnonymousClient { get; private set; } = null!;

    protected static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    protected FinanceTestBase(FinanceWebApplicationFactory factory)
    {
        Factory = factory;
    }

    public virtual async Task InitializeAsync()
    {
        ServiceScope = Factory.Services.CreateScope();
        Mediator = ServiceScope.ServiceProvider.GetRequiredService<IMediator>();

        var seeder = new FinanceDataSeeder(
            ServiceScope.ServiceProvider.GetRequiredService<Infrastructure.Persistence.ApplicationDbContext>());
        await seeder.SeedAsync();

        AdminClient = CreateAuthenticatedClient(FinanceTestIds.AdminUserId, FinanceTestIds.AdminEmail, "Admin");
        AcademyClient = CreateAuthenticatedClient(FinanceTestIds.AcademyUserId, FinanceTestIds.AcademyEmail, "Academy Admin");
        CoachClient = CreateAuthenticatedClient(FinanceTestIds.CoachUserId, FinanceTestIds.CoachEmail, "Coach");
        AthleteClient = CreateAuthenticatedClient(FinanceTestIds.AthleteUserId, FinanceTestIds.AthleteEmail, "Athlete");
        AnonymousClient = Factory.CreateClient();
    }

    public virtual Task DisposeAsync()
    {
        ServiceScope?.Dispose();
        return Task.CompletedTask;
    }

    private HttpClient CreateAuthenticatedClient(Guid userId, string email, string role)
    {
        var client = Factory.CreateClient();
        var token = JwtTokenHelper.GenerateToken(userId, email, role);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    protected async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
    {
        return await Mediator.Send(request);
    }

    protected async Task<HttpResponseMessage> PostAsJsonAsync<T>(HttpClient client, string url, T body)
    {
        return await client.PostAsJsonAsync(url, body, JsonOptions);
    }

    protected async Task<HttpResponseMessage> PutAsJsonAsync<T>(HttpClient client, string url, T body)
    {
        return await client.PutAsJsonAsync(url, body, JsonOptions);
    }

    protected async Task<HttpResponseMessage> GetAsync(HttpClient client, string url)
    {
        return await client.GetAsync(url);
    }

    protected async Task<HttpResponseMessage> DeleteAsync(HttpClient client, string url)
    {
        return await client.DeleteAsync(url);
    }
}
