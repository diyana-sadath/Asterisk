using System.Collections;
using UnityEngine;

public class StarSpawner : MonoBehaviour
{
    public GameObject starPrefab; // Drag the star prefab in Inspector
    public float spawnRate = 0.5f; // How often stars spawn
    public float zigZagWidth = 2f; // Distance for zig-zag movement
    public float starFallSpeed = 3f; // Speed of falling stars
    public int difficultyLevel = 1; // 1 = Easy, 2 = Medium, 3 = Hard

    private bool spawnLeft = true;
    private float startY = 5f; // Starting Y position

    void Start()
    {
        SetDifficultySettings();
        StartCoroutine(SpawnStars());
    }

    void SetDifficultySettings()
    {
        switch (difficultyLevel)
        {
            case 1: // Easy Mode
                spawnRate = 1f;
                zigZagWidth = 2.5f;
                starFallSpeed = 2f;
                break;
            case 2: // Medium Mode
                spawnRate = 0.7f;
                zigZagWidth = 3f;
                starFallSpeed = 3f;
                break;
            case 3: // Hard Mode
                spawnRate = 0.5f;
                zigZagWidth = 4f;
                starFallSpeed = 4f;
                break;
        }
    }

    IEnumerator SpawnStars()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnRate);

            float xOffset = spawnLeft ? -zigZagWidth : zigZagWidth;
            spawnLeft = !spawnLeft; // Toggle direction

            Vector3 spawnPosition = new Vector3(xOffset, startY, 0);
            GameObject star = Instantiate(starPrefab, spawnPosition, Quaternion.identity);

            // Attach the movement script dynamically
            StarMovement starMove = star.AddComponent<StarMovement>();
            starMove.fallSpeed = starFallSpeed;
        }
    }
}
