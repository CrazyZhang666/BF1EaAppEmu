namespace BF1EaAppEmu.Helper;

public static class ProcessHelper
{
    /// <summary>
    /// 打开指定进程（支持静默）
    /// </summary>
    public static void OpenProcess(string appPath, bool isSilent = false)
    {
        if (!File.Exists(appPath))
        {
            LoggerHelper.Warn($"程序路径不存在 {appPath}");
            return;
        }

        var fileInfo = new FileInfo(appPath);

        try
        {
            // 如果应在启动进程时使用 shell，则为 true；如果直接从可执行文件创建进程，则为 false。
            // 默认值为 true .NET Framework 应用和 false .NET Core 应用。
            var processInfo = new ProcessStartInfo
            {
                UseShellExecute = true,
                FileName = fileInfo.FullName,
                WorkingDirectory = fileInfo.DirectoryName
            };

            if (isSilent)
            {
                processInfo.WindowStyle = ProcessWindowStyle.Hidden;
                processInfo.CreateNoWindow = true;
            }

            Process.Start(processInfo);
            LoggerHelper.Info($"启动程序成功 {appPath}");
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"启动程序异常 {appPath}", ex);
        }
    }

    /// <summary>
    /// 根据名字关闭指定进程
    /// </summary>
    public static void CloseProcess(string appName)
    {
        if (string.IsNullOrWhiteSpace(appName))
            return;

        if (appName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
            appName = appName[..^4];

        try
        {
            var isFind = false;

            foreach (var process in Process.GetProcessesByName(appName))
            {
                // 关闭进程树
                process.Kill(true);
                LoggerHelper.Info($"关闭进程成功 {appName}.exe");

                isFind = true;
            }

            if (!isFind)
                LoggerHelper.Warn($"未找到进程信息 {appName}.exe");
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"关闭进程异常 {appName}", ex);
        }
    }
}
