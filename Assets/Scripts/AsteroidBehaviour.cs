using UnityEngine;

public class AsteroidBehavior : MonoBehaviour
{
    private float speed;
    private float rotationSpeed;
    private ScoreManager scoreManager;
    private LifeManager lifeManager;

    void Start()
    {
        // Find managers if needed
        if (GameObject.FindFirstObjectByType<ScoreManager>() != null)
        {
            scoreManager = GameObject.FindFirstObjectByType<ScoreManager>();
        }

        if (GameObject.FindFirstObjectByType<LifeManager>() != null)
        {
            lifeManager = GameObject.FindFirstObjectByType<LifeManager>();
        }

        // Destroy asteroid after 10 seconds if it somehow doesn't get destroyed
        Destroy(gameObject, 10f);
    }

    public void Initialize(float moveSpeed, float rotSpeed)
    {
        speed = moveSpeed;
        rotationSpeed = rotSpeed;
    }

    void Update()
    {
        // Move asteroid downward
        transform.Translate(Vector3.down * speed * Time.deltaTime, Space.World);

        // Rotate asteroid
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);

        // Destroy if it goes off screen
        if (transform.position.y < -7f)
        {
            Destroy(gameObject);
        }
    }

   
}