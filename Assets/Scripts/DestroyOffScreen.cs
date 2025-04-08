using UnityEngine;

public class DestroyOffScreen : MonoBehaviour
{
    private float lowerBound;

    private void Start()
    {
        lowerBound = Camera.main.transform.position.y - Camera.main.orthographicSize - 1f;
    }

    private void Update()
    {
        if (transform.position.y < lowerBound)
        {
            Destroy(gameObject);
        }
    }
}
