using BF1EaAppEmu.Helper;
using BF1EaAppEmu.Utils;

namespace BF1EaAppEmu;

public static class Globals
{
    /// <summary>
    /// 配置文件路径
    /// </summary>
    private static readonly string _iniPath;

    /// <summary>
    /// 战地1主程序位置
    /// </summary>
    public static string AppPath
    {
        get => ReadString("Core", "AppPath");
        set => WriteString("Core", "AppPath", value);
    }

    /// <summary>
    /// Cookie - Remid
    /// </summary>
    public static string Remid
    {
        get => ReadString("Account", "Remid");
        set => WriteString("Account", "Remid", value);
    }

    /// <summary>
    /// Cookie - Sid
    /// </summary>
    public static string Sid
    {
        get => ReadString("Account", "Sid");
        set => WriteString("Account", "Sid", value);
    }

    /// <summary>
    /// 玩家昵称
    /// </summary>
    public static string PlayerName
    {
        get => ReadString("Account", "PlayerName");
        set => WriteString("Account", "PlayerName", value);
    }

    /// <summary>
    /// 玩家数字Id
    /// </summary>
    public static string PersonaId
    {
        get => ReadString("Account", "PersonaId");
        set => WriteString("Account", "PersonaId", value);
    }

    /// <summary>
    /// 玩家用户Id
    /// </summary>
    public static string UserId
    {
        get => ReadString("Account", "UserId");
        set => WriteString("Account", "UserId", value);
    }

    //////////////////////////////////////////

    static Globals()
    {
        _iniPath = Path.Combine(CoreUtil.Dir_Config, "Config.ini");
        LoggerHelper.Info($"当前配置文件路径 {_iniPath}");
    }

    /// <summary>
    /// 从Txt文件读取启动参数
    /// </summary>
    public static string[] ReadArgsFile()
    {
        try
        {
            if (!File.Exists(CoreUtil.File_Config_RunArgs))
            {
                LoggerHelper.Error($"未发现启动参数Txt文件 {CoreUtil.File_Config_RunArgs}");
                return default;
            }

            return File.ReadAllLines(CoreUtil.File_Config_RunArgs);
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"从Txt文件读取启动参数异常 {CoreUtil.File_Config_RunArgs}", ex);
            return default;
        }
    }

    /// <summary>
    /// 保存启动参数到Txt文件
    /// </summary>
    public static void SaveArgsFile(string contents)
    {
        try
        {
            File.WriteAllText(CoreUtil.File_Config_RunArgs, contents.Trim(), Encoding.UTF8);
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"保存启动参数Txt文件异常 {CoreUtil.File_Config_RunArgs}", ex);
        }
    }

    /// <summary>
    /// 读取配置文件字符串数据
    /// </summary>
    private static string ReadString(string section, string key)
    {
        return IniHelper.ReadString(section, key, _iniPath);
    }

    /// <summary>
    /// 写入配置文件字符串数据
    /// </summary>
    private static void WriteString(string section, string key, string value)
    {
        IniHelper.WriteString(section, key, value, _iniPath);
    }
}
