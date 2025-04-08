using UnityEngine;

public class SpaceshipController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private float baseSensitivity = 1f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (SensitivityManager.Instance != null)
        {
            baseSensitivity = SensitivityManager.Instance.Sensitivity;
            Debug.Log($"SpaceshipController loaded sensitivity: {baseSensitivity}");
        }
        else
        {
            baseSensitivity = PlayerPrefs.GetFloat("SensitivityLevel", 3f); // Direct 1–5 scale
            Debug.Log($"Fallback sensitivity loaded: {baseSensitivity}");
        }
    }

    private void FixedUpdate()
    {
        if (IMUReceiverManager.Instance != null)
        {
            Vector3 accel = IMUReceiverManager.Instance.LatestAcceleration;
            float moveX = Mathf.Clamp(-accel.x * baseSensitivity, -1f, 1f);

            if (baseSensitivity > 1f)
            {
                float sensitivityFactor = Mathf.Pow(baseSensitivity, 0.75f);
                moveX = Mathf.Sign(moveX) * Mathf.Pow(Mathf.Abs(moveX), 1f / sensitivityFactor);
            }

            Vector2 moveDirection = new Vector2(moveX, 0f);
            float adjustedSpeed = moveSpeed * Mathf.Lerp(0.5f, 1.5f, (baseSensitivity - 1f) / 4f);
            rb.linearVelocity = moveDirection * adjustedSpeed;

            if (Time.frameCount % 100 == 0)
            {
                Debug.Log($"Movement: accel={accel.x}, sens={baseSensitivity}, moveX={moveX}, speed={adjustedSpeed}");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Asteroid"))
        {
            SoundManager.Instance?.PlayAsteroidHit();
            LifeManager.Instance.LoseLife();
            Destroy(other.gameObject);

            if (LifeManager.Instance.GetCurrentLives() <= 0)
                GameOver();
        }
    }

    private void GameOver()
    {
        Debug.Log("Game Over!");
    }
}
