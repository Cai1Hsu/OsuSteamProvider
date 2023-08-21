using System.Diagnostics;
using OsuRTDataProvider.BeatmapInfo;
using OsuRTDataProvider.Listen;
using OsuRTDataProvider.Mods;
using static OsuRTDataProvider.Listen.OsuListenerManager;
using OsuRTDataProviderSetting = OsuRTDataProvider.Setting;

namespace OsuSteamProvider;

public static class OsuProvider
{
    public static readonly OsuListenerManager Instence = new OsuListenerManager();

    public static string GetPresenceString()
    {
        string prefix = @"Connecting to osu";
        
        if (Status is OsuStatus.NoFoundProcess or OsuStatus.Unkonwn) return prefix;

        switch (Status)
        {
            case OsuStatus.Lobby:
            case OsuStatus.Idle:
            case OsuStatus.MatchSetup:
            case OsuStatus.SelectSong:
                prefix = @"Listening to";
                break;
            
            case OsuStatus.Rank:
            case OsuStatus.Playing:
                prefix = @"Playing";
                break;
            
            case OsuStatus.Editing:
                prefix = "Editing";
                break;
        }

        string beatmapString = UseUnicode ? $"{Beatmap.ArtistUnicode} - {Beatmap.TitleUnicode}" : $"{Beatmap.Artist} - {Beatmap.Title}";

        HasUpdate = false;
        
        switch (Status)
        {
            case OsuStatus.Rank:
            case OsuStatus.Playing:
                return $"{prefix} {beatmapString}[{Beatmap.Difficulty}] {GetModsString()}";
            
            default:
                return $"{prefix} {beatmapString}";
        }
    }

    public static string GetModsString()
    {
        var modsInfo = new ModsInfo();

        modsInfo.Mod = PlayMods;

        if (!modsInfo.HasMod(PlayMods)) return "NM";

        return modsInfo.ToString();
    }

    public static void RegisterDebug()
    {
        Instence.OnStatusChanged += (pre, current) =>
        {
            if (pre != current)
                Logger.Log("new status: " + current);
        };
        
        Instence.OnPlayerChanged += player => Logger.Log("new player: " + player);
        Instence.OnModsChanged += modsInfo => Logger.Log("new mods: " + modsInfo.Mod);
        Instence.OnBeatmapChanged += beatmap => Logger.Log("new beatmap: " + beatmap.Title);
    }
    
    public static void Start()
    {
        OsuRTDataProviderSetting.DisableProcessNotFoundInformation = true;
        OsuRTDataProviderSetting.ListenInterval = 1000;
        
        Instence.OnPlayerChanged += player => Player = player;
        Instence.OnModsChanged += modsInfo => PlayMods = modsInfo.Mod;
        Instence.OnBeatmapChanged += beatmap => Beatmap = beatmap;
        Instence.OnStatusChanged += (_, current) => Status = current;

        Instence.OnModsChanged += _ => HasUpdate = true;
        Instence.OnBeatmapChanged += _ => HasUpdate = true;
        Instence.OnStatusChanged += (_, _) => HasUpdate = true;
        
        Instence.Start();
    }
    
    public static  void Stop() => Instence.Stop();

    public static OsuStatus Status = OsuStatus.NoFoundProcess;

    public static  string Player = "unknown";

    public static  ModsInfo.Mods PlayMods;

    public static Beatmap Beatmap = Beatmap.Empty;

    public static bool UseUnicode = true;

    public static bool HasUpdate = true;
}