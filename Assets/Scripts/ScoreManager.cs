using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("UI References")]
    public TextMeshProUGUI scoreText;

    [Header("Score Settings")]
    public int startingScore = 0;
    public string scorePrefix = "Score: ";

    private int currentScore;

    void Awake()
    {
        // Ensure only one instance exists
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // Prevent duplicate instances
        }
    }

    void Start()
    {
        // Initialize score
        currentScore = startingScore;
        UpdateScoreDisplay();
    }

    public void AddScore(int points)
    {
        currentScore += points;
        UpdateScoreDisplay();
    }

    public void DeductScore(int amount) // <-- Added this method
    {
        currentScore -= amount;
        if (currentScore < 0)
        {
            currentScore = 0; // Prevent negative scores
        }
        UpdateScoreDisplay();
    }

    public void ResetScore()
    {
        currentScore = startingScore;
        UpdateScoreDisplay();
    }

    private void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = scorePrefix + currentScore.ToString();
        }
    }

    public int GetCurrentScore()
    {
        return currentScore;
    }
}
