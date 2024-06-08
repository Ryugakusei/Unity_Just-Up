using Unity.Services.Leaderboards;
using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Leaderboards.Models;

public class LeaderboardManager : MonoBehaviour
{
    private const string leaderboardId = "SpeedGameJam";
    public TextMeshProUGUI leaderboardText;

    public async Task SubmitScore(int score)
    {
        try
        {
            var addPlayerScoreResult = await LeaderboardsService.Instance.AddPlayerScoreAsync(leaderboardId, score);
            Debug.Log("Score submitted successfully");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to submit score: {e.Message}");
        }
    }

    public async void LoadLeaderboard()
    {
        try
        {
            var scores = await LeaderboardsService.Instance.GetScoresAsync(leaderboardId, new GetScoresOptions { Limit = 10 });
            DisplayScores(scores.Results);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load leaderboard: {e.Message}");
        }
    }

    private void DisplayScores(List<LeaderboardEntry> scores)
    {
        leaderboardText.text = "Leaderboard\n";
        foreach (var score in scores)
        {
            leaderboardText.text += $"{score.Rank}. {score.PlayerId}: {score.Score}\n";
        }
    }
}
