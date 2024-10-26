using BF1EaAppEmu.Core;
using BF1EaAppEmu.Helper;

namespace BF1EaAppEmu.Api;

public static class EasyEaApi
{
    /// <summary>
    /// 获取登录玩家账号信息
    /// </summary>
    public static async Task<Identity> GetLoginAccountInfo()
    {
        var result = await EaApi.GetIdentityMe();
        if (!result.IsSuccess)
            return null;

        return JsonHelper.JsonDeserialize<Identity>(result.Content);
    }

    /// <summary>
    /// 获取 LSX 监听服务所需的 AutuCode
    /// </summary>
    public static async Task<string> GetLSXAutuCode(string settingId)
    {
        var result = await EaApi.GetLSXAutuCode(settingId);
        if (!result.IsSuccess)
            return string.Empty;

        return Account.LSXAuthCode;
    }

    /// <summary>
    /// 获取 LSX 监听服务所需的许可证 License
    /// </summary>
    public static async Task<string> GetLSXLicense(string requestToken, string contentId)
    {
        var result = await EaApi.GetLSXLicense(requestToken, contentId);
        if (!result.IsSuccess)
            return string.Empty;

        return result.Content;
    }
}
