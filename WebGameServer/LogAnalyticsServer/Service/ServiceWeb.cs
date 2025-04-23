using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LogAnalyticsServer;

public class ServiceWeb : Singleton<ServiceWeb>
{
    private WebApplication m_application;

    public async Task StartAsync(string url, string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Logging.ClearProviders();
        builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Information);
        //builder.Host.UseNLog();

        builder.Services.AddCors();
        //builder.Services.AddSignalR().AddJsonProtocol();

        // Wait 30 seconds for graceful shutdown.
        builder.Host.ConfigureHostOptions(o => o.ShutdownTimeout = TimeSpan.FromSeconds(30));

        m_application = builder.Build();

        m_application.UseCors(builder =>
        {
            builder.WithOrigins()
                .SetIsOriginAllowed((host) => true)
                //.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod()
            .AllowCredentials();
        });

        m_application.Use((context, next) =>
        {
            // Put a breakpoint here
            return next(context);
        });

        await m_application.RunAsync(url);
    }

    public async Task StopAsync()
    {
        if (m_application == null)
        {
            return;
        }

        await m_application.StopAsync();
    }
}