using SSE.Backend.Services;
using SSE.Client.Handlers;

namespace SSE.Client.Features;

public static class SseEndpoints
{
    public static void MapSseEndpoints(this WebApplication app)
    {
        app.MapGroup("/sse")
            .WithDescription("a simple way to communicate with the server")
            .WithTags("SSE");

        app.MapGet("/stream", SseHandler.HandleAsync);

        app.MapPost("/notifications/send", async (SseService service) => await service.PublishAsync("logSomethings"));
    }
}
