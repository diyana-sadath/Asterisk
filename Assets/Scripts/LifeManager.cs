using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;

public class LifeManager : MonoBehaviour
{
    public static LifeManager Instance;

    [Header("Life Settings")]
    public int maxLives = 3;
    private int currentLives;

    [Header("UI References")]
    public Image[] lifeIcons; // Assign spaceship UI icons here
    public GameObject gameOverPanel; // Assign this in Inspector

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
        ResetLives();
        gameOverPanel.SetActive(false); // Hide Game Over panel at start
    }

    public void SetLives(int lives)
    {
        currentLives = lives;
        UpdateLivesUI();
    }

    public void LoseLife()
    {
        Debug.Log("Life lost! Current lives: " + currentLives);
        currentLives--;

        // Play life lost sound
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayLifeLost();

        UpdateLivesUI();
        Debug.Log("Lives remaining: " + currentLives);

        if (currentLives <= 0)
        {
            GameOver();
        }
    }

    private void UpdateLivesUI()
    {
        for (int i = 0; i < lifeIcons.Length; i++)
        {
            lifeIcons[i].enabled = (i < currentLives);
        }
    }

    private void GameOver()
    {
        Debug.Log("Game Over!");

        // Play game over sound
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayGameOver();

        gameOverPanel.SetActive(true); // Show Game Over screen
        Time.timeScale = 0f; // Stop the game

        int finalScore = ScoreManager.Instance.GetCurrentScore();
        GameController.Difficulty difficulty = GameController.Instance.selectedDifficulty;

        // Submit the score to PlayFab using the central manager
        PlayFabScoreManager.Instance.SubmitScore(difficulty, finalScore);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // Resume the game
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Restart the scene
    }

    public void ResetLives()
    {
        currentLives = maxLives;
        UpdateLivesUI();
    }

    public int GetCurrentLives()
    {
        return currentLives;
    }
}