using UnityEngine;
using UnityEngine.UI;

public class SpaceshipDamageEffects : MonoBehaviour
{
    [Header("Hit Effects")]
    public float flashDuration = 0.15f;
    public Color hitFlashColor = Color.red;
    public Image screenFlashOverlay;  // Assign a full-screen UI Image in canvas
    public float screenShakeAmount = 0.2f;
    public float screenShakeDuration = 0.2f;

    private SpriteRenderer shipRenderer;
    private Color originalColor;
    private Vector3 originalPosition;
    private bool isFlashing = false;
    private float flashTimer = 0f;
    private float shakeTimer = 0f;
    private bool isShaking = false;

    void Start()
    {
        shipRenderer = GetComponent<SpriteRenderer>();
        if (shipRenderer != null)
            originalColor = shipRenderer.color;

        originalPosition = transform.position;

        // Make sure the screen flash overlay starts invisible
        if (screenFlashOverlay != null)
        {
            Color transparent = hitFlashColor;
            transparent.a = 0f;
            screenFlashOverlay.color = transparent;
        }
    }

    void Update()
    {
        // Handle ship flash effect
        if (isFlashing)
        {
            flashTimer -= Time.deltaTime;
            if (flashTimer <= 0)
            {
                EndFlash();
            }
        }

        // Handle screen shake effect
        if (isShaking)
        {
            shakeTimer -= Time.deltaTime;

            if (shakeTimer > 0)
            {
                Vector3 randomOffset = Random.insideUnitSphere * screenShakeAmount;
                transform.position = originalPosition + new Vector3(randomOffset.x, randomOffset.y, 0);
            }
            else
            {
                transform.position = originalPosition;
                isShaking = false;
            }
        }

        // Handle screen flash fade out
        if (screenFlashOverlay != null && screenFlashOverlay.color.a > 0)
        {
            Color fadeColor = screenFlashOverlay.color;
            fadeColor.a = Mathf.Max(0, fadeColor.a - (Time.deltaTime * 2f));
            screenFlashOverlay.color = fadeColor;
        }
    }

    public void TriggerHitEffects()
    {
        // Start ship flash
        if (shipRenderer != null && !isFlashing)
        {
            shipRenderer.color = hitFlashColor;
            isFlashing = true;
            flashTimer = flashDuration;
        }

        // Start screen shake
        if (!isShaking)
        {
            originalPosition = transform.position;
            isShaking = true;
            shakeTimer = screenShakeDuration;
        }

        // Show screen flash overlay
        if (screenFlashOverlay != null)
        {
            Color flashColor = hitFlashColor;
            flashColor.a = 0.3f;  // Semi-transparent
            screenFlashOverlay.color = flashColor;
        }
    }

    private void EndFlash()
    {
        if (shipRenderer != null)
            shipRenderer.color = originalColor;
        isFlashing = false;
    }
}