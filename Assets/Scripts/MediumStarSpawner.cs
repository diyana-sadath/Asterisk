using UnityEngine;

public class MediumStarSpawner : StarSpawnerBase
{
    private void Start()
    {
        spawnRate = 1.2f;
        minSpeed = 3f;
        maxSpeed = 6f;

        if (GameTimer.Instance != null)
        {
            GameTimer.Instance.SetTime(60f); // ✅ First, set the correct timer value
            GameTimer.Instance.StartTimer(false); // ✅ Then start the timer
        }

        StartSpawning();
    }


}
