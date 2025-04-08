using UnityEngine;

public class SpaceshipThruster : MonoBehaviour
{
    [Header("Thruster Effects")]
    public ParticleSystem leftThruster;  // Assign the left thruster effect in the Inspector
    public ParticleSystem rightThruster; // Assign the right thruster effect in the Inspector

    private void Start()
    {
        // Ensure both thrusters are disabled initially
        if (leftThruster != null) leftThruster.Stop();
        if (rightThruster != null) rightThruster.Stop();
    }

    private void Update()
    {
        bool isMoving = Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow);

        if (isMoving)
        {
            ActivateThrusters();
        }
        else
        {
            DeactivateThrusters();
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
