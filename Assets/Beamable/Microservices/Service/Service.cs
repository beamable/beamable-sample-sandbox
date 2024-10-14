using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable.Server;
using UnityEngine;

namespace Beamable.Microservices
{
    [Microservice("Service")]
    public class Service : Microservice
    {
        private readonly string _lastRewardClaimTimeKey = "LastRewardClaimTime";  // Key for the stat

        [ClientCallable]
        public async Task<string> ClaimReward(int tierLevel)
        {
            // Fetch player stats to check the last claim time
            var playerId = Context.UserId;
            var playerStats = await Services.Stats.GetStats("client", "public", "player", playerId);
            
            if (playerStats.TryGetValue(_lastRewardClaimTimeKey, out var lastClaimTimeStr))
            {
                var lastClaimTime = DateTime.Parse(lastClaimTimeStr).ToUniversalTime();
                var currentTime = DateTime.UtcNow;

                // Check if 24 hours have passed since the last claim
                var timeSinceLastClaim = currentTime - lastClaimTime;
                if (timeSinceLastClaim.TotalHours < 24)
                {
                    return $"Cannot claim reward yet. Please wait for {24 - timeSinceLastClaim.TotalHours:F2} hours.";
                }
            }
            
            // Proceed with the reward claim
            await ProcessRewardClaim(tierLevel);
            
            // Update the last claim time in stats
            var updateStats = new Dictionary<string, string>
            {
                { _lastRewardClaimTimeKey, DateTime.UtcNow.ToString("o") }
            };
            await Services.Stats.SetStats("public", updateStats);

            return "Success";
        }

        private Task ProcessRewardClaim(int tierLevel)
        {
            // Logic to handle the reward claim
            // This is where you'd grant the player their rewards (e.g., add items to inventory)
            Debug.Log($"Processing reward for Tier {tierLevel}");

            // Simulate the reward process for now
            return Task.CompletedTask;
        }
    }
}
