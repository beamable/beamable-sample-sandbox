using System;
using System.Threading.Tasks;
using Beamable.Common.Api.Leaderboards;
using Beamable.Server;
using Beamable.Server.Api.Leaderboards;
using UnityEngine;

namespace Beamable.Microservices
{
    [Microservice("Service")]
    public class Service : Microservice
    {
        [ClientCallable]
        public async Task SetLeaderboardScore(string leaderboardId, double score)
        {
            try
            {
                await Services.Leaderboards.SetScore(leaderboardId, score);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                throw;
            }
        }
        
        [ClientCallable]
        public async Task CreateLeaderboard(string leaderboardId)
        {
            try
            {
                await Services.Leaderboards.CreateLeaderboard(leaderboardId, new CreateLeaderboardRequest());
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                throw;
            }
        }

        [ClientCallable]
        public async Task<bool> LeaderboardExists(string leaderboardId)
        {
            try
            {
                var leaderboard = await Services.Leaderboards.GetBoard(leaderboardId, 0, 1);
                return leaderboard != null; // If a board is returned, it exists
            }
            catch (Exception e)
            {
                // Check if the exception contains a 404 error (Not Found)
                if (e.Message.Contains("404") || e.Message.Contains("NotFound"))
                {
                    // Leaderboard doesn't exist, return false
                    Debug.Log($"Leaderboard {leaderboardId} does not exist (404).");
                    return false;
                }

                // Log any other exceptions and rethrow them
                Debug.Log($"Error checking leaderboard existence: {e.Message}");
                throw;
            }
        }

        [ClientCallable]
        public async Task RemoveLeaderboardScore(string leaderboardId)
        {
            try
            {
                Debug.Log(Context.UserId);
                await Services.Leaderboards.RemovePlayerEntry(leaderboardId, Context.UserId);
            }   
            catch (Exception e)
            {
                Debug.Log(e.Message);
                throw;
            }
        }
    }
}

