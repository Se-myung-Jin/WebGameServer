using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MaintenanceServer;

public class ServiceWeb : Singleton<ServiceWeb>
{
    private WebApplication application;

    public async Task StartAsync(string url, string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Logging.ClearProviders();
        builder.Logging.SetMinimumLevel(LogLevel.Information);

        builder.Services.AddCors();

        builder.Host.ConfigureHostOptions(opt => opt.ShutdownTimeout = TimeSpan.FromSeconds(30));

        application = builder.Build();
        application.UseCors(builder =>
        {
            builder.WithOrigins()
                   .SetIsOriginAllowed((host) => true)
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials();
        });
        
        //RestApiRouter.Instance.Initialize();

        application.MapGet("/Hello", async (HttpContext context) => await context.Response.WriteAsync("Hello"));
        //application.MapPost("/RestApi/{name:alpha}", async (HttpContext context) => await RestApiRouter.Instance.Process(context));

        application.Use((context, next) =>
        {
            return next(context);
        });

        await application.RunAsync(url);
    }

    public async Task StopAsync()
    {
        if (application == null)
        {
            return;
        }

        await application.StopAsync();
    }
}