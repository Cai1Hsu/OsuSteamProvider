﻿using OsuSteamProvider;
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

long lastTimestamp = 0;
const int updateRate = 1; // seconds between update
string lastPresence = string.Empty;
SpinWait.SpinUntil(() =>
{
    SteamProvider.Update();
    
    long currentTimestamp = DateTime.UtcNow.Ticks / (10000000 * updateRate);

    if (OsuProvider.Status is OsuListenerManager.OsuStatus.Unkonwn or OsuListenerManager.OsuStatus.NoFoundProcess) return false;

    if (currentTimestamp == lastTimestamp) return false;
    
    string info = OsuProvider.GetPresenceString();

    if (lastPresence == info && !OsuProvider.HasUpdate) return false;
    
    SteamProvider.SetStatus(info);
    Logger.Log("Updating: " + info);

    return false;
});

OsuProvider.Stop();
SteamProvider.Unregister();