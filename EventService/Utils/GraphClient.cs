namespace Semifinals.EventService.Utils;

public interface IGraphClient
{
    GremlinClient GremlinClient { get; }

    Task<ResultSet<T>> SubmitAsync<T>(
        string requestScript,
        Dictionary<string, object> bindings,
        CancellationToken cancellationToken = default);

    Task<ResultSet<T>> SubmitAsync<T>(
        string requestScript,
        CancellationToken cancellationToken = default);

    Task<T> SubmitWithSingleResultAsync<T>(
        string requestScript,
        Dictionary<string, object> bindings,
        CancellationToken cancellationToken = default);

    Task<T> SubmitWithSingleResultAsync<T>(
        string requestScript,
        CancellationToken cancellationToken = default);
}

public class GraphClient : IGraphClient, IDisposable
{
    public GremlinClient GremlinClient { get; init; }

    public GraphClient(
        GremlinServer gremlinServer,
        IMessageSerializer? messageSerializer = null,
        ConnectionPoolSettings? connectionPoolSettings = null,
        Action<ClientWebSocketOptions>? webSocketConfiguration = null,
        string? sessionId = null,
        bool disableCompression = false,
        ILoggerFactory? loggerFactory = null)
    {
        GremlinClient = new(
            gremlinServer,
            messageSerializer,
            connectionPoolSettings,
            webSocketConfiguration,
            sessionId,
            disableCompression,
            loggerFactory);
    }

    void IDisposable.Dispose()
    {
        GremlinClient.Dispose();
        GC.SuppressFinalize(this);
    }

    public async Task<ResultSet<T>> SubmitAsync<T>(
        string requestScript,
        Dictionary<string, object> bindings,
        CancellationToken cancellationToken = default)
    {
        return await GremlinClient.SubmitAsync<T>(requestScript, bindings, cancellationToken);
    }

    public async Task<ResultSet<T>> SubmitAsync<T>(
        string requestScript,
        CancellationToken cancellationToken = default)
    {
        return await SubmitAsync<T>(requestScript, new(), cancellationToken);
    }

    public async Task<T> SubmitWithSingleResultAsync<T>(
        string requestScript,
        Dictionary<string, object> bindings,
        CancellationToken cancellationToken = default)
    {
        return await GremlinClient.SubmitWithSingleResultAsync<T>(requestScript, bindings, cancellationToken);
    }

    public async Task<T> SubmitWithSingleResultAsync<T>(
        string requestScript,
        CancellationToken cancellationToken = default)
    {
        return await SubmitWithSingleResultAsync<T>(requestScript, new(), cancellationToken);
    }
}
