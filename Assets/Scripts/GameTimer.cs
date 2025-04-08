using UnityEngine;
using TMPro;
using System.Collections;

public class GameTimer : MonoBehaviour
{
    public static GameTimer Instance { get; private set; } // Singleton

    public TMP_Text timerText; // UI text element for timer
    public GameObject gameWonPanel; // Assign in the Inspector

    private float currentTime;
    private bool isTimerRunning = false;
    private bool isTimerVisible = false;
    // At the top of GameTimer.cs, add:
    public DifficultyDropdown difficultyDropdown; // Assign this in the inspector
    public float gameDuration { get; private set; }  // Stores the total game duration

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

    public void SetGameDuration(float duration)
    {
        gameDuration = duration;
        currentTime = duration;
    }

    public float GameDuration => gameDuration;

    public void StartTimer(bool showUI)
    {
        if (currentTime <= 0)
        {
            Debug.LogError("Game duration not set!");
            return;
        }

        isTimerVisible = showUI;
        timerText.gameObject.SetActive(showUI);
        isTimerRunning = true;
    }

    private void Update()
    {
        if (!isTimerRunning) return;

        currentTime -= Time.deltaTime;
        UpdateTimerDisplay(currentTime);

        if (currentTime <= 0f)
        {
            isTimerRunning = false;
            currentTime = 0f;
            TimeUp();
        }
    }

    // In GameTimer.cs
    public void TimeUp()
    {
        Debug.Log("Time up! Current score: " + ScoreManager.Instance.GetCurrentScore() +
                  " Required score: " + GetRequiredScore());

        if (GameController.Instance == null || LifeManager.Instance == null)
        {
            Debug.LogError("GameController or LifeManager not found!");
            return;
        }

        int currentScore = ScoreManager.Instance.GetCurrentScore();
        int requiredScore = GetRequiredScore();

        // Add more debug logs to track what's happening
        Debug.Log($"Checking score: Current={currentScore}, Required={requiredScore}, Difficulty={GameController.Instance.selectedDifficulty}");

        if (currentScore >= requiredScore)
        {
            Debug.Log("Score requirement met! Unlocking next difficulty...");
            UnlockNextDifficulty(); // This will now use our persistent manager
            GameWonManager.Instance.ShowGameWonScreen();
        }
        else
        {
            Debug.Log("Score requirement not met. Current: " + currentScore + " Required: " + requiredScore);
            if (!isDrainingLives)
            {
                StartCoroutine(DrainLives());
            }
        }
    }


    private void UnlockNextDifficulty()
    {
        // Find or create the DifficultyUnlockManager
        DifficultyUnlockManager unlockManager = FindAnyObjectByType<DifficultyUnlockManager>();

        if (unlockManager == null)
        {
            // Create new GameObject with DifficultyUnlockManager if it doesn't exist
            GameObject managerObject = new GameObject("DifficultyUnlockManager");
            unlockManager = managerObject.AddComponent<DifficultyUnlockManager>();
            DontDestroyOnLoad(managerObject); // Make sure it persists between scenes
            Debug.Log("Created new DifficultyUnlockManager in gameplay scene");
        }

        // Now unlock the appropriate difficulty based on current difficulty
        switch (GameController.Instance.selectedDifficulty)
        {
            case GameController.Difficulty.Easy:
                Debug.Log("Unlocking Medium difficulty...");
                unlockManager.CompleteEasyMode();
                break;

            case GameController.Difficulty.Medium:
                Debug.Log("Unlocking Hard difficulty...");
                unlockManager.CompleteMediumMode();
                break;
        }
    }
    private int GetRequiredScore()
    {
        switch (GameController.Instance.selectedDifficulty)
        {
            case GameController.Difficulty.Medium:
                return 125;
            case GameController.Difficulty.Hard:
                return 300;
            default:
                return 200; // Easy mode
        }
    }

    private void GameWon()
    {
        Debug.Log("Game Won!");
        gameWonPanel.SetActive(true); // Show win screen
        Time.timeScale = 0f; // Pause game
    }

    private bool isDrainingLives = false;

    private IEnumerator DrainLives()
    {
        isDrainingLives = true;

        while (LifeManager.Instance.GetCurrentLives() > 0)
        {
            LifeManager.Instance.LoseLife();
            yield return new WaitForSeconds(1f);

            if (LifeManager.Instance.GetCurrentLives() <= 0)
            {
                break;
            }
        }

        isDrainingLives = false;
        GameController.Instance.GameOver();
    }

    public void SetTime(float time)
    {
        gameDuration = time;
        currentTime = time;
        UpdateTimerDisplay(currentTime);
    }

    public void UpdateTimerDisplay(float time)
    {
        if (timerText != null && isTimerVisible)
        {
            timerText.text = "Time Left: " + Mathf.CeilToInt(time) + "s";
        }
    }
}