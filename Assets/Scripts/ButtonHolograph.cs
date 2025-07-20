using UnityEngine;
using UnityEngine.UI;

public class HolographicEffect : MonoBehaviour
{
    public float scanlineSpeed = 0.5f;
    public float scanlineWidth = 0.05f;
    public float scanlineIntensity = 0.5f;
    public Color hologramColor = new Color(0.2f, 0.6f, 1.0f);

    private Material hologramMaterial;
    private Image buttonImage;

    void Start()
    {
        buttonImage = GetComponent<Image>();

        // Create a holographic material
        hologramMaterial = new Material(Shader.Find("UI/Default"));
        hologramMaterial.SetColor("_Color", hologramColor);

        buttonImage.material = hologramMaterial;
    }

    void Update()
    {
        // Scanline effect
        float scanlinePos = (Time.time * scanlineSpeed) % 1.0f;
        hologramMaterial.SetFloat("_ScanlinePos", scanlinePos);
        hologramMaterial.SetFloat("_ScanlineWidth", scanlineWidth);
        hologramMaterial.SetFloat("_ScanlineIntensity", scanlineIntensity);
    }
}