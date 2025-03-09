using UnityEngine;

public class StarMovement : MonoBehaviour
{
    public float fallSpeed = 3f;

    void Update()
    {
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;
    }
}
