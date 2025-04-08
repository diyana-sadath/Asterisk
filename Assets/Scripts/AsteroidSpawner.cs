using System.Collections;
using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    [Header("Asteroid Settings")]
    public GameObject asteroidPrefab;
    public float spawnRate = 2.5f;
    public float spawnRangeX = 8f;
    public float spawnHeight = 6f;
    public float minSpeed = 4f;
    public float maxSpeed = 8f;

    private bool isSpawning = false;
    private Coroutine spawnCoroutine;

    private void Awake()
    {
        // Check if we have the asteroid prefab
        if (asteroidPrefab == null)
        {
            Debug.LogError("⚠️ Asteroid prefab is not assigned! Please assign it in the inspector.");
        }
    }

    public void StartSpawning()
    {
        if (isSpawning)
        {
            Debug.Log("🌑 Asteroid spawner is already running.");
            return;
        }

        if (asteroidPrefab == null)
        {
            Debug.LogError("⚠️ Asteroid prefab is not assigned!");
            return;
        }

        isSpawning = true;
        spawnCoroutine = StartCoroutine(SpawnAsteroids());
        Debug.Log("🌑 Asteroid spawner started with spawn rate: " + spawnRate);
    }

    public void StopSpawning()
    {
        if (!isSpawning)
        {
            Debug.Log("🌑 Asteroid spawner is already stopped.");
            return;
        }

        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }

        isSpawning = false;
        Debug.Log("🛑 Asteroid spawner stopped.");
    }

    private IEnumerator SpawnAsteroids()
    {
        Debug.Log("🌑 Starting asteroid spawn coroutine.");
        int spawnCount = 0;

        while (isSpawning)
        {
            try
            {
                // Create random position at top of screen with random X value
                float randomX = Random.Range(-spawnRangeX, spawnRangeX);
                Vector3 spawnPosition = new Vector3(randomX, spawnHeight, 0);

                // Instantiate the asteroid
                GameObject asteroid = Instantiate(asteroidPrefab, spawnPosition, Quaternion.identity);
                asteroid.tag = "Asteroid"; // Set proper tag for collision detection

                // Set random rotation and speed
                float rotationSpeed = Random.Range(-180f, 180f);
                float movementSpeed = Random.Range(minSpeed, maxSpeed);

                // Add components to the asteroid
                AsteroidBehavior behavior = asteroid.GetComponent<AsteroidBehavior>();
                if (behavior == null)
                {
                    behavior = asteroid.AddComponent<AsteroidBehavior>();
                }
                behavior.Initialize(movementSpeed, rotationSpeed);

                spawnCount++;
                Debug.Log($"🌑 Spawned asteroid #{spawnCount}. Next spawn in {spawnRate} seconds.");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error spawning asteroid: {e.Message}");
            }

            // Wait before spawning next asteroid
            yield return new WaitForSeconds(spawnRate);
        }

        Debug.Log("🌑 Asteroid spawn coroutine ended. Total spawned: " + spawnCount);
    }
}