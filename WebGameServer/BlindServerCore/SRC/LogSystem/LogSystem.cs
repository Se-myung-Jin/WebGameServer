using NLog;
using System.Text;

namespace BlindServerCore.Log;

public static class LogSystem
{
    public static Logger Log { get; private set; }
    public static Logger NetLog { get; private set; }

    private static string GetLayOut() => "[${longdate}] [${level}] ${message} ${exception:format=ToString}";
    private static string GetLayOut(string value) => $"[${{longdate}}] [${{level}}] [{value}] ${{message}} ${{exception:format=ToString}}";
    //Layout = "${longdate} ${logger} ${StackTrace} ${message} ${exception:format=ToString}",
    //Layout = "${longdate} ${StackTrace} ${message} ${exception:format=ToString}",
    //Layout = "${longdate} ${message} ${exception:format=ToString} ${callsite:fileName=true}",

    public static void Initialize(string appName)
    {
        var fileName = appName.ToLower();
        Log = AddLog("Service", $"{fileName}");
        NetLog = AddLog("Network", $"{fileName}_network");
    }

    public static Logger AddLog(string name, string fileName)
    {
        var factory = new LogFactory();
        var config = new NLog.Config.LoggingConfiguration(factory);

        // Targets where to log to: File and Console
        var logfile = new NLog.Targets.FileTarget(name)
        {
            FileName = @"${basedir}/../logs/" + fileName + ".log",
            Layout = GetLayOut(name),
            AutoFlush = true,
            Encoding = Encoding.UTF8,
            KeepFileOpen = true,
            OpenFileCacheTimeout = 30,
            ConcurrentWrites = false,
            ArchiveAboveSize = 10485760,
            ArchiveNumbering = NLog.Targets.ArchiveNumberingMode.Sequence,
            ArchiveEvery = NLog.Targets.FileArchivePeriod.Day,
            MaxArchiveDays = 5,
            ArchiveFileName = @"${basedir}/../logs/" + fileName + "_{#}.log",
        };

        var asyncLogFile = new NLog.Targets.Wrappers.AsyncTargetWrapper(name, logfile);
        config.AddRule(LogLevel.Trace, LogLevel.Fatal, asyncLogFile);

        AddConsole(config, logfile.Layout.ToString());

        factory.Configuration = config;
        return factory.GetCurrentClassLogger();
    }

    //[Conditional("DEBUG")]
    public static void DebugBuildTrace(this NLog.Logger logger, string message)
    {
        logger.Trace(message);
    }

    private static void AddConsole(NLog.Config.LoggingConfiguration config, string layout)
    {
        // Rules for mapping loggers to targets            
        var logconsole = new NLog.Targets.ColoredConsoleTarget($"console");
        logconsole.Layout = layout;
        config.AddRule(LogLevel.Trace, LogLevel.Fatal, logconsole);
    }
}
