namespace OsuSteamProvider;

public static class ArgumentParser
{
    private static Dictionary<string, Action<Argument>> _pattern = new();

    private static Dictionary<string, string> _usages = new();

    public static void ParseAndExecute(string[] args)
    {
        foreach (var arg in args)
        {
            var split = arg.Split();
            var key = split[0];
            var val = split.Length > 1 ? split[1] : string.Empty;

            if (_pattern.TryGetValue(key, out var action))
            {
                action(new Argument(key, val));
            }
            else
            {
                throw new InvalidOperationException(
                    string.Format("Unrecognized key: ", key));
            }
        }
    }
    
    public static void AddOperation(string key, string usage,Action<Argument> action)
    {
        _pattern.Add(key, action);
        _usages.Add(key, usage);
    }
    
    public static void GenerateHelp()
    {
        Logger.Log("Add rich presence to osu! with Steam(using the appid of McOsu).");
        Logger.Log("Start osu! and this program will automatically update your status.");
        
        Logger.Log("Usage:");
        
        foreach (var usage in _usages)
        {
            Logger.Log($"\t{usage.Value}");
        }
    }
}

public class Argument
{
    public string Key { get; set; }
    
    public string Value { get; set; }

    public bool HasValue => !string.IsNullOrEmpty(Value);
    
    public Argument(string key, string value)
    {
        Key = key;
        Value = value;
    }

    public Argument(string key)
    {
        Key = key;
        Value = String.Empty;
    }
}