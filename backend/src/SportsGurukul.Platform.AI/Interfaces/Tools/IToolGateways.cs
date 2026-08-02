namespace SportsGurukul.Platform.AI.Interfaces.Tools;

public interface IInternalApiGateway
{
    Task<object?> CallAsync(string operation, IDictionary<string, object?> arguments, CancellationToken cancellationToken = default);
}

public interface IRestApiClient
{
    Task<object?> CallAsync(Uri endpoint, string method, IDictionary<string, object?>? body, IDictionary<string, string?>? headers, CancellationToken cancellationToken = default);
}

public interface IDatabaseQueryExecutor
{
    Task<object?> ExecuteAsync(string statement, IDictionary<string, object?>? parameters, CancellationToken cancellationToken = default);
}

public interface IKnowledgeSearcher
{
    Task<IReadOnlyList<object?>> SearchAsync(string query, int topK, CancellationToken cancellationToken = default);
}

public interface INotificationGateway
{
    Task<object?> SendAsync(string channel, IDictionary<string, object?> payload, CancellationToken cancellationToken = default);
}

public interface IFinanceGateway
{
    Task<object?> ExecuteAsync(string operation, IDictionary<string, object?> payload, CancellationToken cancellationToken = default);
}

public interface ISchedulingGateway
{
    Task<object?> ExecuteAsync(string operation, IDictionary<string, object?> payload, CancellationToken cancellationToken = default);
}
