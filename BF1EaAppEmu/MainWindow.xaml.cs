using BF1EaAppEmu.Api;
using BF1EaAppEmu.Core;
using BF1EaAppEmu.Helper;
using BF1EaAppEmu.Models;
using BF1EaAppEmu.Utils;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace BF1EaAppEmu;

/// <summary>
/// MainWindow.xaml 的交互逻辑
/// </summary>
public partial class MainWindow : Window
{
    /// <summary>
    /// 数据模型
    /// </summary>
    public MainModel MainModel { get; set; } = new();

    public MainWindow()
    {
        InitializeComponent();
    }

    /// <summary>
    /// 窗口加载完成事件
    /// </summary>
    private void Window_Main_Loaded(object sender, RoutedEventArgs e)
    {
        Task.Run(() =>
        {
            // 加载配置文件
            LoadConfig();

            ////////////////////////////////

            // 关闭服务进程
            CoreUtil.CloseServiceProcess();

            LoggerHelper.Info("正在释放资源服务进程文件...");
            FileHelper.ExtractResFile("Exec.EADesktop.exe", CoreUtil.File_Service_EADesktop);
            FileHelper.ExtractResFile("Exec.OriginDebug.exe", CoreUtil.File_Service_OriginDebug);

            // 打开服务进程
            LoggerHelper.Info("正在启动服务进程...");
            ProcessHelper.OpenProcess(CoreUtil.File_Service_EADesktop, true);
            ProcessHelper.OpenProcess(CoreUtil.File_Service_OriginDebug, true);

            LoggerHelper.Info("正在启动 Local HTTP 监听服务...");
            LocalHttpServer.Run();
        });

        // 注册 RunBf1Game
        WeakReferenceMessenger.Default.Register<string, string>(this, "RunBf1Game", async (s, e) =>
        {
            await RunBf1Game();
        });
        // 注册 CloseBf1Game
        WeakReferenceMessenger.Default.Register<string, string>(this, "CloseBf1Game", async (s, e) =>
        {
            await CloseBf1Game();
        });
    }

    /// <summary>
    /// 窗口关闭时事件
    /// </summary>
    private void Window_Main_Closing(object sender, CancelEventArgs e)
    {
        Task.Run(() =>
        {
            LoggerHelper.Info("正在停止 Local HTTP 监听服务...");
            LocalHttpServer.Stop();

            LoggerHelper.Info("正在停止 LSX 监听服务...");
            LSXTcpServer.Stop();

            // 关闭服务进程
            CoreUtil.CloseServiceProcess();

            ////////////////////////////////

            // 保存配置文件
            SaveConfig();
        });
    }

    /// <summary>
    /// 加载配置文件
    /// </summary>
    private void LoadConfig()
    {
        MainModel.AppPath = Globals.AppPath;

        MainModel.Remid = Globals.Remid;
        MainModel.Sid = Globals.Sid;

        MainModel.PlayerName = Globals.PlayerName;
        MainModel.PersonaId = Globals.PersonaId;
        MainModel.UserId = Globals.UserId;

        MainModel.RunArgs = string.Join(Environment.NewLine, Globals.ReadArgsFile()).Trim();
    }

    /// <summary>
    /// 保存配置文件
    /// </summary>
    private void SaveConfig()
    {
    }

    [RelayCommand]
    private void SelectBf1FilePath()
    {
        // 选择战地1主程序路径
        var dialog = new OpenFileDialog
        {
            Title = "选择战地1主程序 EAAntiCheat.GameServiceLauncher.exe 文件路径",
            FileName = "EAAntiCheat.GameServiceLauncher.exe",
            DefaultExt = ".exe",
            Filter = "可执行文件 (.exe)|*.exe",
            Multiselect = false,
            RestoreDirectory = true,
            AddExtension = true,
            CheckFileExists = true,
            CheckPathExists = true
        };

        // 如果未选择，则退出程序
        if (dialog.ShowDialog() == false)
            return;

        if (Path.GetFileName(dialog.FileName) != "EAAntiCheat.GameServiceLauncher.exe")
        {
            MsgBoxHelper.Warning("请选择正确的文件 EAAntiCheat.GameServiceLauncher.exe");
            return;
        }

        MainModel.AppPath = dialog.FileName;
    }

