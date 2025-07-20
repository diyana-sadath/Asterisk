using UnityEngine;

public class DynamicBackground : MonoBehaviour
{
    public Material nebulaMaterial;
    public float scrollSpeed = 0.05f;
    public float parallaxFactor = 0.5f;

    private Vector2 uvOffset = Vector2.zero;
    private Renderer backgroundRenderer;

    void Start()
    {
        backgroundRenderer = GetComponent<Renderer>();
        if (nebulaMaterial != null)
            backgroundRenderer.material = nebulaMaterial;
    }

    void Update()
    {
        uvOffset.x += scrollSpeed * Time.deltaTime;
        backgroundRenderer.material.SetTextureOffset("_MainTex", uvOffset);
    }
}