using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable;
using Beamable.Server.Clients;
using UnityEngine;

public class LeaderboardLoadTest : MonoBehaviour
{
    public int numPlayers = 10000; // How many simulated players to create
    public int baseScore = 1000;

    private async void Start()
    {
        Debug.Log("Begin populating leaderboard with dummy players...");

        for (int i = 1406; i < 10000; i++)
        {
            string playerCode = $"test_player_{i}";
            try
            {
                var context = await BeamContext.ForPlayer(playerCode).Instance;
                var client = context.Microservices().Service();

                var stats = new Dictionary<string, object>
                {
                    {"name", $"Player_{i}"},
                    {"tier", UnityEngine.Random.Range(1, 20)},
                    {"video", "https://www.youtube.com/watch?v=dQw4w9WgXcQ"},
                    {"level", UnityEngine.Random.Range(1, 100)}
                };

                int score = baseScore + i;
                var result = await client.SetLeaderboardScore("leaderboards.tcp", score, stats);
                Debug.Log($"[{i}] Submitted score: {score} | success: {result}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error on player {i}: {ex.Message}");
            }
            await Task.Delay(10);

        }

        Debug.Log("Leaderboard population complete.");
    }
}