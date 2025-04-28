using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LogAnalyticsServer;

public class ServiceWeb : Singleton<ServiceWeb>
{
    private WebApplication _application;

    public async Task StartAsync(string url, string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Logging.ClearProviders();
        builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Information);
        //builder.Host.UseNLog();

        builder.Services.AddCors();
        builder.Services.AddHostedService<MetricsJobScheduler>();
        //builder.Services.AddSignalR().AddJsonProtocol();

        // Wait 30 seconds for graceful shutdown.
        builder.Host.ConfigureHostOptions(o => o.ShutdownTimeout = TimeSpan.FromSeconds(30));

        _application = builder.Build();

        _application.UseCors(builder =>
        {
            builder.WithOrigins()
                .SetIsOriginAllowed((host) => true)
                //.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod()
            .AllowCredentials();
        });

        _application.Use((context, next) =>
        {
            // Put a breakpoint here
            return next(context);
        });

        await _application.RunAsync(url);
    }

    public async Task StopAsync()
    {
        if (_application == null)
        {
            return;
        }

        await _application.StopAsync();
    }
}