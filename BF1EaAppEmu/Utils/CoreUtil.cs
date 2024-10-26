using BF1EaAppEmu.Helper;

namespace BF1EaAppEmu.Utils;

public static class CoreUtil
{
    public static string Dir_Default { get; private set; }

    public static string Dir_Config { get; private set; }
    public static string Dir_Service { get; private set; }
    public static string Dir_NLog { get; private set; }
    public static string Dir_Crash { get; private set; }

    public static string File_Config_RunArgs { get; private set; }

    public static string File_Service_EADesktop { get; private set; }
    public static string File_Service_OriginDebug { get; private set; }

    static CoreUtil()
    {
        var baseDir = AppContext.BaseDirectory;

        Dir_Default = Path.Combine(baseDir, "AppData");

        FileHelper.CreateDirectory(Dir_Default);

        Dir_Config = Path.Combine(Dir_Default, "Config");
        Dir_Service = Path.Combine(Dir_Default, "Service");
        Dir_NLog = Path.Combine(Dir_Default, "NLog");
        Dir_Crash = Path.Combine(Dir_Default, "Crash");

        FileHelper.CreateDirectory(Dir_Config);
        FileHelper.CreateDirectory(Dir_Service);
        FileHelper.CreateDirectory(Dir_NLog);
        FileHelper.CreateDirectory(Dir_Crash);

        File_Config_RunArgs = Path.Combine(Dir_Config, "RunArgs.txt");

        File_Service_EADesktop = Path.Combine(Dir_Service, "EADesktop.exe");
        File_Service_OriginDebug = Path.Combine(Dir_Service, "OriginDebug.exe");
    }

    /// <summary>
    /// 判断战地1是否正在运行
    /// </summary>
    /// <returns>返回相应结果</returns>
    public static bool IsBf1Running()
    {
        return Process.GetProcessesByName("bf1").Length > 0;
    }

    /// <summary>
    /// 判断战地1文件路径是否存在
    /// </summary>
    /// <param name="appPath">文件 EAAntiCheat.GameServiceLauncher.exe 路径</param>
    /// <returns>返回相应结果</returns>
    public static bool IsBf1AppPathExists(string appPath)
    {
        return File.Exists(appPath);
    }

    /// <summary>
    /// 关闭服务进程
    /// </summary>
    public static void CloseServiceProcess()
    {
        LoggerHelper.Info("正在关闭服务进程中...");

        ProcessHelper.CloseProcess("EADesktop");
        ProcessHelper.CloseProcess("OriginDebug");

        ProcessHelper.CloseProcess("Origin");

        ProcessHelper.CloseProcess("EAappEmulater");
        ProcessHelper.CloseProcess("BF1ModTools");
    }
}
