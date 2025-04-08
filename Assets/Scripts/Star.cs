using UnityEngine;

public class Star : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Check if the spaceship collects it
        {
            // Play collection sound
            if (SoundManager.Instance != null)
                SoundManager.Instance.PlayStarCollect();

            ScoreManager.Instance.AddScore(10); // Add 10 points to score
            Destroy(gameObject); // Remove the star from the scene
        }
    }
}