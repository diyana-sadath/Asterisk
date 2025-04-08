using UnityEngine;

public class HardStarSpawner : StarSpawnerBase
{
    public AsteroidSpawner asteroidSpawner; // Reference to the Asteroid Spawner

    protected override void Awake()
    {
        base.Awake();
        spawnRate = 0.8f;
        minSpeed = 5f;
        maxSpeed = 9f;
    }

    private void Start()
    {
        if (GameTimer.Instance != null)
        {
            GameTimer.Instance.StartTimer(true);
            GameTimer.Instance.SetTime(60f);
        }

        Debug.Log("✅ Hard mode spawner started.");
    }

    public override void StartSpawning()
    {
        base.StartSpawning();

        // Start asteroid spawning (Only if assigned)
        if (asteroidSpawner != null)
        {
            Debug.Log("🌠 Starting Asteroid Spawner from HardStarSpawner...");
            asteroidSpawner.StartSpawning();
        }
        else
        {
            Debug.LogWarning("⚠️ AsteroidSpawner is NOT assigned in the Inspector!");
        }
    }

    public override void StopSpawning()
    {
        base.StopSpawning();
        Debug.Log("🛑 Hard mode star spawner stopped.");

        // Stop asteroid spawner if assigned
        if (asteroidSpawner != null)
        {
            Debug.Log("🌑 Stopping Asteroid Spawner from HardStarSpawner...");
            asteroidSpawner.StopSpawning();
        }
        else
        {
            Debug.LogWarning("⚠️ AsteroidSpawner is NULL, cannot stop it!");
        }
    }

    public override void ResumeSpawning()
    {
        base.ResumeSpawning();

        // Resume asteroid spawning too
        if (asteroidSpawner != null)
        {
            Debug.Log("🌠 Resuming Asteroid Spawner...");
            asteroidSpawner.StartSpawning();
        }
    }
}