using System.Threading.Tasks;
using Beamable;
using Beamable.Common.Leaderboards;
using UnityEngine;

namespace DefaultNamespace
{
    public class LeagueBasedSystem: MonoBehaviour
    {
    private BeamContext _beamContext;
    private long _userId;

    private async void Start()
    {
        // Initialize Beamable context
        _beamContext = await BeamContext.Default.Instance;
        _userId = _beamContext.PlayerId;
        
        Debug.Log($"User Id: {_userId}");

        // Example score - this would come from game logic
        double playerScore = 1500;

        // Step 1: Assign player to a league based on their score
        var league = AssignLeague(playerScore);
        Debug.Log($"Player assigned to league: {league}");

        // Step 2: Set the player's score on the corresponding league leaderboard
        await SetScoreOnLeagueLeaderboard(league, playerScore);

        // Step 3: Display the leaderboard for the player's league
        await DisplayLeagueLeaderboard(league);
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

    private async Task SetScoreOnLeagueLeaderboard(string league, double score)
    {
        var leaderboardId = $"{league}_Leaderboard";
        
        Debug.Log($"Setting score on {leaderboardId}");

        // Set the player's score on the league-specific leaderboard
        await _beamContext.Api.LeaderboardService.SetScore(leaderboardId, score);
    }

    private async Task DisplayLeagueLeaderboard(string league)
    {
        string leaderboardId = $"{league}_Leaderboard";

        Debug.Log($"Retrieving leaderboard for {league}");

        // Get the leaderboard data for the league
        var leaderboardContent = await _beamContext.Api.LeaderboardService.GetBoard(leaderboardId, 0, 6);
        
        // Display the top 10 players in the league
        var leaderboardView = await _beamContext.Api.LeaderboardService.GetBoard(leaderboardId, 0, 10);
        foreach (var entry in leaderboardView.rankings)
        {
            Debug.Log($"Player: {entry.gt}, Rank: {entry.rank}, Score: {entry.score}");
        }
    }

    }
}