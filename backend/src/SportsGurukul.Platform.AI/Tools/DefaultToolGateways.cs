using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.AI.Interfaces.Tools;

namespace SportsGurukul.Platform.AI.Tools;

public class StubInternalApiGateway : IInternalApiGateway
{
    private readonly ILogger<StubInternalApiGateway> _logger;

    public StubInternalApiGateway(ILogger<StubInternalApiGateway>? logger = null)
    {
        _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<StubInternalApiGateway>.Instance;
    }

    public Task<object?> CallAsync(string operation, IDictionary<string, object?> arguments, CancellationToken cancellationToken = default)
    {
        _logger.LogWarning("Internal API gateway is not configured; stubbed call '{Operation}'.", operation);
        return Task.FromResult<object?>(new Dictionary<string, object?>
        {
            ["operation"] = operation,
            ["stubbed"] = true,
            ["echo"] = arguments
        });
    }
}

public class DefaultRestApiClient : IRestApiClient
{
    private readonly HttpClient _http;
    private readonly ILogger<DefaultRestApiClient> _logger;

    public DefaultRestApiClient(HttpClient? http = null, ILogger<DefaultRestApiClient>? logger = null)
    {
        _http = http ?? new HttpClient();
        _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<DefaultRestApiClient>.Instance;
    }

    public async Task<object?> CallAsync(Uri endpoint, string method, IDictionary<string, object?>? body, IDictionary<string, string?>? headers, CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(new HttpMethod(method.ToUpperInvariant()), endpoint);

        if (headers is not null)
        {
            foreach (var (key, value) in headers)
            {
                request.Headers.TryAddWithoutValidation(key, value);
            }
        }

        if (body is not null)
        {
            request.Content = JsonContent.Create(body);
        }

        using var response = await _http.SendAsync(request, cancellationToken);
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        response.EnsureSuccessStatusCode();
        return content;
    }
}

public class StubDatabaseQueryExecutor : IDatabaseQueryExecutor
{
    private readonly ILogger<StubDatabaseQueryExecutor> _logger;

    public StubDatabaseQueryExecutor(ILogger<StubDatabaseQueryExecutor>? logger = null)
    {
        _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<StubDatabaseQueryExecutor>.Instance;
    }

    public Task<object?> ExecuteAsync(string statement, IDictionary<string, object?>? parameters, CancellationToken cancellationToken = default)
    {
        _logger.LogWarning("Database query executor is not configured; stubbed statement: {Statement}", statement);
        return Task.FromResult<object?>(new Dictionary<string, object?>
        {
            ["statement"] = statement,
            ["parameters"] = parameters,
            ["stubbed"] = true
        });
    }
}

public class StubKnowledgeSearcher : IKnowledgeSearcher
{
    private readonly ILogger<StubKnowledgeSearcher> _logger;

    public StubKnowledgeSearcher(ILogger<StubKnowledgeSearcher>? logger = null)
    {
        _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<StubKnowledgeSearcher>.Instance;
    }

    public Task<IReadOnlyList<object?>> SearchAsync(string query, int topK, CancellationToken cancellationToken = default)
    {
        _logger.LogWarning("Knowledge searcher is not configured; stubbed search '{Query}'.", query);
        return Task.FromResult<IReadOnlyList<object?>>([]);
    }
}

public class StubNotificationGateway : INotificationGateway
{
    private readonly ILogger<StubNotificationGateway> _logger;

    public StubNotificationGateway(ILogger<StubNotificationGateway>? logger = null)
    {
        _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<StubNotificationGateway>.Instance;
    }

    public Task<object?> SendAsync(string channel, IDictionary<string, object?> payload, CancellationToken cancellationToken = default)
    {
        _logger.LogWarning("Notification gateway is not configured; stubbed send on '{Channel}'.", channel);
        return Task.FromResult<object?>(new Dictionary<string, object?>
        {
            ["channel"] = channel,
            ["stubbed"] = true,
            ["messageId"] = Guid.NewGuid().ToString("N")
        });
    }
}

public class StubFinanceGateway : IFinanceGateway
{
    private readonly ILogger<StubFinanceGateway> _logger;

    public StubFinanceGateway(ILogger<StubFinanceGateway>? logger = null)
    {
        _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<StubFinanceGateway>.Instance;
    }

    public Task<object?> ExecuteAsync(string operation, IDictionary<string, object?> payload, CancellationToken cancellationToken = default)
    {
        _logger.LogWarning("Finance gateway is not configured; stubbed operation '{Operation}'.", operation);
        return Task.FromResult<object?>(new Dictionary<string, object?>
        {
            ["operation"] = operation,
            ["stubbed"] = true,
            ["echo"] = payload
        });
    }
}

public class StubSchedulingGateway : ISchedulingGateway
{
    private readonly ILogger<StubSchedulingGateway> _logger;

    public StubSchedulingGateway(ILogger<StubSchedulingGateway>? logger = null)
    {
        _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<StubSchedulingGateway>.Instance;
    }

    public Task<object?> ExecuteAsync(string operation, IDictionary<string, object?> payload, CancellationToken cancellationToken = default)
    {
        _logger.LogWarning("Scheduling gateway is not configured; stubbed operation '{Operation}'.", operation);
        return Task.FromResult<object?>(new Dictionary<string, object?>
        {
            ["operation"] = operation,
            ["stubbed"] = true,
            ["echo"] = payload
        });
    }
}
