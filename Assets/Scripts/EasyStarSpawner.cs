using UnityEngine;

public class EasyStarSpawner : StarSpawnerBase
{
    private float timer;
    private bool isTimeUp = false;

    protected override void Awake()
    {
        base.Awake();
        spawnRate = 0.8f;
        minSpeed = 1.5f;
        maxSpeed = 3.5f;
    }

    private void Start()
    {
        if (GameTimer.Instance != null)
        {
            GameTimer.Instance.SetGameDuration(120f);
            GameTimer.Instance.StartTimer(true);
            timer = GameTimer.Instance.gameDuration;
        }
        else
        {
            Debug.LogError("GameTimer instance not found! Using default fallback timer.");
            timer = 120f;
        }

        StartSpawning(); // This should now correctly use the base class spawning
    }

    private void Update()
    {
        if (isTimeUp) return;

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