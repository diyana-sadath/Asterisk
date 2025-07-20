using UnityEngine;
using System.Collections;

public class Star : MonoBehaviour
{
    [Header("Collection Effects")]
    public float collectionAnimationDuration = 0.3f;
    public float expandSize = 1.8f;
    public float rotationSpeed = 360f; // degrees per second
    public Color glowColor = new Color(1f, 1f, 0.6f, 1f); // Warm white/yellow glow
    public int sparkleCount = 5; // Number of small stars to spawn

    private SpriteRenderer spriteRenderer;
    private bool isCollected = false;
    private Vector3 originalScale;
    private Color originalColor;

    private void Start()
    {
        // Get the sprite renderer component
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
        originalScale = transform.localScale;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isCollected) // Check if the spaceship collects it
        {
            isCollected = true;

            // Play collection sound
            if (SoundManager.Instance != null)
                SoundManager.Instance.PlayStarCollect();

            // Add 10 points to score
            ScoreManager.Instance.AddScore(10);

            // Play collection animation
            StartCoroutine(CollectionAnimation());
        }
    }

    private IEnumerator CollectionAnimation()
    {
        // Disable collider to prevent multiple collections
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
            collider.enabled = false;

        // Create sparkle effect mini-stars
        CreateSparkles();

        // Animation timing variables
        float elapsed = 0f;

        // Run animation
        while (elapsed < collectionAnimationDuration)
        {
            // Calculate animation progress (0 to 1)
            float t = elapsed / collectionAnimationDuration;

            // Sine curve for smoother animation (starts slow, speeds up, then slows down)
            float smoothT = Mathf.Sin(t * Mathf.PI * 0.5f);

            // Scale up with smooth easing
            transform.localScale = Vector3.Lerp(originalScale, originalScale * expandSize, smoothT);

            // Add slight rotation
            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);

            // Brighten and fade
            if (spriteRenderer != null)
            {
                // First brighten to glow color, then fade out
                if (t < 0.5f)
                {
                    // Brighten to glow color in first half
                    spriteRenderer.color = Color.Lerp(originalColor, glowColor, t * 2f);
                }
                else
                {
                    // Fade out in second half
                    Color fadeColor = glowColor;
                    fadeColor.a = 1 - ((t - 0.5f) * 2f);
                    spriteRenderer.color = fadeColor;
                }
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Remove the star from the scene
        Destroy(gameObject);
    }

    private void CreateSparkles()
    {
        // Create small star sparkles that fly outward
        for (int i = 0; i < sparkleCount; i++)
        {
            // Create a small clone of this star as a sparkle
            GameObject sparkle = new GameObject("StarSparkle");
            sparkle.transform.position = transform.position;

            // Add sprite renderer with same sprite
            SpriteRenderer sparkleRenderer = sparkle.AddComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                sparkleRenderer.sprite = spriteRenderer.sprite;
                sparkleRenderer.color = glowColor;
            }

            // Make it small
            sparkle.transform.localScale = originalScale * Random.Range(0.2f, 0.4f);

            // Add random movement component
            StarSparkle sparkleScript = sparkle.AddComponent<StarSparkle>();

            // Set random direction
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            sparkleScript.Initialize(direction, Random.Range(1f, 3f), Random.Range(0.3f, 0.6f));
        }
    }
}

// Helper class for the small star sparkles
public class StarSparkle : MonoBehaviour
{
    private Vector2 direction;
    private float speed;
    private float lifetime;
    private float elapsed = 0f;
    private Vector3 originalScale;
    private SpriteRenderer spriteRenderer;

    public void Initialize(Vector2 direction, float speed, float lifetime)
    {
        this.direction = direction;
        this.speed = speed;
        this.lifetime = lifetime;
        originalScale = transform.localScale;
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Add a random spin
        StartCoroutine(SpinAndFade());
    }

    private IEnumerator SpinAndFade()
    {
        while (elapsed < lifetime)
        {
            // Move outward
            transform.Translate(direction * speed * Time.deltaTime);

            // Spin
            transform.Rotate(0, 0, 360f * Time.deltaTime);

            // Scale down towards the end
            if (elapsed > lifetime * 0.5f)
            {
                float scaleT = (elapsed - lifetime * 0.5f) / (lifetime * 0.5f);
                transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, scaleT);
            }

            // Fade out
            if (spriteRenderer != null)
            {
                Color color = spriteRenderer.color;
                color.a = Mathf.Lerp(1f, 0f, elapsed / lifetime);
                spriteRenderer.color = color;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}