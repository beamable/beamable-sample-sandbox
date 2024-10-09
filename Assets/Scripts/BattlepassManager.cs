using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Beamable;
using Beamable.Common.Content;
using DefaultNamespace;
using UnityEngine;

public class BattlepassManager : MonoBehaviour
{
    [SerializeField] private ContentRef<Battlepass> battlepassRef;
    private Battlepass _battlepass;
    private BeamContext _beamContext;
    private readonly string _lastRewardClaimTimeKey = "LastRewardClaimTime";  // Key for the stat
    private readonly string _access = "public"; 
        
    private async void Start()
    {
        // Get the Beamable context
        _beamContext = await BeamContext.Default.Instance;
            
        // Fetch the Battlepass content
        await battlepassRef.Resolve()
            .Then(content =>
            {
                _battlepass = content;
                Debug.Log($"Fetched Battlepass: {_battlepass.Name}");
                DisplayBattlepassDetails();
            })
            .Error(_ =>
            {
                Debug.LogError("Failed to fetch the Battlepass content.");
            });
    }

    private void DisplayBattlepassDetails()
    {
        Debug.Log($"Battlepass: {_battlepass.Name}");
        foreach (var tier in _battlepass.Tiers)
        {
            Debug.Log($"Tier {tier.Level}");
            foreach (var reward in tier.Rewards)
            {
                Debug.Log($"  Reward: {reward.rewardName}, Quantity: {reward.quantity }");
            }
        }
    }
        
    // Check the last claim time before allowing reward claim
    private async Task TryClaimReward(int tierLevel)
    {
        Debug.Log($"Attempting to claim reward for Tier {tierLevel}...");
        
        // Fetch the last reward claim time from the player's stats
        var playerStats = await _beamContext.Api.StatsService.GetStats("client", _access, "player", _beamContext.PlayerId);
        
        if (playerStats.TryGetValue(_lastRewardClaimTimeKey, out var lastClaimTimeStr))
        {
            var lastClaimTime = DateTime.Parse(lastClaimTimeStr).ToUniversalTime();
            var currentTime = DateTime.UtcNow;
        
            Debug.Log($"Last claim time: {lastClaimTime.ToString("o")} (UTC)");
            Debug.Log($"Current time: {currentTime.ToString("o")} (UTC)");
        
            // Check if enough time has passed since the last claim
            var timeSinceLastClaim = currentTime - lastClaimTime;
            Debug.Log($"Time since last claim: {timeSinceLastClaim.TotalHours} hours");
        
            if (timeSinceLastClaim.TotalHours < 24) // Example: 24 hours between claims
            {
                Debug.Log("Cannot claim reward yet. You must wait until 24 hours have passed.");
                return;
            }
        }
        else
        {
            Debug.Log("No previous reward claim time found. This is the first claim.");
        }
        
        // Proceed with claiming the reward
        ClaimReward(tierLevel);
        
        // Update the last reward claim time
        var updateStats = new Dictionary<string, string>
        {
            { _lastRewardClaimTimeKey, DateTime.UtcNow.ToString("o") } // Save current UTC time in ISO 8601 format
        };
        await _beamContext.Api.StatsService.SetStats(_access, updateStats);
        
        Debug.Log("Reward claimed successfully, and last claim time updated.");
    }
        
    private void ClaimReward(int tierLevel)
    {
        // Log reward claim attempt
        Debug.Log($"Claiming reward for Tier {tierLevel}");
        
        foreach (var tier in _battlepass.Tiers.Where(tier => tier.Level == tierLevel))
        {
            foreach (var reward in tier.Rewards)
            {
                Debug.Log($"  Reward: {reward.rewardName}, Quantity: {reward.quantity}");
            }
            break;
        }
        
        Debug.Log("Reward claim process completed.");
    }
        
    // Method to trigger reward claim on button press
    public async void ClaimRewardButton()
    {
        await TryClaimReward(1);
    }

}