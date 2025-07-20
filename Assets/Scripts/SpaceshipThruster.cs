using UnityEngine;

public class SpaceshipThruster : MonoBehaviour
{
    [Header("Thruster Effects")]
    public ParticleSystem leftThruster;  // Assign the left thruster effect in the Inspector
    public ParticleSystem rightThruster; // Assign the right thruster effect in the Inspector

    [Header("Tilt Sensitivity")]
    public float tiltThreshold = 0.1f;  // Minimum tilt required to activate thrusters

    private Vector3 neutralAcceleration = Vector3.zero;
    private bool isCalibrated = false;

    private void Start()
    {
        // Ensure both thrusters are disabled initially
        if (leftThruster != null) leftThruster.Stop();
        if (rightThruster != null) rightThruster.Stop();

        // Check if we have calibration data available
        if (CalibrationSessionManager.Instance != null && CalibrationSessionManager.Instance.IsCalibrated)
        {
            neutralAcceleration = CalibrationSessionManager.Instance.NeutralAcceleration;
            isCalibrated = true;
        }
        else if (AccelerationDataManager.IsCalibrated)
        {
            neutralAcceleration = AccelerationDataManager.LoadCalibrationData();
            isCalibrated = true;
        }
    }

    private void Update()
    {
        bool isMovingLeft = Input.GetKey(KeyCode.LeftArrow);
        bool isMovingRight = Input.GetKey(KeyCode.RightArrow);
        bool isMovingByTilt = false;

        // Check for tilt input if IMUReceiverManager is available and calibrated
        if (isCalibrated && IMUReceiverManager.Instance != null)
        {
            Vector3 currentAcceleration = IMUReceiverManager.Instance.LatestAcceleration;
            Vector3 relativeMovement = currentAcceleration - neutralAcceleration;

            // Detect significant tilt on X axis (left/right)
            if (Mathf.Abs(relativeMovement.x) > tiltThreshold)
            {
                isMovingByTilt = true;

                // Update the left/right flags based on tilt direction
                if (relativeMovement.x > 0)
                {
                    isMovingLeft = true;
                }
                else
                {
                    isMovingRight = true;
                }
            }
        }

        if (isMovingLeft || isMovingRight || isMovingByTilt)
        {
            // Activate specific thrusters based on movement direction
            if (isMovingLeft)
            {
                ActivateThruster(rightThruster);  // Right thruster pushes ship left
                DeactivateThruster(leftThruster);
            }
            else if (isMovingRight)
            {
                ActivateThruster(leftThruster);   // Left thruster pushes ship right
                DeactivateThruster(rightThruster);
            }
            else
            {
                // Activate both if there's some other kind of movement
                ActivateThrusters();
            }
        }
        else
        {
            DeactivateThrusters();
        }
    }

    private void ActivateThruster(ParticleSystem thruster)
    {
        if (thruster != null && !thruster.isPlaying)
        {
            thruster.Play();
        }
    }

    private void ActivateThrusters()
    {
        if (leftThruster != null && !leftThruster.isPlaying)
        {
            leftThruster.Play();
        }

        if (rightThruster != null && !rightThruster.isPlaying)
        {
            rightThruster.Play();
        }
    }

    private void DeactivateThruster(ParticleSystem thruster)
    {
        if (thruster != null && thruster.isPlaying)
        {
            thruster.Stop();
        }
    }

    private void DeactivateThrusters()
    {
        if (leftThruster != null && leftThruster.isPlaying)
        {
            leftThruster.Stop();
        }

        if (rightThruster != null && rightThruster.isPlaying)
        {
            rightThruster.Stop();
        }
    }
}