using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;



public class GameWonManager : MonoBehaviour
{
    public static GameWonManager Instance;

    [Header("Game Won UI")]
    public GameObject gameWonPanel; // Assign the panel in the Inspector

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Ensure the panel is hidden at the start
        if (gameWonPanel != null)
        {
            gameWonPanel.SetActive(false);
        }
    }

    public void ShowGameWonScreen()
    {
        if (gameWonPanel != null)
        {
            // Play game win sound
            if (SoundManager.Instance != null)
                SoundManager.Instance.PlayGameWin();

            gameWonPanel.SetActive(true);
            Time.timeScale = 0f; // Pause the game

            int finalScore = ScoreManager.Instance.GetCurrentScore();
            string difficulty = GameController.Instance.selectedDifficulty.ToString();

            // Submit the score to PlayFab
            SubmitScoreToPlayFab(difficulty, finalScore);

        }
    }

    private void SubmitScoreToPlayFab(string difficulty, int score)
    {
        string statisticName = difficulty + "Score"; // e.g., "EasyScore"
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
            result => Debug.Log("Successfully submitted score."),
            error => Debug.LogError("Error submitting score: " + error.GenerateErrorReport())
        );
    }

}
