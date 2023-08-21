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

        bool useUnicode1 = UseUnicode && !string.IsNullOrEmpty(Beatmap.ArtistUnicode);
        bool useUnicode2 = UseUnicode && !string.IsNullOrEmpty(Beatmap.TitleUnicode);
        string artist = useUnicode1 ? Beatmap.ArtistUnicode : Beatmap.Artist;
        string title = useUnicode2 ? Beatmap.TitleUnicode : Beatmap.Title;

        string beatmapString = $"{artist} - {title}";

        HasUpdate = false;
        
        switch (Status)
        {
            case OsuStatus.Rank:
            case OsuStatus.Playing:
                string mods = GetModsString();
                
                if (mods.ToLower().Contains("auto")) prefix = "Watching";
                if (mods.ToLower().Contains("error")) mods = string.Empty;
                
                return $"{prefix} {beatmapString}[{Beatmap.Difficulty}] {mods}";
            
            default:
                return $"{prefix} {beatmapString}";
        }
    }

    public static string GetModsString()
    {
        var modsInfo = new ModsInfo();

        modsInfo.Mod = PlayMods;

        if (!modsInfo.HasMod(PlayMods)) return "NM";

        return modsInfo.ShortName;
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