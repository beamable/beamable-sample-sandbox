using System;
using System.Threading.Tasks;
using Beamable;
using UnityEngine;
using System.Collections.Generic;

namespace DefaultNamespace
{
    public class LeagueBasedSystem : MonoBehaviour
    {
        private BeamContext _beamContext;
        private long _userId;
        private const string TournamentId = "tournaments.default"; // Replace with your actual tournament ID

        private async void Start()
        {
            // Initialize Beamable context
            _beamContext = await BeamContext.Default.Instance;
            _userId = _beamContext.PlayerId;
            
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

            // Step 2: Determine the player's new league based on their updated score
            var newLeague = AssignLeague(playerScore);

            // Step 3: If the player is switching leagues, handle the transition
            if (currentLeague != null && !currentLeague.Equals(newLeague))
            {
                Debug.Log($"Player is switching from {currentLeague} to {newLeague}.");
            }

            // Step 4: Join the correct tournament tier (league) if not already joined
            await JoinLeagueTournament(newLeague, playerScore);

            // Step 5: Set the player's score in the correct tournament tier
            await SetScoreInTournament(playerScore);

            // Step 6: Update the player's current league in stats
            await SetCurrentLeague(newLeague);

            // Step 7: Display the updated rankings for the player's league
            await DisplayLeagueRankings(newLeague);
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

        private async Task JoinLeagueTournament(string leagueTier, double score)
        {
            Debug.Log($"Checking if player is already in tournament {TournamentId} in tier {leagueTier}...");

            try
            {
                // Check if the player is already part of the tournament
                var playerStatusResponse = await _beamContext.Api.TournamentsService.GetPlayerStatus(TournamentId);
                bool isAlreadyJoined = playerStatusResponse.statuses.Exists(status => status.tournamentId == TournamentId);

                if (!isAlreadyJoined)
                {
                    // Join the tournament if the player is not already part of it
                    await _beamContext.Api.TournamentsService.JoinTournament(TournamentId, score);
                    Debug.Log($"Player joined tournament {TournamentId} in tier {leagueTier}.");
                }
                else
                {
                    Debug.Log("Player is already part of the tournament.");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error checking or joining tournament: {e.Message}");
            }
        }

        private async Task SetScoreInTournament(double score)
        {
            Debug.Log($"Setting score in tournament {TournamentId}...");
            await _beamContext.Api.TournamentsService.SetScore(TournamentId, _userId, score);
        }

        private async Task DisplayLeagueRankings(string leagueTier)
        {
            Debug.Log($"Retrieving rankings for league tier: {leagueTier} in tournament {TournamentId}...");

            // Get and display the rankings for the current league's tier
            try
            {
                var leaderboardContent = await _beamContext.Api.TournamentsService.GetStandings(TournamentId, 0, 10);

                foreach (var entry in leaderboardContent.entries)
                {
                    Debug.Log($"Player: {entry.playerId}, Rank: {entry.rank}, Score: {entry.score}");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error fetching rankings: {e.Message}");
            }
        }
    }
}
