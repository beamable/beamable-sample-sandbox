using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Beamable;
using Beamable.Common.Content;
using Beamable.Server.Clients;
using DefaultNamespace;
using UnityEngine;

public class BattlepassManager : MonoBehaviour
{
    [SerializeField] private ContentRef<Battlepass> battlepassRef;
    private Battlepass _battlepass;
    private ServiceClient _service;

    private async void Start()
    {
        _service = new ServiceClient();
        
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
                Debug.Log($"  Reward: {reward.rewardName}, Quantity: {reward.quantity}");
            }
        }
    }

    // Trigger reward claim on button press
    public async void ClaimRewardButton()
    {
        await TryClaimReward(1);
    }

    private async Task TryClaimReward(int tierLevel)
    {
        Debug.Log($"Attempting to claim reward for Tier {tierLevel}...");
        
        // Call the service to check eligibility and claim the reward
        var result = await _service.ClaimReward(tierLevel);
        
        if (result == "Success")
        {
            Debug.Log("Reward claimed successfully.");
        }
        else
        {
            Debug.Log(result);  // Log the error message from the service
        }
    }
}
