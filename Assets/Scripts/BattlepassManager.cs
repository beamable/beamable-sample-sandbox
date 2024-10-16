using System;
using System.Threading.Tasks;
using Beamable;
using Beamable.Common.Content;
using Beamable.Server.Clients;
using DefaultNamespace;
using UnityEngine;

public class BattlepassManager : MonoBehaviour
{
    [SerializeField] private ContentRef<Battlepass> battlepassRef;
    private BeamContext _beamContext;
    private Battlepass _battlepass;
    private ServiceClient _service;

    private async void Start()
    {
        Debug.Log("Starting BattlepassManager...");
        _beamContext = await BeamContext.Default.Instance;        
        _service = _beamContext.Microservices().Service();
        
        // Fetch the Battlepass content
        await battlepassRef.Resolve()
            .Then(content =>
            {
                Debug.Log("Battlepass content fetched successfully.");
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
        Debug.Log($"Displaying details for Battlepass: {_battlepass.Name}");
        
        if (_battlepass.Tiers == null || _battlepass.Tiers.Count == 0)
        {
            Debug.LogWarning("No tiers found in the Battlepass.");
            return;
        }

        foreach (var tier in _battlepass.Tiers)
        {
            Debug.Log($"Processing Tier {tier.Level}");
            
            if (tier.Rewards == null || tier.Rewards.Count == 0)
            {
                Debug.LogWarning($"No rewards found in Tier {tier.Level}");
            }
            else
            {
                foreach (var reward in tier.Rewards)
                {
                    Debug.Log($"  Reward: {reward.rewardName}, Quantity: {reward.quantity}");
                }
            }
            
        }
    }

    // Trigger reward claim on button press
    public async void ClaimRewardButton()
    {
        Debug.Log("ClaimRewardButton pressed. Attempting to claim reward...");
        await TryClaimReward(1);
    }

    private async Task TryClaimReward(int tierLevel)
    {
        Debug.Log($"Attempting to claim reward for Tier {tierLevel}...");

        // Call the service to check eligibility and claim the reward
        try
        {
            var result = await _service.ClaimReward(tierLevel);
            if (result == "Success")
            {
                Debug.Log("Reward claimed successfully.");
            }
            else
            {
                Debug.LogWarning($"Reward claim failed: {result}");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error claiming reward: {ex.Message}");
        }
    }
}
