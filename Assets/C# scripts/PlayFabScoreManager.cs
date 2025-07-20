using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;

public class PlayFabScoreManager : MonoBehaviour
{
    public static PlayFabScoreManager Instance;

    // Dictionary to map difficulty enum to statistic name
    private Dictionary<GameController.Difficulty, string> difficultyToStatistic = new Dictionary<GameController.Difficulty, string>
    {
        { GameController.Difficulty.Easy, "EasyScore" },
        { GameController.Difficulty.Medium, "MediumScore" },
        { GameController.Difficulty.Hard, "HardScore" }
    };

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SubmitScore(GameController.Difficulty difficulty, int score)
    {
        if (!PlayFabClientAPI.IsClientLoggedIn())
        {
            Debug.LogWarning("Cannot submit score: Player not logged in to PlayFab");
            return;
        }

        // Get the correct statistic name based on difficulty
        if (!difficultyToStatistic.TryGetValue(difficulty, out string statisticName))
        {
            Debug.LogError($"Unknown difficulty: {difficulty}");
            return;
        }

        Debug.Log($"Submitting score {score} to {statisticName} leaderboard");

        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = statisticName,
                    Value = score
                }
            }
        };

        PlayFabClientAPI.UpdatePlayerStatistics(request,
            result => Debug.Log($"Successfully submitted score {score} to {statisticName}"),
            error => Debug.LogError("Error submitting score: " + error.GenerateErrorReport())
        );
    }
}