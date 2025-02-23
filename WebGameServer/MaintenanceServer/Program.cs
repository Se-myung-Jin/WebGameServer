global using Common;

namespace MaintenanceServer;

class Program
{
    static ProcessStarter processStarter = new MaintenanceProcessStarter();
    
    static async Task Main(string[] args)
    {
        AppDomain.CurrentDomain.UnhandledException += processStarter.UnhandledException;
        AppDomain.CurrentDomain.ProcessExit += processStarter.CloseAppEvent;
        Console.CancelKeyPress += processStarter.CloseConsoleEvent;

        if (args.Length >= 1)
        {
            string netDllPath = typeof(Program).Assembly.Location;
            if (args[0] == "--install" || args[0] == "-i")
            {
                RegisterService.InstallService(netDllPath, true);
            }
            else if (args[0] == "--uninstall" || args[0] == "-u")
            {
                RegisterService.InstallService(netDllPath, false);
            }
            return;
        }

        await Global.InitializeAsync("Maintenance", "maintenance.toml");

        await processStarter.StartAsync(Global.ServiceConfig);

        Global.Destruct();
    }
}