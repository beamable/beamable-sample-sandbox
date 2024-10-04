using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable;
using Beamable.Common.Api.Events;
using UnityEngine;

public class example : MonoBehaviour
{
    private BeamContext _beamContext;
    private double _score = 0;

    async void Start()
    {
        // Initialize Beamable and setup the context
        _beamContext = await BeamContext.Default.Instance;

        Debug.Log($"Player ID: {_beamContext.PlayerId}");

        // Call the method to set the score and level
        SetScoreAndLevelInSpecificEvent();
    }

    public async void SetScoreAndLevelInSpecificEvent()
    {
        _score = 1;  
        
        var eventsGetResponse = await _beamContext.Api.EventsService.GetCurrent();
        
        foreach (EventView eventView in eventsGetResponse.running)
        {
            Debug.Log(eventView.id);
            if (eventView.id == "events.EventTestTemplate.1728047887000")
            {
                // Add LEVEL stat to the event score metadata
                Dictionary<string, object> scoreMetadata = new Dictionary<string, object>
                {
                    { "LEVEL", 2 }  // Set LEVEL stat to 2
                };
                
                // Set score for the specific event and include LEVEL in metadata
                await _beamContext.Api.EventsService.SetScore(
                   eventView.id, _score, false, scoreMetadata);
                
                var scoreLog = $"SetScore()" +
                                  $"\n\tEvent ID = {eventView.id}" +
                                  $"\n\tname = {eventView.name}" +
                                  $"\n\tscore = {_score}" +
                                  $"\n\tLEVEL set to 2 in metadata";
                Debug.Log(scoreLog);
            }
        }

        // Set the LEVEL stat to 2 (client public stat)
        const string statKey = "LEVEL";
        const string access = "public"; // Public access as specified
        Dictionary<string, string> statsDictionary = new Dictionary<string, string>()
        {
            { statKey, "2" }  // Set LEVEL stat to 2
        };
        
        await _beamContext.Api.StatsService.SetStats(access, statsDictionary);
        
        // Log the stat update
        Debug.Log("LEVEL stat set to 2");

        // Force refresh after the update
        _beamContext.Api.EventsService.Subscribable.ForceRefresh();
        await Task.Delay(200);  // Wait for refresh to complete
    }
}
