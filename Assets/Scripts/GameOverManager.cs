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
        GameController.Difficulty difficulty = GameController.Instance.selectedDifficulty;

        // Submit the score to PlayFab using the central manager
        PlayFabScoreManager.Instance.SubmitScore(difficulty, finalScore);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // Resume time
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reload the current scene
        Debug.Log("Restart button clicked!");
    }
}