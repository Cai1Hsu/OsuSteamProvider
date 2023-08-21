using OsuSteamProvider;
using Steamworks;
using OsuRTDataProvider;
using OsuRTDataProvider.Listen;
using Logger = OsuSteamProvider.Logger;

SteamProvider.Register();

// for McOsu
SteamProvider.SetupRichPresenceLocalization();

SteamProvider.SetStatus("Connecting to osu!");

OsuProvider.Start();
OsuProvider.RegisterDebug();


ArgumentParser.AddOperation("-h", "Print *help* table",(_) => ArgumentParser.GenerateHelp());
ArgumentParser.AddOperation("--help", "Print *help* table",(_) => ArgumentParser.GenerateHelp());
ArgumentParser.AddOperation("--no-debug", "no debug output",(_) => Logger.Disable = true);
ArgumentParser.ParseAndExecute(args);

Console.CancelKeyPress += (_, _) =>
{
    OsuProvider.Stop();
    SteamProvider.Unregister();
};

OsuProvider.Instence.OnBeatmapChanged += _ => UpdatePresense();
OsuProvider.Instence.OnStatusChanged += (_, _) => UpdatePresense();

long lastTimestamp = 0;
SpinWait.SpinUntil(() =>
{
    SteamProvider.Update();
    return false;
});

OsuProvider.Stop();
SteamProvider.Unregister();

void UpdatePresense()
{
    string info = OsuProvider.GetPresenceString();
    
    SteamProvider.SetStatus(info);
    Logger.Log("Updating: " + info);
}