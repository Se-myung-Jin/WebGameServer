using System.Diagnostics;

namespace Common;

public class RegisterService
{
    public static int InstallService(string netDllPath, bool doInstall)
    {
        string serviceFile = @"
[Unit]
Description={0} running on {1}

[Service]
WorkingDirectory={2}
ExecStart={3}
SyslogIdentifier={5}
KillMode=mixed

[Install]
WantedBy=multi-user.target
";

        string dllFileName = Path.GetFileName(netDllPath);
        string osName = Environment.OSVersion.ToString();

        FileInfo fileInfo = null;

        try
        {
            fileInfo = new FileInfo(netDllPath);
        }
        catch { }

        if (doInstall == true && fileInfo != null && fileInfo.Exists == false)
        {
            Console.WriteLine("NOT FOUND: " + fileInfo.FullName);

            return 1;
        }

        string serviceName = Path.GetFileNameWithoutExtension(dllFileName).ToLower();

        string exeName = Process.GetCurrentProcess().MainModule.FileName;

        string workingDir = Path.GetDirectoryName(fileInfo.FullName);
        string fullText = string.Format(serviceFile, dllFileName, osName, workingDir, exeName, fileInfo.FullName, serviceName);

        string serviceFilePath = $"/etc/systemd/system/{serviceName}.service";

        if (doInstall == true)
        {
            File.WriteAllText(serviceFilePath, fullText);
            Console.WriteLine(serviceFilePath + " Created");
            ControlService(serviceName, "enable");
            ControlService(serviceName, "start");
        }
        else
        {
            if (File.Exists(serviceFilePath) == true)
            {
                ControlService(serviceName, "stop");
                File.Delete(serviceFilePath);
                Console.WriteLine(serviceFilePath + " Deleted");
            }
        }

        return 0;
    }

    static int ControlService(string serviceName, string mode)
    {
        string servicePath = $"/etc/systemd/system/{serviceName}.service";

        if (File.Exists(servicePath) == false)
        {
            Console.WriteLine($"No service: {serviceName} to {mode}");

            return 1;
        }

        ProcessStartInfo processStartInfo = new ProcessStartInfo();
        processStartInfo.FileName = "systemctl";
        processStartInfo.Arguments = $"{mode} {serviceName}";

        Process child = Process.Start(processStartInfo);
        child.WaitForExit();
        return child.ExitCode;
    }
}
