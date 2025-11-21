using System.Threading.Channels;

namespace SSE.Backend.Services;

public class SseService
{
    /// <summary>
    ///  instead of using channels you can Also Use Redis Pub/sub But Channels Are faster and Simple
    /// </summary>
    private readonly Channel<string> _channel = Channel.CreateUnbounded<string>();

    public IAsyncEnumerable<string> SubscribeAsync(CancellationToken cancellationToken)
        => _channel.Reader.ReadAllAsync(cancellationToken);

    public async Task PublishAsync(string data)
    {
        if (!string.IsNullOrWhiteSpace(data))
            await _channel.Writer.WriteAsync(data);
    }
}
