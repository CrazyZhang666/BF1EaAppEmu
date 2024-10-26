using BF1EaAppEmu.Helper;

namespace BF1EaAppEmu;

/// <summary>
/// App.xaml 的交互逻辑
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// 主程序互斥体
    /// </summary>
    private Mutex AppMainMutex;
    /// <summary>
    /// 应用程序名称
    /// </summary>
    private readonly string AppName = ResourceAssembly.GetName().Name;

    protected override void OnStartup(StartupEventArgs e)
    {
        AppMainMutex = new Mutex(true, AppName, out var createdNew);
        if (!createdNew)
        {
            LoggerHelper.Warn("请不要重复打开，程序已经运行");
            MsgBoxHelper.Warning($"请不要重复打开，程序已经运行\n如果一直提示，请到\"任务管理器-详细信息（win7为进程）\"里\n强制结束 \"{AppName}.exe\" 程序");
            Current.Shutdown();
            return;
        }

        base.OnStartup(e);
    }
}
