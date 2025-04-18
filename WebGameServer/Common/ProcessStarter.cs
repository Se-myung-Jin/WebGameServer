﻿using System.Runtime.InteropServices;

namespace Common;

public class ProcessStarter
{
    const uint ENABLE_QUICK_EDIT = 0x0040;
    const int STD_INPUT_HANDLE = -10;

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern IntPtr GetStdHandle(int stdHandle);

    [DllImport("kernel32.dll")]
    static extern bool GetConsoleMode(IntPtr consoleHandle, out uint mode);

    [DllImport("kernel32.dll")]
    static extern bool SetConsoleMode(IntPtr consoleHandle, uint mode);

    readonly AutoResetEvent waitEvent = new(false);

    public async Task StartAsync(ServiceConfig config)
    {
        DisableQuickEdit();
        
        await InitializeConfigAsync(config);
        await InitializeDatabaseAsync(config);
        await InitializeTableAsync(config);
        await InitializeOthersAsync(config);
        await InitializeServiceAsync(config);

        await OnWaitExitSignalAsync();
    }

    protected static bool DisableQuickEdit()
    {
        if (OperatingSystem.IsWindows() == false)
        { 
            return false;
        }

        IntPtr consoleHandle = GetStdHandle(STD_INPUT_HANDLE);

        uint consoleMode;
        if (GetConsoleMode(consoleHandle, out consoleMode) == false)
        {
            return false;
        }

        consoleMode &= ~ENABLE_QUICK_EDIT;
        if (SetConsoleMode(consoleHandle, consoleMode) == false)
        {
            return false;
        }

        return true;
    }

    protected virtual async Task InitializeConfigAsync(ServiceConfig config)
    {
        await Task.FromResult(0);
    }

    protected virtual async Task InitializeDatabaseAsync(ServiceConfig config)
    {
        DatabaseContextContainer.Instance.Create(config.ConfigDatabase.DbPoolConfig());

        spGetDbConfig proc = new();
        if (proc.Run() == false)
        {
            throw new Exception("initializeDatabaseAsync spGetDbConfig or Variable Error");
        }

        DatabaseContextContainer.Instance.Create(proc.Result.RedisList.ConvertAll(x => x.DbPoolConfig()));
        DatabaseContextContainer.Instance.Create(proc.Result.LogDatabase.DbPoolConfig());
        DatabaseContextContainer.Instance.Initialize();

        await Task.FromResult(0);
    }

    protected virtual async Task InitializeTableAsync(ServiceConfig config)
    {
        await Task.FromResult(0);
    }

    protected virtual async Task InitializeOthersAsync(ServiceConfig config)
    {
        await Task.FromResult(0);
    }

    protected virtual async Task InitializeServiceAsync(ServiceConfig config)
    {
        await Task.FromResult(0);
    }
    protected virtual async Task OnWaitExitSignalAsync()
    {
        if (OperatingSystem.IsWindows() == false)
        {
            PosixSignalRegistration.Create((PosixSignal)(10) /* SIGUSR1 */, context =>
            {
                context.Cancel = true;
                CloseApp($"PosixSignal {context.Signal}");
            });
        }

        waitEvent.WaitOne();

        await Task.FromResult(0);
    }

    public void UnhandledException(object sender, UnhandledExceptionEventArgs args)
    {
        if (args.ExceptionObject != null)
        {

        }            

        Environment.Exit(9999);
    }

    public virtual void CloseApp(string reason)
    {
        waitEvent.Set();
    }

    public void CloseAppEvent(object? sender, EventArgs args)
    {
        CloseApp("CloseApplication");
    }

    public void CloseConsoleEvent(object? sender, ConsoleCancelEventArgs args)
    {
        args.Cancel = true;

        CloseApp("CloseConsole");
    }
}
