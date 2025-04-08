using UnityEngine;
using UnityEngine.SceneManagement;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;


public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverPanel; // Assign this in the Inspector

    private void Start()
    {
        gameOverPanel.SetActive(false); // Hide the panel at the start
    }

    public void ShowGameOverScreen()
    {
        gameOverPanel.SetActive(true); // Show Game Over screen
        Time.timeScale = 0f; // Pause the game
        int finalScore = ScoreManager.Instance.GetCurrentScore();
        string difficulty = GameController.Instance.selectedDifficulty.ToString();

            // Submit the score to PlayFab
        SubmitScoreToPlayFab(difficulty, finalScore);
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

    public void RestartGame()
    {
        Time.timeScale = 1f; // Resume time
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reload the current scene
        Debug.Log("Restart button clicked!");

    }
}
