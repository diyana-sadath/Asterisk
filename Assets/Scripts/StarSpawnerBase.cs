using UnityEngine;
using System.Collections;

public abstract class StarSpawnerBase : MonoBehaviour
{
    [Header("Star Settings")]
    public GameObject starPrefab;
    public float spawnRate;
    public float minSpeed;
    public float maxSpeed;

    [Header("Spawn Area")]
    public bool useScreenBounds = true;
    public float spawnWidth = 10f;
    public float spawnHeight = 5f;

    private Camera mainCamera;
    private bool isGameRunning = false;
    private Coroutine spawnCoroutine;

    protected virtual void Awake()
    {
        mainCamera = Camera.main;

        if (starPrefab == null)
        {
            starPrefab = Resources.Load<GameObject>("Star");
            if (starPrefab == null)
            {
                Debug.LogError("Star prefab is not assigned! Please assign a star prefab in the inspector.");
                return;
            }
        }
    }

    public virtual void StartSpawning()
    {
        if (!isGameRunning)
        {
            isGameRunning = true;
            spawnCoroutine = StartCoroutine(SpawnStars());
        }
    }

    private IEnumerator SpawnStars()
    {
        while (isGameRunning)
        {
            Vector3 spawnPos = GetRandomSpawnPosition();
            GameObject star = Instantiate(starPrefab, spawnPos, Quaternion.identity);
            star.tag = "Star";

            StarMovement starMovement = star.GetComponent<StarMovement>() ?? star.AddComponent<StarMovement>();
            starMovement.speed = Random.Range(minSpeed, maxSpeed);

            yield return new WaitForSeconds(1f / spawnRate);
        }
    }

    protected Vector3 GetRandomSpawnPosition()
    {
        float randomX;
        float spawnY;

        if (useScreenBounds && mainCamera != null)
        {
            Vector3 screenBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCamera.transform.position.z));
            randomX = Random.Range(-screenBounds.x * 0.9f, screenBounds.x * 0.9f);
            spawnY = screenBounds.y + 1f;
        }
        else
        {
            randomX = Random.Range(-spawnWidth / 2, spawnWidth / 2);
            spawnY = spawnHeight;
        }

        return new Vector3(randomX, spawnY, 0f);
    }

    public virtual void StopSpawning()
    {
        CancelInvoke(nameof(SpawnStars));
        isGameRunning = false;

        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }

        StopAllCoroutines();
    }

    public virtual void ResumeSpawning()
    {
        StartSpawning();
    }
}