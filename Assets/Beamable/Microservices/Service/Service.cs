using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable.Server;
using UnityEngine;

namespace Beamable.Microservices
{
    [Microservice("Service")]
    public class Service : Microservice
    {
        [AdminOnlyCallable]
        public async Task SetIsVipStat(long playerId)
        {
            try
            {
                Debug.Log("Entered SetIsVipStat method.");

                // Log the player ID being processed
                Debug.Log($"Processing VIP flag update for Player ID: {playerId}");

                // Assume the context for the player
                var assumed = AssumeNewUser(playerId);
                Debug.Log($"Assumed context for Player ID: {playerId}");

                // Define stat details
                string statKey = "vip_flag";
                string access = "public";
                string newValue = "false";

                // Log stat details
                Debug.Log($"Preparing to update stat: {statKey} with value: {newValue} and access level: {access}");

                // Prepare the stats dictionary
                Dictionary<string, string> stats = new Dictionary<string, string>
                {
                    { statKey, newValue }
                };

                // Update stats
                await assumed.Services.Stats.SetStats(access, stats);
                Debug.Log($"Successfully updated '{statKey}' for Player ID: {playerId} with value: {newValue}");
            }
            catch (System.Exception ex)
            {
                // Log any errors encountered during the process
                Debug.LogError($"Failed to update VIP flag for Player ID: {playerId}. Exception: {ex.Message}");
            }
        }
    }
}