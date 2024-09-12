using System.Collections;
using System.Collections.Generic;
using Beamable;
using UnityEngine;
using Beamable.Server.Clients;

public class StatExample : MonoBehaviour
{
    private async void Start()
    {
        // Setup Beamable context
        var context = BeamContext.Default;
        await context.OnReady;
        await context.Accounts.OnReady;
        Debug.Log(context.Accounts.Current.GamerTag);

        // Get player ID
        long playerId = context.PlayerId;
        Debug.Log($"PlayerId: {playerId}");

        // Define stat key and access
        string statKey = "GAMER_TAG";
        string access = "public"; // Use "private" if you want this stat to be private
        string domain = "client"; // or "game" for backend
        string type = "player";   // Legacy parameter

        // Set the GAMER_TAG stat to the player's PlayerId
        Dictionary<string, string> setStats = new Dictionary<string, string>()
        {
            { statKey, playerId.ToString() }
        };

        // Set the stat
        await context.Api.StatsService.SetStats(access, setStats);
        Debug.Log($"Set GAMER_TAG stat with PlayerId: {playerId}");

        // Optionally, retrieve and log the stat to verify
        Dictionary<string, string> getStats = await context.Api.StatsService.GetStats(domain, access, type, playerId);
        if (getStats.TryGetValue(statKey, out string retrievedValue))
        {
            Debug.Log($"Retrieved GAMER_TAG stat: {retrievedValue}");
        }
        else
        {
            Debug.Log("Failed to retrieve GAMER_TAG stat.");
        }
    }
}