    [RelayCommand]
    private async Task RunBf1Game()
    {
        if (ProcessHelper.IsAppRun(CoreUtil.Name_BF1))
        {
            MsgBoxHelper.Warning("战地1已经启动了，请不要重复运行");
            return;
        }

        if (string.IsNullOrWhiteSpace(Globals.AppPath))
        {
            MsgBoxHelper.Warning("战地1启动路径不能为空");
            return;
        }

        Account.Remid = Globals.Remid;
        Account.Sid = Globals.Sid;

        if (string.IsNullOrWhiteSpace(Account.Remid))
        {
            MsgBoxHelper.Warning("玩家账号 Remid 不能为空");
            return;
        }

        if (string.IsNullOrWhiteSpace(Account.Sid))
        {
            MsgBoxHelper.Warning("玩家账号 Sid 不能为空");
            return;
        }

        /////////////////////////////////

        // 获取 AccessToken
        {
            var result = await EaApi.GetAccessToken();
            if (!result.IsSuccess)
            {
                MsgBoxHelper.Error("获取 AccessToken 失败");
                return;
            }
            LoggerHelper.Info("获取 AccessToken 成功");
        }

        // 获取 OriginPCAuth
        {
            var result = await EaApi.GetOriginPCAuth();
            if (!result.IsSuccess)
            {
                MsgBoxHelper.Error("获取 OriginPCAuth 失败");
                return;
            }
            LoggerHelper.Info("获取 OriginPCAuth 成功");
        }

        // 获取 OriginPCToken
        {
            var result = await EaApi.GetOriginPCToken();
            if (!result.IsSuccess)
            {
                MsgBoxHelper.Error("获取 OriginPCToken 失败");
                return;
            }
            LoggerHelper.Info("获取 OriginPCToken 成功");
        }

        // 正在获取当前登录玩家信息
        {
            var result = await EasyEaApi.GetLoginAccountInfo();
            if (result is null)
            {
                MsgBoxHelper.Error("获取当前登录玩家信息失败");
                return;
            }
            LoggerHelper.Info("获取当前登录玩家信息成功");

            var persona = result.personas.persona[0];

            Account.PlayerName = persona.displayName;
            LoggerHelper.Info($"玩家名称 {Account.PlayerName}");
            // 保存到本地
            Globals.PlayerName = Account.PlayerName;

            Account.PersonaId = persona.personaId.ToString();
            LoggerHelper.Info($"玩家PId {Account.PersonaId}");
            // 保存到本地
            Globals.PersonaId = Account.PersonaId;

            Account.UserId = persona.pidId.ToString();
            LoggerHelper.Info($"玩家UserId {Account.UserId}");
            // 保存到本地
            Globals.UserId = Account.UserId;
        }

        /////////////////////////////////

        LoggerHelper.Info("正在启动 LSX 监听服务...");
        LSXTcpServer.Run();

        // 带启动参数
        var runArgs = Globals.ReadArgsFile();
        if (runArgs != default)
        {
            var builder = new StringBuilder();
            foreach (var arg in runArgs)
            {
                builder.Append($"-{arg.Trim()} ");
            }

            // 启动战地1游戏
            Game.RunBf1Game(Globals.AppPath, builder.ToString().Trim());
            return;
        }

        // 启动战地1游戏
        Game.RunBf1Game(Globals.AppPath, "");
    }

    [RelayCommand]
    private async Task CloseBf1Game()
    {
        LoggerHelper.Info("正在停止 LSX 监听服务...");
        LSXTcpServer.Stop();

        foreach (var process in Process.GetProcessesByName("bf1"))
        {
            // 不关闭进程树
            process.Kill(false);
            await process.WaitForExitAsync();
        }
    }
}