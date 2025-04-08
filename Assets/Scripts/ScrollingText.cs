using UnityEngine;
using TMPro;

public class ScrollingText : MonoBehaviour
{
    public float speed = 50f; // Adjust speed of movement
    public float moveDistance = 100f; // Adjust how far it moves

    private Vector3 startPosition;
    private bool movingRight = true;

    void Start()
    {
        startPosition = transform.localPosition; // Store the starting position
    }

    void Update()
    {
        float movement = speed * Time.deltaTime;

        if (movingRight)
        {
            transform.localPosition += new Vector3(movement, 0, 0);
            if (transform.localPosition.x >= startPosition.x + moveDistance)
                movingRight = false;
        }
        else
        {
            transform.localPosition -= new Vector3(movement, 0, 0);
            if (transform.localPosition.x <= startPosition.x - moveDistance)
                movingRight = true;
        }
    }
}
