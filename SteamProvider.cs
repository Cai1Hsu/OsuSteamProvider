using System.Runtime.CompilerServices;
using Steamworks;

namespace OsuSteamProvider;

public static class SteamProvider
{
    public const int MCOSU_APPID = 607260;
    
    public static bool Initlized { get; private set; } = false;
    
    public static void Register()
    {
        if (Initlized) return;
        
        SteamClient.Init(MCOSU_APPID);

        Initlized = true;
    }

    public static void Unregister()
    {
        if (!Initlized) return;
        
        SteamClient.Shutdown();
    }

    public static void Update()
    {
		if (!Initlized) return;

        SteamClient.RunCallbacks();
    }

    public static bool SetStatus(string info)
    {
        if (!Initlized) return false;

        if (string.IsNullOrEmpty(info)) return false;

        const string STEAM_STATUS_KEY = @"status";
        
        const int k_cchMaxRichPresenceValueLength = 256;
        
        if (info.Length > k_cchMaxRichPresenceValueLength)
        {
            info = info.Substring(0, k_cchMaxRichPresenceValueLength - 3);
            
            info += "...";
        }
        
        return SteamFriends.SetRichPresence(STEAM_STATUS_KEY, info);
    }

    public static bool SetupRichPresenceLocalization()
    {
        try
        {
            const string STEAM_DISPLAY_KEY = @"steam_display";

            SteamFriends.SetRichPresence(STEAM_DISPLAY_KEY, "#Status");
        }
        catch (Exception e)
        {
            return false;
        }

        return true;
    }
}