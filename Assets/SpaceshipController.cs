using UnityEngine;

public class SpaceshipController : MonoBehaviour
{
    public float moveSpeed = 5f; // Speed of movement
    public float maxTiltAngle = 30f; // Maximum tilt angle for movement
    public float screenBounds = 2.5f; // Restrict spaceship within screen bounds

    private float roll = 0f; // IMU roll data
    private IMUReceiver imuReceiver;

    void Start()
    {
        imuReceiver = FindFirstObjectByType<IMUReceiver>();

        if (imuReceiver == null)
        {
            Debug.LogError("IMUReceiver not found! Make sure it is in the scene.");
        }
    }

    void Update()
    {
        if (imuReceiver != null)
        {
            roll = imuReceiver.GetRoll();
            Debug.Log("Roll in SpaceshipController: " + roll); // ✅ Debug output
        }

        // Convert roll into horizontal movement
        float moveDirection = Mathf.Clamp(roll, -1f, 1f);
        float newX = transform.position.x + moveDirection * moveSpeed * Time.deltaTime;

        // Clamp spaceship within screen bounds
        newX = Mathf.Clamp(newX, -screenBounds, screenBounds);

        // Apply movement
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);

        // Rotate spaceship based on roll
        transform.rotation = Quaternion.Euler(0f, 0f, -roll * maxTiltAngle);
        Debug.Log("Applied Rotation: " + (-roll * maxTiltAngle)); // ✅ Debug output
    }
}
