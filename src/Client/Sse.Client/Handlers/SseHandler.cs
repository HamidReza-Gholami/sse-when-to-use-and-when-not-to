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


            // initial message so client enters SSE mode immediately
            await context.Response.WriteAsync("retry: 2000\n\n", cancellationToken);
            await context.Response.Body.FlushAsync(cancellationToken);

            await foreach (var message in sseService.SubscribeAsync(cancellationToken))
            {
                try
                {
                    await context.Response.WriteAsync($"data: {message}\n\n", cancellationToken);
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
