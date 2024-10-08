using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable;
using Beamable.Common.Api.Events;
using Beamable.Server.Clients;
using UnityEngine;

public class Example : MonoBehaviour
{
    private BeamContext _beamContext;
    private double _score = 0;
    private ServiceClient _service;

    async void Start()
    {
        // Initialize Beamable and setup the context
        Debug.Log("Initializing Beamable context...");
        _beamContext = await BeamContext.Default.Instance;
        _service = new ServiceClient();
        
        Debug.Log($"Player ID: {_beamContext.PlayerId}");

        // Call the method to set the score and level
        Debug.Log("Calling SetScoreAndLevelInSpecificEvent...");
        await SetScoreAndLevelInSpecificEvent();
    }

    public async Task SetScoreAndLevelInSpecificEvent()
    {
        _score = 1;  // Example score

        Debug.Log("Fetching player's LEVEL stat...");
        const string statKey = "LEVEL";
        const string access = "public"; // Public access as specified
        const string domain = "client";
        const string type = "player";
        long playerId = _beamContext.PlayerId;

        // Retrieve the player's LEVEL stat
        Dictionary<string, string> statsDictionary;
        try
        {
            statsDictionary = await _beamContext.Api.StatsService.GetStats(domain, access, type, playerId);
            Debug.Log("Successfully fetched player's stats.");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error fetching player's stats: {e.Message}");
            return;
        }

        int playerLevel;

        // If stat doesn't exist, set it to a default value 
        if (!statsDictionary.TryGetValue(statKey, out string levelString))
        {
            playerLevel = 30; // Default value for new players
            Dictionary<string, string> newStatsDictionary = new Dictionary<string, string>()
            {
                { statKey, playerLevel.ToString() }
            };

            // Set the stat to the default value
            try
            {
                await _beamContext.Api.StatsService.SetStats(access, newStatsDictionary);
                Debug.Log($"LEVEL stat not found. Setting LEVEL stat to default value: {playerLevel}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Error setting default LEVEL stat: {e.Message}");
                return;
            }
        }
        else
        {
            // Parse the stat if it exists
            if (int.TryParse(levelString, out playerLevel))
            {
                Debug.Log($"Player LEVEL stat found: {playerLevel}");
            }
            else
            {
                Debug.LogError($"Error parsing LEVEL stat: {levelString}");
                return;
            }
        }

        Debug.Log($"Player's final LEVEL stat: {playerLevel}");

        // Step 2: Get the current events
        Debug.Log("Fetching current events...");
        EventsGetResponse eventsGetResponse;
        try
        {
            eventsGetResponse = await _beamContext.Api.EventsService.GetCurrent();
            Debug.Log("Successfully fetched current events.");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error fetching current events: {e.Message}");
            return;
        }

        // Process events
        foreach (EventView eventView in eventsGetResponse.running)
        {
            Debug.Log($"Processing event: {eventView.id}");

            if (eventView.id == "events.EventTestTemplate.1728390016000")
            {
                Debug.Log($"Event '{eventView.id}' matches the target event.");

                // Step 3: Determine the leaderboard name based on player's level
                string leaderboardName = GetLeaderboardNameForCohort(playerLevel, eventView.id);
                Debug.Log($"Determined leaderboard name: {leaderboardName}");

                // Step 4: Create the appropriate leaderboard if it doesn't exist
                Debug.Log($"Creating/checking leaderboard: {leaderboardName}");
                await CreateLeaderboardIfNotExists(eventView.id, leaderboardName);

                // Step 5: Add score to the leaderboard
                Dictionary<string, object> scoreMetadata = new Dictionary<string, object>
                {
                    { "LEVEL", playerLevel }
                };

                try
                {
                    Debug.Log($"Setting score {_score} in leaderboard: {leaderboardName}");
                    await _service.SetLeaderboardScore(leaderboardName, _score);

                    Debug.Log("Setting score in event with metadata...");
                    await _beamContext.Api.EventsService.SetScore(
                        eventView.id, _score, false, scoreMetadata);

                    Debug.Log($"Score successfully submitted to leaderboard: {leaderboardName}");
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error setting score in leaderboard or event: {e.Message}");
                }
            }
        }
    }

    // Helper function to create the leaderboard name based on player's level
    private string GetLeaderboardNameForCohort(int playerLevel, string eventId)
    {
        Debug.Log($"Generating leaderboard name for level {playerLevel}...");
        if (playerLevel <= 10)
        {
            return $"{eventId}_BelowLevel10Leaderboard";
        }
        else
        {
            return $"{eventId}_AboveLevel10Leaderboard";
        }
    }

    // Helper function to create the leaderboard if it doesn't already exist
    private async Task CreateLeaderboardIfNotExists(string eventId, string leaderboardName)
    {
        try
        {
            // Attempt to create the leaderboard
            Debug.Log($"Attempting to create leaderboard: {leaderboardName}");
            await _service.CreateLeaderboard(leaderboardName);

            Debug.Log($"Leaderboard '{leaderboardName}' created successfully.");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error creating leaderboard '{leaderboardName}': {e.Message}");
        }
    }
}
