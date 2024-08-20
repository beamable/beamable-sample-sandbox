using System;
using System.Threading.Tasks;
using Beamable;
using Beamable.Api.Payments;
using Beamable.Common.Shop;
using JetBrains.Annotations;
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

        // Get and log the initial endTime
        await InitializeOffer();
        InvokeRepeating(nameof(UpdateCountdown), 0f, 1f);
    }

    private async Task InitializeOffer()
    {
        try
        {
            var playerListingView = await GetPlayerListingView();
            _endTime = playerListingView.endTime;
            Debug.Log($"Initial offer end time: {_endTime}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to initialize offer: {ex.Message}");
        }
    }

    private async Task<PlayerListingView> GetPlayerListingView()
    {
        try
        {
            Debug.Log("Fetching current store view...");
            var storeView = await _beamContext.Api.CommerceService.GetCurrent("stores.default");

            foreach (var listing in storeView.listings)
            {
                if (listing.symbol == listingRef.Id)
                {
                    Debug.Log("Matching listing found.");
                    return listing;
                }
            }

            throw new Exception("Listing not found.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error fetching store view: {ex.Message}");
            throw;
        }
    }



    private void UpdateCountdown()
    {
        var remainingTime = _endTime - DateTime.UtcNow;
        if (remainingTime > TimeSpan.Zero)
        {
            Debug.Log($"{remainingTime.Hours:D2}:{remainingTime.Minutes:D2}:{remainingTime.Seconds:D2} remaining");
        }
        else
        {
            Debug.Log("Offer expired!");
            CancelInvoke(nameof(UpdateCountdown));
        }
    }
}
