using SSE.Backend.Services;

namespace SSE.Client.Handlers
{
    public static class SseHandler
    {
        public static async Task HandleAsync(
            SseService sseService,
            HttpContext context,
            CancellationToken cancellationToken)
        {
            context.Response.Headers.Add("Content-Type", "text/event-stream");
            context.Response.Headers.Add("Cache-Control", "no-cache");
            context.Response.Headers.Add("Connection", "keep-alive");
            context.Response.Headers.Add("X-Accel-Buffering", "no");

            /// enable Event Identification and Resumption
            string lastEventId = context.Request.Headers["Last-Event-ID"];

            // initial message so client enters SSE mode immediately
            await context.Response.WriteAsync("retry: 2000\n\n", cancellationToken);
            await context.Response.Body.FlushAsync(cancellationToken);

            await foreach (var sseEvent in sseService.SubscribeAsync(lastEventId, cancellationToken))
            {
                try
                {
                    await context.Response.WriteAsync($"id: {sseEvent.Id}\ndata: {sseEvent.Data}\n\n", cancellationToken);
                    await context.Response.Body.FlushAsync(cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("SSE: client disconnected");
                    break;
                }
            }
        }
    }
}
