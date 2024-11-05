using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable;
using Beamable.Common.Api.Leaderboards;
using Beamable.Server.Clients;
using UnityEngine;

namespace DefaultNamespace
{
    public class CachingExample : MonoBehaviour
    {
        private BeamContext _beamContext;
        private long _userId;
        
        // Cache for leaderboard data
        private readonly Dictionary<string, (DateTime timestamp, List<RankEntry> data)> _leaderboardCache = new();
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(5);  // Cache expiration time

        private async void Start()
        {
            // Initialize Beamable context
            _beamContext = await BeamContext.Default.Instance;
            _userId = _beamContext.PlayerId;
            
            Debug.Log($"User Id: {_userId}");

        }


        // Methods for caching and displaying leaderboard
        public async void OnShowBronzeLeaderboardButtonClicked()
        {
            await DisplayLeagueLeaderboard("Bronze_Leaderboard");
        }

        public async void OnShowSilverLeaderboardButtonClicked()
        {
            await DisplayLeagueLeaderboard("Silver_Leaderboard");
        }

        private async Task DisplayLeagueLeaderboard(string leaderboardId)
        {
            List<RankEntry> leaderboardData;

            if (_leaderboardCache.TryGetValue(leaderboardId, out var cacheEntry) &&
                DateTime.Now - cacheEntry.timestamp < _cacheDuration)
            {
                Debug.Log($"Loading {leaderboardId} from cache.");
                leaderboardData = cacheEntry.data;
            }
            else
            {
                Debug.Log($"Fetching {leaderboardId} from API.");
                var leaderboardContent = await _beamContext.Api.LeaderboardService.GetBoard(leaderboardId, 0, 10);
                leaderboardData = leaderboardContent.rankings;

                _leaderboardCache[leaderboardId] = (DateTime.Now, leaderboardData); // Update cache
            }

            // Display leaderboard
            foreach (var entry in leaderboardData)
            {
                Debug.Log($"Player: {entry.gt}, Rank: {entry.rank}, Score: {entry.score}");
            }
        }
    }
}
