namespace OsuSteamProvider;

public static class Logger
{
    public static bool Disable = false;
    
    public static void Log(string message)
    {
        if (Disable) return;
        
        foreach (var line in message.Split('\n'))
        {
            Console.Write(GetPrefix());
            Console.WriteLine(line);
        }
    }

    private static string GetPrefix()
    {
        return $"[{DateTime.Now:HH:mm:ss}] [osu!Hosts] ";
    }
}