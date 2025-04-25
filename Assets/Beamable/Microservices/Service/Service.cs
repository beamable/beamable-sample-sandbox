using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable.Common;
using Beamable.Common.Api.Groups;
using Beamable.Server;
using UnityEngine;

namespace Beamable.Microservices
{
    [Microservice("Service")]
    public class Service : Microservice
    {
        [ClientCallable]
        private async Promise<bool> SetLeaderboardScore(string leaderboardId, int score, Dictionary<string, object> leaderboardScores = null)
        {
            try
            {
                await Services.Leaderboards.SetScore(leaderboardId, score, leaderboardScores);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to add leaderboard entry: {e.Message}");
                return false;
            }
        }

        [ClientCallable]
        private async Task<string> GetLeaderboard()
        {
            var leaderboard = await Services.Leaderboards.GetBoard("leaderboards.tcp", 0, 10);
            return leaderboard.rankgt.ToString();
        }
    }
}