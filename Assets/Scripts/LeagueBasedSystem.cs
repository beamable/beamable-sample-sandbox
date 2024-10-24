using System;
using System.Threading.Tasks;
using Beamable;
using Beamable.Server.Clients;
using UnityEngine;
using System.Collections.Generic;

namespace DefaultNamespace
{
    public class LeagueBasedSystem : MonoBehaviour
    {
        private BeamContext _beamContext;
        private long _userId;
        private ServiceClient _service;

        private async void Start()
        {
            // Initialize Beamable context
            _beamContext = await BeamContext.Default.Instance;
            _userId = _beamContext.PlayerId;
            _service = new ServiceClient();
            
            Debug.Log($"User Id: {_userId}");

            // Example score - this would come from game logic
            const double playerScore = 1500;

            // Check if the player already has a score and which league they're in
            await UpdatePlayerLeague(playerScore);
        }

        private string AssignLeague(double score)
        {
            return score switch
            {
                <= 1000 => "Bronze",
                <= 2000 => "Silver",
                _ => "Gold"
            };
        }

        private async Task UpdatePlayerLeague(double playerScore)
        {
            // Step 1: Get the current league from stats
            var currentLeague = await GetCurrentLeague();

            // Step 2: Assign player to the new league based on their new score
            var newLeague = AssignLeague(playerScore);
            var newLeaderboardId = $"{newLeague}_Leaderboard";
            
            // Step 3: Remove the player from the previous league if they are switching leagues
            if (currentLeague != null && !currentLeague.Equals(newLeague))
            {
                Debug.Log($"Player is switching from {currentLeague} to {newLeague}. Removing from {currentLeague}_Leaderboard.");
                await RemovePlayerFromPreviousLeaderboard($"{currentLeague}_Leaderboard");
            }

            // Step 4: Ensure the new league's leaderboard exists
            await EnsureLeaderboardExists(newLeaderboardId);

            // Step 5: Set the player's score on the new league's leaderboard
            await SetScoreOnLeagueLeaderboard(newLeaderboardId, playerScore);

            // Step 6: Update the player's current league in stats
            await SetCurrentLeague(newLeague);

            // Step 7: Display the updated leaderboard
            await DisplayLeagueLeaderboard(newLeaderboardId);
        }

        private async Task<string> GetCurrentLeague()
        {
            Debug.Log("Retrieving current league from stats...");

            // Retrieve the player's current league from stats
            string access = "public";  
            string domain = "client";
            string type = "player";
            long id = _userId;
            string statKey = "PlayerLeague";

            var stats = await _beamContext.Api.StatsService.GetStats(domain, access, type, id);

            if (stats.TryGetValue(statKey, out string currentLeague))
            {
                Debug.Log($"Current league from stats: {currentLeague}");
                return currentLeague;
            }
            else
            {
                Debug.Log("Player does not have a current league set in stats.");
                return null; // No league stat found, indicating this is the player's first time being assigned to a league
            }
        }

        private async Task SetCurrentLeague(string newLeague)
        {
            Debug.Log($"Setting player's current league to {newLeague} in stats...");

            // Set the player's current league in stats
            string access = "public";  
            var statsDictionary = new Dictionary<string, string>
            {
                { "PlayerLeague", newLeague }
            };

            await _beamContext.Api.StatsService.SetStats(access, statsDictionary);
            Debug.Log("Player's league updated in stats.");
        }

        private async Task RemovePlayerFromPreviousLeaderboard(string leaderboardId)
        {
            // Remove player from the previous leaderboard
            Debug.Log($"Removing player from {leaderboardId}");
            try
            {
                await _service.RemoveLeaderboardScore(leaderboardId);
                Debug.Log($"Player removed from {leaderboardId}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to remove player from {leaderboardId}: {e.Message}");
            }
        }

        private async Task EnsureLeaderboardExists(string leaderboardId)
        {
            Debug.Log($"Checking if leaderboard {leaderboardId} exists...");

            // Check if the leaderboard exists
            var leaderboardExists = await _service.LeaderboardExists(leaderboardId);

            // Create the leaderboard if it doesn't exist
            if (!leaderboardExists)
            {
                Debug.Log($"Leaderboard {leaderboardId} doesn't exist. Creating...");
                await _service.CreateLeaderboard(leaderboardId);
                Debug.Log($"Leaderboard {leaderboardId} created.");
            }
            else
            {
                Debug.Log($"Leaderboard {leaderboardId} already exists.");
            }
        }

        private async Task SetScoreOnLeagueLeaderboard(string leaderboardId, double score)
        {
            Debug.Log($"Setting score on {leaderboardId}");
            await _service.SetLeaderboardScore(leaderboardId, score);
        }

        private async Task DisplayLeagueLeaderboard(string leaderboardId)
        {
            Debug.Log($"Retrieving leaderboard for {leaderboardId}");

            // Get the leaderboard data for the league
            var leaderboardContent = await _beamContext.Api.LeaderboardService.GetBoard(leaderboardId, 0, 10);
            
            // Display the top 10 players in the league
            foreach (var entry in leaderboardContent.rankings)
            {
                Debug.Log($"Player: {entry.gt}, Rank: {entry.rank}, Score: {entry.score}");
            }
        }
    }
}
