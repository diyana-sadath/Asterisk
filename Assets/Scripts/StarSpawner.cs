using UnityEngine;

public class StarSpawner : StarSpawnerBase
{
    private float timer;
    private bool isTimeUp = false;

    public enum Difficulty { Easy, Medium, Hard }
    public Difficulty selectedDifficulty = Difficulty.Medium;

    private void Start()
    {
        ConfigureSpawner();
        StartSpawning();
    }

    private void ConfigureSpawner()
    {
        switch (selectedDifficulty)
        {
            case Difficulty.Easy:
                spawnRate = 1.5f;
                minSpeed = 2f;
                maxSpeed = 5f;
                SetupEasyMode();
                break;
            case Difficulty.Medium:
                spawnRate = 1.2f;
                minSpeed = 3f;
                maxSpeed = 6f;
                SetupMediumMode();
                break;
            case Difficulty.Hard:
                spawnRate = 0.8f;
                minSpeed = 5f;
                maxSpeed = 9f;
                SetupHardMode();
                break;
        }
    }

    private void SetupEasyMode()
    {
        if (GameTimer.Instance != null)
        {
            GameTimer.Instance.StartTimer(true);
            timer = GameTimer.Instance.gameDuration;  // This will now work!
        }
        else
        {
            Debug.LogError("GameTimer instance not found! Make sure GameTimer is present in the scene.");
            timer = 120f;
        }
    }


    private void SetupMediumMode()
    {
        if (GameTimer.Instance != null)
        {
            GameTimer.Instance.StartTimer(false);
        }
    }

    private void SetupHardMode()
    {
        if (GameTimer.Instance != null)
        {
            GameTimer.Instance.StartTimer(false);
        }
    }

    private void Update()
    {
        if (selectedDifficulty == Difficulty.Easy && !isTimeUp)
        {
            timer -= Time.deltaTime;
            GameTimer.Instance?.UpdateTimerDisplay(timer);

            if (timer <= 0f)
            {
                isTimeUp = true;
                StopSpawning();
                GameTimer.Instance.TimeUp();
            }
        }
    }
}
