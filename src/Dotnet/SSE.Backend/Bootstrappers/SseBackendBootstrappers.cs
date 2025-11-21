using Microsoft.Extensions.DependencyInjection;
using SSE.Backend.Services;

namespace SSE.Backend.Bootstrappers;

public static class SseBackendBootstrappers
{
    public static IServiceCollection AddSseBackend(this IServiceCollection services)
    {
        services.AddSingleton<SseService>();
        return services;
    }
}
