using UnityEngine;

public class StarMovement : MonoBehaviour
{
    public float speed = 3f;
    public float rotationSpeed;

    private Camera mainCamera;
    private GameController gameController;

    void Start()
    {
        mainCamera = Camera.main;
        gameController = FindFirstObjectByType<GameController>();

        // We'll keep this line to set a random rotation speed
        rotationSpeed = Random.Range(-180f, 180f);
    }

    void Update()
    {
        // Move the star downward (in world space like the asteroid)
        transform.Translate(Vector3.down * speed * Time.deltaTime, Space.World);

        // Rotate the star (this is the same rotation logic as in AsteroidBehaviour)
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);

        // Check if the star is below the screen
        if (IsOutOfBounds())
        {
            // Notify game controller that a star was missed (optional life loss)
            if (gameController != null)
            {
                gameController.StarMissed();
            }

            // Destroy the star
            Destroy(gameObject);
        }
    }

    private bool IsOutOfBounds()
    {
        if (mainCamera != null)
        {
            // Convert world position to viewport position (0-1 range)
            Vector3 viewportPosition = mainCamera.WorldToViewportPoint(transform.position);

            // If star is below the bottom of the screen with a small margin
            return viewportPosition.y < -0.1f;
        }

        // Fallback if camera not found
        return transform.position.y < -10f;
    }
}