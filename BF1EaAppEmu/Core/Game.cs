using BF1EaAppEmu.Helper;

namespace BF1EaAppEmu.Core;

public static class Game
{
    /// <summary>
    /// 获取系统环境变量集合
    /// </summary>
    private static Dictionary<string, string> GetEnvironmentVariables()
    {
        var environmentVariables = new Dictionary<string, string>();
        foreach (DictionaryEntry dirEnity in Environment.GetEnvironmentVariables())
        {
            environmentVariables.Add(dirEnity.Key.ToString(), dirEnity.Value.ToString());
        }
        return environmentVariables;
    }

    /// <summary>
    /// 启动战地1游戏
    /// </summary>
    public static void RunBf1Game(string bf1AppPath, string runArgs = "")
    {
        try
        {
            if (string.IsNullOrWhiteSpace(bf1AppPath))
            {
                LoggerHelper.Warn("战地1游戏路径为空，启动游戏终止");
                return;
            }

            LoggerHelper.Info("正在启动战地1游戏中...");

            // 获取当前进程所有环境变量名及其值
            var environmentVariables = GetEnvironmentVariables();

            // 直接赋值给字典，如果键已存在，则更新对应的值，否则则创建
            environmentVariables["EAFreeTrialGame"] = "false";
            environmentVariables["EAAuthCode"] = Account.OriginPCToken;
            environmentVariables["EALaunchOfflineMode"] = "false";
            environmentVariables["OriginSessionKey"] = "7102090b-ea9a-4531-9598-b2a7e943b544";
            environmentVariables["EAGameLocale"] = "zh_TW";
            environmentVariables["EALaunchEnv"] = "production";
            environmentVariables["EALaunchEAID"] = Account.PlayerName;
            environmentVariables["EALicenseToken"] = "114514";
            environmentVariables["EAEntitlementSource"] = "EA";
            environmentVariables["EAUseIGOAPI"] = "1";
            environmentVariables["EALaunchUserAuthToken"] = Account.OriginPCToken;
            environmentVariables["EAGenericAuthToken"] = Account.OriginPCToken;
            environmentVariables["EALaunchCode"] = "unavailable";
            environmentVariables["EARtPLaunchCode"] = EaCrypto.GetRTPHandshakeCode();
            environmentVariables["EALsxPort"] = "3216";
            environmentVariables["EAEgsProxyIpcPort"] = "1705";
            environmentVariables["EASteamProxyIpcPort"] = "1704";
            environmentVariables["EAExternalSource"] = "EA";
            environmentVariables["EASecureLaunchTokenTemp"] = "1001006949032";
            environmentVariables["SteamAppId"] = "";
            environmentVariables["ContentId"] = "1026023";
            environmentVariables["EAConnectionId"] = "1026023";

            var startInfo = new ProcessStartInfo
            {
                UseShellExecute = false,
                FileName = bf1AppPath,
                WorkingDirectory = Path.GetDirectoryName(bf1AppPath),
                Arguments = runArgs
            };

            // 批量设置进程启动环境变量
            foreach (var variable in environmentVariables)
            {
                startInfo.EnvironmentVariables[variable.Key] = variable.Value;
            }

            //启动程序 
            Process.Start(startInfo);
            LoggerHelper.Info("启动战地1游戏成功");
        }
        catch (Exception ex)
        {
            LoggerHelper.Error("启动战地1游戏发生异常", ex);
        }
    }
}
