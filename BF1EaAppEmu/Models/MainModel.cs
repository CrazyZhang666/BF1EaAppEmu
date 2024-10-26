using CommunityToolkit.Mvvm.ComponentModel;

namespace BF1EaAppEmu.Models;

public partial class MainModel : ObservableObject
{
    [ObservableProperty]
    private string appPath;

    [ObservableProperty]
    private string remid;

    [ObservableProperty]
    private string sid;

    [ObservableProperty]
    private string runArgs;

    [ObservableProperty]
    private string playerName;

    [ObservableProperty]
    private string personaId;

    [ObservableProperty]
    private string userId;

    partial void OnAppPathChanged(string value)
    {
        value = value.Trim();
        Globals.AppPath = value;
    }

    partial void OnRemidChanged(string value)
    {
        value = value.Trim();
        Globals.Remid = value;
    }

    partial void OnSidChanged(string value)
    {
        value = value.Trim();
        Globals.Sid = value;
    }

    partial void OnRunArgsChanged(string value)
    {
        value = value.Trim();
        Globals.SaveArgsFile(value);
    }
}
