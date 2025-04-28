using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GameServer;

public class ServiceWeb : Singleton<ServiceWeb>
{
    private WebApplication _application;

    public async Task StartAsync(string url, string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Logging.ClearProviders();
        builder.Logging.SetMinimumLevel(LogLevel.Information);

        builder.Services.AddCors();

        builder.Host.ConfigureHostOptions(opt => opt.ShutdownTimeout = TimeSpan.FromSeconds(30));

        _application = builder.Build();
        _application.UseCors(builder =>
        {
            builder.WithOrigins()
                   .SetIsOriginAllowed((host) => true)
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials();
        });
        
        RestApiRouter.Instance.Initialize();

        _application.MapGet("/Hello", async (HttpContext context) => await context.Response.WriteAsync("Hello"));
        _application.MapPost("/RestApi/{name:alpha}", async (HttpContext context) => await RestApiRouter.Instance.Process(context));

        _application.Use((context, next) =>
        {
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