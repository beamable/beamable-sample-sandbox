using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable;
using Beamable.Common.Shop;
using UnityEngine;

public class IAPExample : MonoBehaviour
{
    [SerializeField] private ListingRef listingRef;

    private BeamContext _beamContext;
    private DateTime _endTime;

    private async void Start()
    {
        _beamContext = await BeamContext.Default.Instance;
        Debug.Log($"Beamable initialized, PlayerId: {_beamContext.PlayerId}");

        // Fetch the initial store and get the endTime
        await FetchListingEndTime();

        // Fetch the current stat value before simulation
        await FetchStatValue("MAIN_CAMPAIGN_PROGRESS");

        // Simulate a stat change after 5 seconds
        Invoke(nameof(SimulateStatChange), 5f);
    }

    private async Task FetchListingEndTime()
    {
        try
        {
            Debug.Log("Fetching store view...");
            var storeView = await _beamContext.Api.CommerceService.GetCurrent("stores.Store01");
            Debug.Log("Store view fetched successfully.");

            foreach (var listing in storeView.listings)
            {
                if (listing.symbol == listingRef.Id)
                {
                    Debug.Log("Matching listing found.");
                    _endTime = listing.endTime;
                    Debug.Log($"Listing End Time: {_endTime}");
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error fetching listing endTime: {ex.Message}");
        }
    }

    private async Task FetchStatValue(string statKey)
    {
        try
        {
            Debug.Log($"Fetching current stat value for {statKey}...");
            var stats = await _beamContext.Api.StatsService.GetStats("client", "public", "player", _beamContext.PlayerId);

            if (stats.TryGetValue(statKey, out string statValue))
            {
                Debug.Log($"{statKey} value before change: {statValue}");
            }
            else
            {
                Debug.LogWarning($"Stat {statKey} not found or has no value.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error fetching stat value: {ex.Message}");
        }
    }

    private async void SimulateStatChange()
    {
        Debug.Log("Simulating stat change for MAIN_CAMPAIGN_PROGRESS...");

        string statKey = "MAIN_CAMPAIGN_PROGRESS";
        Dictionary<string, string> statsDictionary = new Dictionary<string, string> { { statKey, "8" } };

        try
        {
            // Set new stat value
            await _beamContext.Api.StatsService.SetStats("public", statsDictionary);
            Debug.Log($"Updated {statKey} to value: 8");

            // Fetch the updated stat value
            await FetchStatValue(statKey);

            // Re-fetch the listing endTime after stat change
            await FetchListingEndTime();
            Debug.Log($"Listing End Time after stat change: {_endTime}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error updating stat or fetching endTime: {ex.Message}");
        }
    }
}
