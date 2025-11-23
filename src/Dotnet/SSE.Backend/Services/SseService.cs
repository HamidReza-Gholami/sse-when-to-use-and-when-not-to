using System.Runtime.CompilerServices;
using System.Threading.Channels;

namespace SSE.Backend.Services;

public record SseEvent(long Id, string Data);

public class SseService
{
    private readonly Channel<SseEvent> _channel = Channel.CreateUnbounded<SseEvent>();
    
    // Store the last 100 events for resumption (adjust capacity as needed)
    private readonly List<SseEvent> _recentEvents = new(100);
    private long _currentId = 0;
    private readonly object _lock = new();

    public async Task PublishAsync(string data)
    {
        if (string.IsNullOrWhiteSpace(data)) return;

        SseEvent newEvent;
        lock (_lock)
        {
            _currentId++;
            newEvent = new SseEvent(_currentId, $"{data}, Id: {_currentId}");

            _recentEvents.Add(newEvent);
            if (_recentEvents.Count > 100)
            {
                _recentEvents.RemoveAt(0);
            }
        }

        await _channel.Writer.WriteAsync(newEvent);
    }

    public async IAsyncEnumerable<SseEvent> SubscribeAsync(
        string? lastEventId,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        long startingId = 0;

        if (long.TryParse(lastEventId, out long parsedId))
            startingId = parsedId;

        List<SseEvent> missedEvents;
        lock (_lock)
            missedEvents = _recentEvents.Where(e => e.Id > startingId).ToList();

        foreach (var missedEvent in missedEvents)
            yield return missedEvent;
    }
}