using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public class GameController : MonoBehaviour
{
    [Header("Game Components")]
    public EasyStarSpawner easySpawner;
    public MediumStarSpawner mediumSpawner;
    public HardStarSpawner hardSpawner;
    public LifeManager lifeManager;
    public ScoreManager scoreManager;
    public GameObject pauseMenu;
    public GameObject gameOverPanel;
    public int missedStarPenalty = 5;
    private PauseMenuController pauseMenuController;

    public static GameController Instance { get; private set; }

    [Header("Game Settings")]
    private bool isGamePaused = false;
    private bool isGameOver = false;
    private StarSpawnerBase activeSpawner;

    public enum Difficulty { Easy, Medium, Hard }
    [Header("Difficulty Settings")]
    public Difficulty selectedDifficulty = Difficulty.Medium;

    void Awake()
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

    void Start()
    {
        pauseMenuController = FindFirstObjectByType<PauseMenuController>();
        ResumeGame();
        HideGameOver();
        if (pauseMenu != null)
            pauseMenu.SetActive(false);

        // Load difficulty setting from PlayerPrefs - use GetInt instead of HasKey
        int savedDifficulty = PlayerPrefs.GetInt("SelectedDifficulty", 1); // Default to Medium (1) if not set
        selectedDifficulty = (Difficulty)savedDifficulty;
        Debug.Log("Loaded difficulty setting: " + selectedDifficulty);

        SetDifficulty(selectedDifficulty);
    }

    public bool IsGamePaused()
    {
        return isGamePaused;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (isGameOver) return;
        if (isGamePaused) ResumeGame();
        else PauseGame();
    }

    public void PauseGame()
    {
        if (isGamePaused) return; // Prevent multiple calls

        Time.timeScale = 0f;
        isGamePaused = true;
        activeSpawner?.StopSpawning();

        if (pauseMenu != null)
            pauseMenu.SetActive(true);
    }

    public void ResumeGame()
    {
        if (pauseMenu != null)
            pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isGamePaused = false;
        activeSpawner?.ResumeSpawning();
    }

    public void GameOver()
    {
        isGameOver = true;
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
        activeSpawner?.StopSpawning();
        foreach (GameObject star in GameObject.FindGameObjectsWithTag("Star"))
        {
            Destroy(star);
        }
    }

    public void RestartGame()
    {
        lifeManager?.ResetLives();
        scoreManager?.ResetScore();
        HideGameOver();
        isGameOver = false;
        SetDifficulty(selectedDifficulty);
        ResumeGame();
    }

    public void StarMissed()
    {
        // Do nothing - just let the missed star disappear
    }


    public void LoseLifeDueToLowScore()
    {
        if (lifeManager != null)
        {
            lifeManager.LoseLife();
        }

        if (lifeManager.GetCurrentLives() <= 0)
        {
            GameOver();
        }
    }


    public void RestartGameWithLives(int lives)
    {
        if (lifeManager != null)
        {
            lifeManager.SetLives(lives); // Assuming SetLives() exists in LifeManager
        }

        scoreManager?.ResetScore();
        HideGameOver();
        isGameOver = false;
        SetDifficulty(selectedDifficulty);
        ResumeGame();
    }


    private void HideGameOver()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    public void SetDifficulty(Difficulty difficulty)
    {
        selectedDifficulty = difficulty;

        // Stop and disable ALL spawners first to ensure only one runs
        easySpawner.gameObject.SetActive(false);
        mediumSpawner.gameObject.SetActive(false);
        hardSpawner.gameObject.SetActive(false);

        easySpawner.StopSpawning();
        mediumSpawner.StopSpawning();
        hardSpawner.StopSpawning();

        // Assign the correct spawner based on difficulty
        switch (difficulty)
        {
            case Difficulty.Easy:
                activeSpawner = easySpawner;
                break;
            case Difficulty.Medium:
                activeSpawner = mediumSpawner;
                break;
            case Difficulty.Hard:
                activeSpawner = hardSpawner;
                break;
        }

        if (GameTimer.Instance != null)
        {
            GameTimer.Instance.StartTimer(true);
            if (difficulty == Difficulty.Medium)
            {
                GameTimer.Instance.SetTime(60f);
                Debug.Log("Medium Mode: Timer set to 60 seconds");
            }
            else if (difficulty == Difficulty.Easy)
            {
                GameTimer.Instance.SetTime(120f);
                Debug.Log("Easy Mode: Timer set to 120 seconds");
            }
            else if (difficulty == Difficulty.Hard)
            {
                GameTimer.Instance.SetTime(180f);
                Debug.Log("Hard Mode: Timer set to 180 seconds");
            }
        }

        // Enable and start the selected spawner
        if (activeSpawner != null)
        {
            activeSpawner.gameObject.SetActive(true);
            activeSpawner.StartSpawning();
        }
    }
}